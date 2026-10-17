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
   public void get_id_token_claims_includes_hd_claim_when_user_has_tenant_id()
   {
      var testSubject = new GoogleProviderClaimsPolicy();

      var user = new User(Subject: "arbitrary-subject", Claims: [], TenantId: "ArbitraryTenantId");

      var claims = testSubject.GetIdTokenClaims(user, Scope.Empty);

      claims.ShouldContain(c => c.Type == "hd" && c.Value == "ArbitraryTenantId");
   }

   [Test]
   public void get_id_token_claims_does_not_include_hd_claim_when_user_has_no_tenant_id()
   {
      var testSubject = new GoogleProviderClaimsPolicy();

      var user = new User(Subject: "arbitrary-subject", Claims: []);

      var claims = testSubject.GetIdTokenClaims(user, Scope.Empty);

      claims.ShouldNotContain(c => c.Type == "hd");
   }

   [TestCase(true, "true")]
   [TestCase(false, "false")]
   public void get_id_token_claims_includes_email_verified_claim_when_user_has_verified(bool verified, string expectedValue)
   {
      var testSubject = new GoogleProviderClaimsPolicy();

      var user = new User(Subject: "arbitrary-subject", Claims: [], Verified: verified);

      var claims = testSubject.GetIdTokenClaims(user, Scope.Empty);

      claims.ShouldContain(c => c.Type == "email_verified" && c.Value == expectedValue && c.ValueType == ClaimValueTypes.Boolean);
   }

   [Test]
   public void get_id_token_claims_does_not_include_email_verified_claim_when_user_has_no_verified()
   {
      var testSubject = new GoogleProviderClaimsPolicy();

      var user = new User(Subject: "arbitrary-subject", Claims: []);

      var claims = testSubject.GetIdTokenClaims(user, Scope.Empty);

      claims.ShouldNotContain(c => c.Type == "email_verified");
   }

   [Test]
   public void get_access_token_claims_does_not_include_email_verified_claim_when_user_has_verified()
   {
      var testSubject = new GoogleProviderClaimsPolicy();

      var user = new User(Subject: "arbitrary-subject", Claims: [], Verified: true);

      var claims = testSubject.GetAccessTokenClaims(user, Scope.Empty);

      claims.ShouldNotContain(c => c.Type == "email_verified");
   }

   [Test]
   public void get_access_token_claims_does_not_include_hd_claim_when_user_has_tenant_id()
   {
      var testSubject = new GoogleProviderClaimsPolicy();

      var user = new User(Subject: "arbitrary-subject", Claims: [], TenantId: "ArbitraryTenantId");

      var claims = testSubject.GetAccessTokenClaims(user, Scope.Empty);

      claims.ShouldNotContain(c => c.Type == "hd");
   }
}
