namespace Stackage.OAuth2.Fake.Tests.Services.ProviderClaimsPolicies;

using System.Security.Claims;
using NUnit.Framework;
using Shouldly;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Services.ProviderClaimsPolicies;

public class EntraProviderClaimsPolicyTests
{
   [Test]
   public void provider_mode_is_entra()
   {
      var testSubject = new EntraProviderClaimsPolicy();

      Assert.That(testSubject.ProviderMode, Is.EqualTo("Entra"));
   }

   [Test]
   public void get_access_token_claims_includes_tid_claim_when_user_has_tenant_id()
   {
      var testSubject = new EntraProviderClaimsPolicy();

      var user = new User(Subject: "arbitrary-subject", Claims: [], TenantId: "ArbitraryTenantId");

      var claims = testSubject.GetAccessTokenClaims(user, Scope.Empty);

      claims.ShouldContain(c => c.Type == "tid" && c.Value == "ArbitraryTenantId");
   }

   [Test]
   public void get_access_token_claims_does_not_include_tid_claim_when_user_has_no_tenant_id()
   {
      var testSubject = new EntraProviderClaimsPolicy();

      var user = new User(Subject: "arbitrary-subject", Claims: []);

      var claims = testSubject.GetAccessTokenClaims(user, Scope.Empty);

      claims.ShouldNotContain(c => c.Type == "tid");
   }

   [Test]
   public void get_access_token_claims_includes_oid_claim_when_user_has_object_id()
   {
      var testSubject = new EntraProviderClaimsPolicy();

      var user = new User(Subject: "arbitrary-subject", Claims: [], ObjectId: "ArbitraryObjectId");

      var claims = testSubject.GetAccessTokenClaims(user, Scope.Empty);

      claims.ShouldContain(c => c.Type == "oid" && c.Value == "ArbitraryObjectId");
   }

   [Test]
   public void get_access_token_claims_does_not_include_oid_claim_when_user_has_no_object_id()
   {
      var testSubject = new EntraProviderClaimsPolicy();

      var user = new User(Subject: "arbitrary-subject", Claims: []);

      var claims = testSubject.GetAccessTokenClaims(user, Scope.Empty);

      claims.ShouldNotContain(c => c.Type == "oid");
   }

   [Test]
   public void get_id_token_claims_includes_tid_claim_when_user_has_tenant_id()
   {
      var testSubject = new EntraProviderClaimsPolicy();

      var user = new User(Subject: "arbitrary-subject", Claims: [], TenantId: "ArbitraryTenantId");

      var claims = testSubject.GetIdTokenClaims(user, Scope.Empty);

      claims.ShouldContain(c => c.Type == "tid" && c.Value == "ArbitraryTenantId");
   }

   [Test]
   public void get_id_token_claims_does_not_include_tid_claim_when_user_has_no_tenant_id()
   {
      var testSubject = new EntraProviderClaimsPolicy();

      var user = new User(Subject: "arbitrary-subject", Claims: []);

      var claims = testSubject.GetIdTokenClaims(user, Scope.Empty);

      claims.ShouldNotContain(c => c.Type == "tid");
   }

   [Test]
   public void get_id_token_claims_includes_oid_claim_when_user_has_object_id_and_scope_includes_profile()
   {
      var testSubject = new EntraProviderClaimsPolicy();

      var user = new User(Subject: "arbitrary-subject", Claims: [], ObjectId: "ArbitraryObjectId");

      var claims = testSubject.GetIdTokenClaims(user, (Scope)"profile");

      claims.ShouldContain(c => c.Type == "oid" && c.Value == "ArbitraryObjectId");
   }

   [Test]
   public void get_id_token_claims_does_not_include_oid_claim_when_scope_does_not_include_profile()
   {
      var testSubject = new EntraProviderClaimsPolicy();

      var user = new User(Subject: "arbitrary-subject", Claims: [], ObjectId: "ArbitraryObjectId");

      var claims = testSubject.GetIdTokenClaims(user, Scope.Empty);

      claims.ShouldNotContain(c => c.Type == "oid");
   }

   [Test]
   public void get_id_token_claims_does_not_include_oid_claim_when_user_has_no_object_id()
   {
      var testSubject = new EntraProviderClaimsPolicy();

      var user = new User(Subject: "arbitrary-subject", Claims: []);

      var claims = testSubject.GetIdTokenClaims(user, (Scope)"profile");

      claims.ShouldNotContain(c => c.Type == "oid");
   }

   [TestCase(true, "true")]
   [TestCase(false, "false")]
   public void get_id_token_claims_includes_xms_edov_claim_when_user_has_verified(bool verified, string expectedValue)
   {
      var testSubject = new EntraProviderClaimsPolicy();

      var user = new User(Subject: "arbitrary-subject", Claims: [], Verified: verified);

      var claims = testSubject.GetIdTokenClaims(user, Scope.Empty);

      claims.ShouldContain(c => c.Type == "xms_edov" && c.Value == expectedValue && c.ValueType == ClaimValueTypes.Boolean);
   }

   [Test]
   public void get_id_token_claims_does_not_include_xms_edov_claim_when_user_has_no_verified()
   {
      var testSubject = new EntraProviderClaimsPolicy();

      var user = new User(Subject: "arbitrary-subject", Claims: []);

      var claims = testSubject.GetIdTokenClaims(user, Scope.Empty);

      claims.ShouldNotContain(c => c.Type == "xms_edov");
   }

   [Test]
   public void get_access_token_claims_does_not_include_xms_edov_claim_when_user_has_verified()
   {
      var testSubject = new EntraProviderClaimsPolicy();

      var user = new User(Subject: "arbitrary-subject", Claims: [], Verified: true);

      var claims = testSubject.GetAccessTokenClaims(user, Scope.Empty);

      claims.ShouldNotContain(c => c.Type == "xms_edov");
   }
}
