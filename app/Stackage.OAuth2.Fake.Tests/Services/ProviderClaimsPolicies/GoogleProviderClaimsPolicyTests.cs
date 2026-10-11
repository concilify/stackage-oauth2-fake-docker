namespace Stackage.OAuth2.Fake.Tests.Services.ProviderClaimsPolicies;

using System.Security.Claims;
using NUnit.Framework;
using Shouldly;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Services.ProviderClaimsPolicies;

public class GoogleProviderClaimsPolicyTests
{
   [Test]
   public void provider_mode_is_google()
   {
      var testSubject = new GoogleProviderClaimsPolicy();

      Assert.That(testSubject.ProviderMode, Is.EqualTo("Google"));
   }

   [Test]
   public void get_id_token_claims_includes_hd_claim_when_user_has_hosted_domain()
   {
      var testSubject = new GoogleProviderClaimsPolicy();

      var user = new User(Subject: "arbitrary-subject", Claims: [], HostedDomain: "ArbitraryHostedDomain");

      var claims = testSubject.GetIdTokenClaims(user, Scope.Empty);

      claims.ShouldContain(c => c.Type == "hd" && c.Value == "ArbitraryHostedDomain");
   }

   [Test]
   public void get_id_token_claims_does_not_include_hd_claim_when_user_has_no_hosted_domain()
   {
      var testSubject = new GoogleProviderClaimsPolicy();

      var user = new User(Subject: "arbitrary-subject", Claims: []);

      var claims = testSubject.GetIdTokenClaims(user, Scope.Empty);

      claims.ShouldNotContain(c => c.Type == "hd");
   }
}
