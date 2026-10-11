namespace Stackage.OAuth2.Fake.Tests.Services.ProviderCustomisers;

using System.Collections.Generic;
using System.Security.Claims;
using NUnit.Framework;
using Shouldly;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Services.ProviderCustomisers;

public class EntraProviderCustomiserTests
{
   [Test]
   public void provider_mode_is_entra()
   {
      var testSubject = new EntraProviderCustomiser();

      Assert.That(testSubject.ProviderMode, Is.EqualTo("Entra"));
   }

   [Test]
   public void add_access_token_claims_adds_tid_claim_when_user_has_tenant_id()
   {
      var testSubject = new EntraProviderCustomiser();

      var user = new User(Subject: "arbitrary-subject", Claims: [], TenantId: "ArbitraryTenantId");
      var claims = new List<Claim>();

      testSubject.AddAccessTokenClaims(user, claims);

      claims.ShouldContain(c => c.Type == "tid" && c.Value == "ArbitraryTenantId");
   }

   [Test]
   public void add_access_token_claims_does_not_add_tid_claim_when_user_has_no_tenant_id()
   {
      var testSubject = new EntraProviderCustomiser();

      var user = new User(Subject: "arbitrary-subject", Claims: []);
      var claims = new List<Claim>();

      testSubject.AddAccessTokenClaims(user, claims);

      claims.ShouldNotContain(c => c.Type == "tid");
   }

   [Test]
   public void add_access_token_claims_does_not_add_tid_claim_when_user_is_null()
   {
      var testSubject = new EntraProviderCustomiser();

      var claims = new List<Claim>();

      testSubject.AddAccessTokenClaims(null, claims);

      claims.ShouldNotContain(c => c.Type == "tid");
   }

   [Test]
   public void add_access_token_claims_adds_oid_claim_when_user_has_object_id()
   {
      var testSubject = new EntraProviderCustomiser();

      var user = new User(Subject: "arbitrary-subject", Claims: [], ObjectId: "ArbitraryObjectId");
      var claims = new List<Claim>();

      testSubject.AddAccessTokenClaims(user, claims);

      claims.ShouldContain(c => c.Type == "oid" && c.Value == "ArbitraryObjectId");
   }

   [Test]
   public void add_access_token_claims_does_not_add_oid_claim_when_user_has_no_object_id()
   {
      var testSubject = new EntraProviderCustomiser();

      var user = new User(Subject: "arbitrary-subject", Claims: []);
      var claims = new List<Claim>();

      testSubject.AddAccessTokenClaims(user, claims);

      claims.ShouldNotContain(c => c.Type == "oid");
   }

   [Test]
   public void add_access_token_claims_does_not_add_oid_claim_when_user_is_null()
   {
      var testSubject = new EntraProviderCustomiser();

      var claims = new List<Claim>();

      testSubject.AddAccessTokenClaims(null, claims);

      claims.ShouldNotContain(c => c.Type == "oid");
   }

   [Test]
   public void add_id_token_claims_adds_tid_claim_when_user_has_tenant_id()
   {
      var testSubject = new EntraProviderCustomiser();

      var user = new User(Subject: "arbitrary-subject", Claims: [], TenantId: "ArbitraryTenantId");
      var claims = new List<Claim>();

      testSubject.AddIdTokenClaims(user, Scope.Empty, claims);

      claims.ShouldContain(c => c.Type == "tid" && c.Value == "ArbitraryTenantId");
   }

   [Test]
   public void add_id_token_claims_does_not_add_tid_claim_when_user_has_no_tenant_id()
   {
      var testSubject = new EntraProviderCustomiser();

      var user = new User(Subject: "arbitrary-subject", Claims: []);
      var claims = new List<Claim>();

      testSubject.AddIdTokenClaims(user, Scope.Empty, claims);

      claims.ShouldNotContain(c => c.Type == "tid");
   }

   [Test]
   public void add_id_token_claims_adds_oid_claim_when_user_has_object_id_and_scope_includes_profile()
   {
      var testSubject = new EntraProviderCustomiser();

      var user = new User(Subject: "arbitrary-subject", Claims: [], ObjectId: "ArbitraryObjectId");
      var claims = new List<Claim>();

      testSubject.AddIdTokenClaims(user, (Scope)"profile", claims);

      claims.ShouldContain(c => c.Type == "oid" && c.Value == "ArbitraryObjectId");
   }

   [Test]
   public void add_id_token_claims_does_not_add_oid_claim_when_scope_does_not_include_profile()
   {
      var testSubject = new EntraProviderCustomiser();

      var user = new User(Subject: "arbitrary-subject", Claims: [], ObjectId: "ArbitraryObjectId");
      var claims = new List<Claim>();

      testSubject.AddIdTokenClaims(user, Scope.Empty, claims);

      claims.ShouldNotContain(c => c.Type == "oid");
   }

   [Test]
   public void add_id_token_claims_does_not_add_oid_claim_when_user_has_no_object_id()
   {
      var testSubject = new EntraProviderCustomiser();

      var user = new User(Subject: "arbitrary-subject", Claims: []);
      var claims = new List<Claim>();

      testSubject.AddIdTokenClaims(user, (Scope)"profile", claims);

      claims.ShouldNotContain(c => c.Type == "oid");
   }

   [Test]
   public void add_id_token_claims_does_not_add_tid_or_oid_claim_when_user_is_null()
   {
      var testSubject = new EntraProviderCustomiser();

      var claims = new List<Claim>();

      testSubject.AddIdTokenClaims(null, (Scope)"profile", claims);

      claims.ShouldBeEmpty();
   }
}
