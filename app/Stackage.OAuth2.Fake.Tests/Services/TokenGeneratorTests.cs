namespace Stackage.OAuth2.Fake.Tests.Services;

using NUnit.Framework;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Services;
using Stackage.OAuth2.Fake.Tests.Stubs;

public class TokenGeneratorTests
{
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

   private static TokenGenerator CreateGenerator(
      JsonWebKeyCache? jsonWebKeyCache = null,
      Settings? settings = null)
   {
      jsonWebKeyCache ??= new JsonWebKeyCache();
      settings ??= new Settings();

      return new TokenGenerator(
         jsonWebKeyCache,
         settings);
   }
}
