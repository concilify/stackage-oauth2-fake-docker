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
using Stackage.OAuth2.Fake.Services.ProviderClaimsPolicies;
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
   public void throws_exception_when_no_provider_claims_policy_matches_provider_mode()
   {
      var testSubject = CreateGenerator(
         settings: new Settings { ProviderMode = "UNSUPPORTED_MODE" },
         providerClaimsPolicies: []);

      var authorization = AuthorizationStub.With();

      Assert.That(
         () => testSubject.Generate(authorization),
         Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("ProviderMode UNSUPPORTED_MODE is not supported."));
   }

   [Test]
   public void response_scope_is_null_when_scope_is_empty()
   {
      var testSubject = CreateGenerator();

      var authorization = AuthorizationStub.With(scope: Scope.Empty);

      var response = testSubject.Generate(authorization);

      Assert.That(response.Scope, Is.Null);
   }

   [Test]
   public void response_scope_is_not_null_when_scope_is_not_empty()
   {
      var testSubject = CreateGenerator();

      var authorization = AuthorizationStub.With(scope: (Scope)"arbitrary_scope");

      var response = testSubject.Generate(authorization);

      Assert.That(response.Scope, Is.EqualTo("arbitrary_scope"));
   }

   [Test]
   public void response_access_token_contains_claims_from_matching_provider_claims_policy_when_user_found()
   {
      var user = new User(Subject: "arbitrary-subject", Claims: []);
      var userStore = UserStoreStub.Returns(user);

      var providerClaimsPolicy = ProviderClaimsPolicyStub.AddingClaims(
         providerMode: "Default",
         accessTokenClaim: new Claim("arbitrary-access-token-claim", "ArbitraryAccessTokenValue"),
         idTokenClaim: new Claim("arbitrary-id-token-claim", "ArbitraryIdTokenValue"));

      var testSubject = CreateGenerator(
         userStore: userStore,
         providerClaimsPolicies: [providerClaimsPolicy]);

      var authorization = AuthorizationStub.With(subject: "arbitrary-subject");

      var response = testSubject.Generate(authorization);

      var jwtSecurityToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(response.AccessToken);

      var claims = jwtSecurityToken.ParseClaims("arbitrary-access-token-claim");

      var expectedClaims = new Dictionary<string, StringValues>
      {
         ["arbitrary-access-token-claim"] = "ArbitraryAccessTokenValue",
      };

      claims.ShouldBeEquivalentTo(expectedClaims);
   }

   [Test]
   public void response_access_token_does_not_contain_claims_from_provider_claims_policy_when_user_not_found()
   {
      var userStore = UserStoreStub.NotFound("arbitrary-subject");

      var providerClaimsPolicy = ProviderClaimsPolicyStub.AddingClaims(
         providerMode: "Default",
         accessTokenClaim: new Claim("arbitrary-access-token-claim", "ArbitraryAccessTokenValue"),
         idTokenClaim: new Claim("arbitrary-id-token-claim", "ArbitraryIdTokenValue"));

      var testSubject = CreateGenerator(
         userStore: userStore,
         providerClaimsPolicies: [providerClaimsPolicy]);

      var authorization = AuthorizationStub.With(subject: "arbitrary-subject");

      var response = testSubject.Generate(authorization);

      var jwtSecurityToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(response.AccessToken);

      var claims = jwtSecurityToken.ParseClaims("arbitrary-access-token-claim");

      claims.ShouldBeEmpty();
   }

   [Test]
   public void response_refresh_token_is_null_when_scope_is_empty()
   {
      var testSubject = CreateGenerator();

      var authorization = AuthorizationStub.With(scope: Scope.Empty);

      var response = testSubject.Generate(authorization);

      Assert.That(response.RefreshToken, Is.Null);
   }

   [Test]
   public void response_refresh_token_is_null_when_scope_is_not_offline_access()
   {
      var testSubject = CreateGenerator();

      var authorization = AuthorizationStub.With(scope: (Scope)"valid_scope");

      var response = testSubject.Generate(authorization);

      Assert.That(response.RefreshToken, Is.Null);
   }

   [Test]
   public void response_refresh_token_is_not_null_when_scope_is_offline_access()
   {
      var testSubject = CreateGenerator();

      var authorization = AuthorizationStub.With(scope: (Scope)"offline_access");

      var response = testSubject.Generate(authorization);

      Assert.That(response.RefreshToken, Is.Not.Null);
   }

   [Test]
   public void response_id_token_is_null_when_scope_is_empty()
   {
      var testSubject = CreateGenerator();

      var authorization = AuthorizationStub.With(scope: Scope.Empty);

      var response = testSubject.Generate(authorization);

      Assert.That(response.IdToken, Is.Null);
   }

   [TestCase("valid_scope")]
   [TestCase("profile")]
   public void response_id_token_is_null_when_scope_is_not_openid(string scope)
   {
      var testSubject = CreateGenerator();

      var authorization = AuthorizationStub.With(scope: (Scope)scope);

      var response = testSubject.Generate(authorization);

      Assert.That(response.IdToken, Is.Null);
   }

   [TestCase("openid")]
   [TestCase("openid profile")]
   public void response_id_token_can_be_parsed_as_jwt_security_token_when_scope_includes_openid(string scope)
   {
      var testSubject = CreateGenerator();

      var authorization = AuthorizationStub.With(scope: (Scope)scope);

      var response = testSubject.Generate(authorization);

      var jwtSecurityToken = new JwtSecurityTokenHandler().ReadToken(response.IdToken) as JwtSecurityToken;

      Assert.That(jwtSecurityToken, Is.Not.Null);
   }

   [Test]
   public void response_id_token_contains_claims_from_matching_provider_claims_policy_when_user_found()
   {
      var user = new User(Subject: "arbitrary-subject", Claims: []);
      var userStore = UserStoreStub.Returns(user);

      var providerClaimsPolicy = ProviderClaimsPolicyStub.AddingClaims(
         providerMode: "Default",
         accessTokenClaim: new Claim("arbitrary-access-token-claim", "ArbitraryAccessTokenValue"),
         idTokenClaim: new Claim("arbitrary-id-token-claim", "ArbitraryIdTokenValue"));

      var testSubject = CreateGenerator(
         userStore: userStore,
         providerClaimsPolicies: [providerClaimsPolicy]);

      var authorization = AuthorizationStub.With(
         scope: (Scope)"openid",
         subject: "arbitrary-subject");

      var response = testSubject.Generate(authorization);

      var jwtSecurityToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(response.IdToken);

      var claims = jwtSecurityToken.ParseClaims("arbitrary-id-token-claim");

      var expectedClaims = new Dictionary<string, StringValues>
      {
         ["arbitrary-id-token-claim"] = "ArbitraryIdTokenValue",
      };

      claims.ShouldBeEquivalentTo(expectedClaims);
   }

   [Test]
   public void response_id_token_contains_boolean_claims_from_provider_claims_policy_as_json_booleans()
   {
      var user = new User(Subject: "arbitrary-subject", Claims: []);
      var userStore = UserStoreStub.Returns(user);

      var providerClaimsPolicy = ProviderClaimsPolicyStub.AddingClaims(
         providerMode: "Default",
         accessTokenClaim: new Claim("arbitrary-access-token-claim", "ArbitraryAccessTokenValue"),
         idTokenClaim: new Claim("arbitrary-id-token-claim", "true", ClaimValueTypes.Boolean));

      var testSubject = CreateGenerator(
         userStore: userStore,
         providerClaimsPolicies: [providerClaimsPolicy]);

      var authorization = AuthorizationStub.With(
         scope: (Scope)"openid",
         subject: "arbitrary-subject");

      var response = testSubject.Generate(authorization);

      var jwtSecurityToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(response.IdToken);

      jwtSecurityToken.Payload["arbitrary-id-token-claim"].ShouldBe(true);
   }

   [Test]
   public void response_id_token_does_not_contain_claims_from_provider_claims_policy_when_user_not_found()
   {
      var userStore = UserStoreStub.NotFound("arbitrary-subject");

      var providerClaimsPolicy = ProviderClaimsPolicyStub.AddingClaims(
         providerMode: "Default",
         accessTokenClaim: new Claim("arbitrary-access-token-claim", "ArbitraryAccessTokenValue"),
         idTokenClaim: new Claim("arbitrary-id-token-claim", "ArbitraryIdTokenValue"));

      var testSubject = CreateGenerator(
         userStore: userStore,
         providerClaimsPolicies: [providerClaimsPolicy]);

      var authorization = AuthorizationStub.With(
         scope: (Scope)"openid",
         subject: "arbitrary-subject");

      var response = testSubject.Generate(authorization);

      var jwtSecurityToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(response.IdToken);

      var claims = jwtSecurityToken.ParseClaims("arbitrary-id-token-claim");

      claims.ShouldBeEmpty();
   }

   [Test]
   public void response_id_token_contains_nonce_when_scope_includes_openid_and_authorization_contains_nonce()
   {
      var testSubject = CreateGenerator();

      var authorization = UserAuthorization.Create(
         clientId: "ArbitraryClientId",
         scope: (Scope)"openid",
         audience: null,
         nonce: "ArbitraryNonce");
      authorization.Authenticate("arbitrary-subject");

      var response = testSubject.Generate(authorization);

      var jwtSecurityToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(response.IdToken);

      var claims = jwtSecurityToken.ParseClaims("nonce");

      var expectedClaims = new Dictionary<string, StringValues>
      {
         ["nonce"] = "ArbitraryNonce",
      };

      claims.ShouldBeEquivalentTo(expectedClaims);
   }

   [Test]
   public void response_access_token_does_not_contain_nonce_when_scope_includes_openid_and_authorization_contains_nonce()
   {
      var testSubject = CreateGenerator();

      var authorization = UserAuthorization.Create(
         clientId: "ArbitraryClientId",
         scope: (Scope)"openid",
         audience: null,
         nonce: "ArbitraryNonce");
      authorization.Authenticate("arbitrary-subject");

      var response = testSubject.Generate(authorization);

      var jwtSecurityToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(response.AccessToken);

      var claims = jwtSecurityToken.ParseClaims("nonce");

      claims.ShouldBeEmpty();
   }

   [Test]
   public void response_id_token_does_not_contain_nonce_when_generated_from_refresh_authorization()
   {
      var testSubject = CreateGenerator();

      var userAuthorization = UserAuthorization.Create(
         clientId: "ArbitraryClientId",
         scope: (Scope)"openid offline_access",
         audience: null,
         nonce: "ArbitraryNonce");
      userAuthorization.Authenticate("arbitrary-subject");
      var refreshAuthorization = RefreshAuthorization.Create("ArbitraryRefreshToken", userAuthorization);

      var response = testSubject.Generate(refreshAuthorization);

      var jwtSecurityToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(response.IdToken);

      var claims = jwtSecurityToken.ParseClaims("nonce");

      claims.ShouldBeEmpty();
   }

   [Test]
   public void authorization_cache_is_not_added_to_when_scope_is_not_offline_access()
   {
      var authorizationCache = new AuthorizationCache<RefreshAuthorization>();

      var testSubject = CreateGenerator(
         authorizationCache: authorizationCache);

      var authorization = AuthorizationStub.With(scope: (Scope)"valid_scope");

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
         subject: "valid-subject");

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
      AuthorizationCache<RefreshAuthorization>? authorizationCache = null,
      IEnumerable<IProviderClaimsPolicy>? providerClaimsPolicies = null)
   {
      jsonWebKeyCache ??= new JsonWebKeyCache();
      userStore ??= UserStoreStub.Valid();
      settings ??= new Settings();
      authorizationCache ??= new AuthorizationCache<RefreshAuthorization>();
      providerClaimsPolicies ??= [ProviderClaimsPolicyStub.ReturningNoClaims(settings.ProviderMode)];

      return new TokenGenerator(
         jsonWebKeyCache,
         userStore,
         settings,
         authorizationCache,
         providerClaimsPolicies);
   }
}
