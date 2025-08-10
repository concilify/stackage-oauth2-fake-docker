namespace Stackage.OAuth2.Fake.Tests.Services;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using Shouldly;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Model.Authorization;
using Stackage.OAuth2.Fake.Services;
using Stackage.OAuth2.Fake.Tests.Stubs;
using static Stackage.OAuth2.Fake.Tests.Support;

public class TokenGeneratorTests
{
   [Test]
   public void throws_exception_when_unauthenticated()
   {
      var testSubject = CreateGenerator();

      var authorization = AuthorizationStub.Unauthenticated();

      Assert.That(
         () => testSubject.Generate(authorization),
         Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("IAuthorizationProxy has not been authenticated."));
   }

   [Test]
   public void response_scope_is_null_when_scope_is_empty()
   {
      var testSubject = CreateGenerator();

      var authorization = AuthorizationStub.With(Scope.Empty);

      var response = testSubject.Generate(authorization);

      Assert.That(response.Scope, Is.Null);
   }

   [Test]
   public void response_scope_is_not_null_when_scope_is_not_empty()
   {
      var testSubject = CreateGenerator();

      var authorization = AuthorizationStub.With((Scope)"any_scope");

      var response = testSubject.Generate(authorization);

      Assert.That(response.Scope, Is.EqualTo("any_scope"));
   }

   [Test]
   public void response_refresh_token_is_null_when_scope_is_empty()
   {
      var testSubject = CreateGenerator();

      var authorization = AuthorizationStub.With(Scope.Empty);

      var response = testSubject.Generate(authorization);

      Assert.That(response.RefreshToken, Is.Null);
   }

   [Test]
   public void response_refresh_token_is_null_when_scope_is_not_offline_access()
   {
      var testSubject = CreateGenerator();

      var authorization = AuthorizationStub.With((Scope)"any_scope");

      var response = testSubject.Generate(authorization);

      Assert.That(response.RefreshToken, Is.Null);
   }

   [Test]
   public void response_refresh_token_is_not_null_when_scope_is_offline_access()
   {
      var testSubject = CreateGenerator();

      var authorization = AuthorizationStub.With((Scope)"offline_access");

      var response = testSubject.Generate(authorization);

      Assert.That(response.RefreshToken, Is.Not.Null);
   }

   [Test]
   public void response_id_token_is_null_when_scope_is_empty()
   {
      var testSubject = CreateGenerator();

      var authorization = AuthorizationStub.With(Scope.Empty);

      var response = testSubject.Generate(authorization);

      Assert.That(response.IdToken, Is.Null);
   }

   [TestCase("any_scope")]
   [TestCase("profile")]
   public void response_id_token_is_null_when_scope_is_not_openid(string scope)
   {
      var testSubject = CreateGenerator();

      var authorization = AuthorizationStub.With((Scope)scope);

      var response = testSubject.Generate(authorization);

      Assert.That(response.IdToken, Is.Null);
   }

   [TestCase("openid")]
   [TestCase("openid profile")]
   public void response_id_token_can_be_parsed_as_jwt_security_token_when_scope_includes_openid(string scope)
   {
      var testSubject = CreateGenerator();

      var authorization = AuthorizationStub.With((Scope)scope);

      var response = testSubject.Generate(authorization);

      var jwtSecurityToken = new JwtSecurityTokenHandler().ReadToken(response.IdToken) as JwtSecurityToken;

      Assert.That(jwtSecurityToken, Is.Not.Null);
   }

   [Test]
   public void response_id_token_contains_user_claims_when_scope_includes_openid_and_profile()
   {
      var user = new User(
         Subject: "the-subject",
         Claims: [
            new Claim("name", "name-claim"),
            new Claim("nickname", "nickname-claim"),
            new Claim("picture", "picture-claim")
         ]);
      var userStore = UserStoreStub.Returns(user);

      var testSubject = CreateGenerator(
         userStore: userStore);

      var authorization = AuthorizationStub.With(
         scope: (Scope)"openid profile",
         subject: "the-subject");

      var response = testSubject.Generate(authorization);

      var jwtSecurityToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(response.IdToken);

      var claims = jwtSecurityToken.ParseClaims("name", "nickname", "picture");

      var expectedClaims = new Dictionary<string, StringValues>
      {
         ["name"] = "name-claim",
         ["nickname"] = "nickname-claim",
         ["picture"] = "picture-claim"
      };

      claims.ShouldBeEquivalentTo(expectedClaims);
   }

