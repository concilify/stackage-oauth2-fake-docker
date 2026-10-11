namespace Stackage.OAuth2.Fake.Tests.Services.ProviderClaimsPolicies;

using System.Security.Claims;
using NUnit.Framework;
using Shouldly;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Services.ProviderClaimsPolicies;

public class DefaultProviderClaimsPolicyTests
{
   [Test]
   public void provider_mode_is_default()
   {
      var testSubject = new DefaultProviderClaimsPolicy();

      Assert.That(testSubject.ProviderMode, Is.EqualTo("Default"));
   }

   [Test]
   public void get_access_token_claims_includes_non_profile_user_claims()
   {
      var testSubject = new DefaultProviderClaimsPolicy();

      var user = new User(
         Subject: "arbitrary-subject",
         Claims: [new Claim("arbitrary-claim", "ArbitraryValue")]);

      var claims = testSubject.GetAccessTokenClaims(user, Scope.Empty);

      claims.ShouldContain(c => c.Type == "arbitrary-claim" && c.Value == "ArbitraryValue");
   }

   [TestCase("name")]
   [TestCase("nickname")]
   [TestCase("picture")]
   [TestCase("email")]
   public void get_access_token_claims_excludes_profile_claims(string claimType)
   {
      var testSubject = new DefaultProviderClaimsPolicy();

      var user = new User(
         Subject: "arbitrary-subject",
         Claims: [new Claim(claimType, "ArbitraryValue")]);

      var claims = testSubject.GetAccessTokenClaims(user, Scope.Empty);

      claims.ShouldNotContain(c => c.Type == claimType);
   }

   [Test]
   public void get_id_token_claims_includes_profile_claims_when_scope_includes_profile()
   {
      var testSubject = new DefaultProviderClaimsPolicy();

      var user = new User(
         Subject: "arbitrary-subject",
         Claims: [
            new Claim("name", "ArbitraryName"),
            new Claim("nickname", "ArbitraryNickname"),
            new Claim("picture", "ArbitraryPicture"),
            new Claim("email", "user@example.com"),
         ]);

      var claims = testSubject.GetIdTokenClaims(user, (Scope)"profile");

      claims.ShouldContain(c => c.Type == "name" && c.Value == "ArbitraryName");
      claims.ShouldContain(c => c.Type == "nickname" && c.Value == "ArbitraryNickname");
      claims.ShouldContain(c => c.Type == "picture" && c.Value == "ArbitraryPicture");
      claims.ShouldContain(c => c.Type == "email" && c.Value == "user@example.com");
   }

   [Test]
   public void get_id_token_claims_excludes_non_profile_user_claims_when_scope_includes_profile()
   {
      var testSubject = new DefaultProviderClaimsPolicy();

      var user = new User(
         Subject: "arbitrary-subject",
         Claims: [new Claim("arbitrary-claim", "ArbitraryValue")]);

      var claims = testSubject.GetIdTokenClaims(user, (Scope)"profile");

      claims.ShouldNotContain(c => c.Type == "arbitrary-claim");
   }

   [Test]
   public void get_id_token_claims_is_empty_when_scope_does_not_include_profile()
   {
      var testSubject = new DefaultProviderClaimsPolicy();

      var user = new User(
         Subject: "arbitrary-subject",
         Claims: [new Claim("name", "ArbitraryName")]);

      var claims = testSubject.GetIdTokenClaims(user, Scope.Empty);

      claims.ShouldBeEmpty();
   }
}
