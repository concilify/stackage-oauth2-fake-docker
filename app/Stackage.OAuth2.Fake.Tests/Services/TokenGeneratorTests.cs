namespace Stackage.OAuth2.Fake.Tests.Services;

using System;
using NUnit.Framework;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Model.Authorization;
using Stackage.OAuth2.Fake.Services;
using Stackage.OAuth2.Fake.Tests.Stubs;

public class TokenGeneratorTests
{
   [Test]
   public void throws_exception_when_unauthorized()
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
      Settings? settings = null,
      AuthorizationCache<RefreshAuthorization>? authorizationCache = null)
   {
      jsonWebKeyCache ??= new JsonWebKeyCache();
      settings ??= new Settings();
      authorizationCache ??= new AuthorizationCache<RefreshAuthorization>();

      return new TokenGenerator(
         jsonWebKeyCache,
         settings,
         authorizationCache);
   }
}