   [Test]
   public void response_id_token_contains_available_user_claims_when_scope_includes_openid_and_profile()
   {
      var user = new User(
         Subject: "the-subject",
         Claims: [
            new Claim("name", "name-claim")
         ]);
      var userStore = UserStoreStub.Returns(user);

      var testSubject = CreateGenerator(
         userStore: userStore);

      var authorization = AuthorizationStub.With(
         scope: (Scope)"openid profile",
         subject: "the-subject");

      var response = testSubject.Generate(authorization);

      var jwtSecurityToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(response.IdToken);

      var claims = jwtSecurityToken.ParseClaims("name", "nickname", "picture");

      var expectedClaims = new Dictionary<string, StringValues>
      {
         ["name"] = "name-claim"
      };

      claims.ShouldBeEquivalentTo(expectedClaims);
   }

   [Test]
   public void response_id_token_does_not_contain_unknown_user_claims_when_scope_includes_openid_and_profile()
   {
      var user = new User(
         Subject: "the-subject",
         Claims: [
            new Claim("unknown-claim", "unknown-value")
         ]);
      var userStore = UserStoreStub.Returns(user);

      var testSubject = CreateGenerator(
         userStore: userStore);

      var authorization = AuthorizationStub.With(
         scope: (Scope)"openid profile",
         subject: "the-subject");

      var response = testSubject.Generate(authorization);

      var jwtSecurityToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(response.IdToken);

      var claims = jwtSecurityToken.ParseClaims("name", "nickname", "picture");

      claims.ShouldBeEmpty();
   }

   [Test]
   public void response_id_token_does_not_contain_user_claims_when_scope_does_not_include_profile()
   {
      var user = new User(
         Subject: "the-subject",
         Claims: [
            new Claim("name", "name-claim"),
            new Claim("nickname", "nickname-claim"),
            new Claim("picture", "picture-claim")
         ]);
      var userStore = UserStoreStub.Returns(user);

      var testSubject = CreateGenerator(
         userStore: userStore);

      var authorization = AuthorizationStub.With(
         scope: (Scope)"openid no-profile",
         subject: "the-subject");

      var response = testSubject.Generate(authorization);

      var jwtSecurityToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(response.IdToken);

      var claims = jwtSecurityToken.ParseClaims("name", "nickname", "picture");

      claims.ShouldBeEmpty();
   }

   [Test]
   public void response_id_token_does_not_contain_user_claims_when_scope_includes_profile_but_user_not_found()
   {
      var userStore = UserStoreStub.NotFound();

      var testSubject = CreateGenerator(
         userStore: userStore);

      var authorization = AuthorizationStub.With(
         scope: (Scope)"openid profile",
         subject: "the-subject");

      var response = testSubject.Generate(authorization);

      var jwtSecurityToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(response.IdToken);

      var claims = jwtSecurityToken.ParseClaims("name", "nickname", "picture");

      claims.ShouldBeEmpty();
   }

   [Test]
   public void authorization_cache_is_not_added_to_when_scope_is_not_offline_access()
   {
      var authorizationCache = new AuthorizationCache<RefreshAuthorization>();

      var testSubject = CreateGenerator(
         authorizationCache: authorizationCache);

      var authorization = AuthorizationStub.With((Scope)"any_scope");

      testSubject.Generate(authorization);

      Assert.That(authorizationCache.Count, Is.EqualTo(0));
   }

   [Test]
   public void authorization_cache_is_added_to_when_scope_is_offline_access()
   {
      var authorizationCache = new AuthorizationCache<RefreshAuthorization>();

      var testSubject = CreateGenerator(
         authorizationCache: authorizationCache);

      var authorization = AuthorizationStub.With(
         scope: (Scope)"offline_access",
         subject: "AnySubject");

      var response = testSubject.Generate(authorization);

      Assert.That(authorizationCache.Count, Is.EqualTo(1));

      authorizationCache.TryGet(response.RefreshToken!, out var cachedAuthorization);

      Assert.That(cachedAuthorization, Is.Not.Null);
      Assert.That(cachedAuthorization!.Scope, Is.SameAs(authorization.Scope));
      Assert.That(cachedAuthorization.Subject, Is.SameAs(authorization.Subject));
   }

   private static TokenGenerator CreateGenerator(
      JsonWebKeyCache? jsonWebKeyCache = null,
      IUserStore? userStore = null,
      Settings? settings = null,
      AuthorizationCache<RefreshAuthorization>? authorizationCache = null)
   {
      jsonWebKeyCache ??= new JsonWebKeyCache();
      userStore ??= UserStoreStub.Valid();
      settings ??= new Settings();
      authorizationCache ??= new AuthorizationCache<RefreshAuthorization>();

      return new TokenGenerator(
         jsonWebKeyCache,
         userStore,
         settings,
         authorizationCache);
   }
}
