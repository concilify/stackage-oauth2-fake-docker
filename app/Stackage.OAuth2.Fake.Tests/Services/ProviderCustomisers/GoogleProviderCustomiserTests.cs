namespace Stackage.OAuth2.Fake.Tests.Services.ProviderCustomisers;

using System.Collections.Generic;
using System.Security.Claims;
using NUnit.Framework;
using Shouldly;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Services.ProviderCustomisers;

public class GoogleProviderCustomiserTests
{
   [Test]
   public void provider_mode_is_google()
   {
      var testSubject = new GoogleProviderCustomiser();

      Assert.That(testSubject.ProviderMode, Is.EqualTo("Google"));
   }

   [Test]
   public void add_access_token_claims_does_not_add_any_claims_when_user_has_hosted_domain()
   {
      var testSubject = new GoogleProviderCustomiser();

      var user = new User(Subject: "arbitrary-subject", Claims: [], HostedDomain: "ArbitraryHostedDomain");
      var claims = new List<Claim>();

      testSubject.AddAccessTokenClaims(user, claims);

      claims.ShouldBeEmpty();
   }

   [Test]
   public void add_access_token_claims_does_not_add_any_claims_when_user_is_null()
   {
      var testSubject = new GoogleProviderCustomiser();

      var claims = new List<Claim>();

      testSubject.AddAccessTokenClaims(null, claims);

      claims.ShouldBeEmpty();
   }

   [Test]
   public void add_id_token_claims_adds_hd_claim_when_user_has_hosted_domain()
   {
      var testSubject = new GoogleProviderCustomiser();

      var user = new User(Subject: "arbitrary-subject", Claims: [], HostedDomain: "ArbitraryHostedDomain");
      var claims = new List<Claim>();

      testSubject.AddIdTokenClaims(user, Scope.Empty, claims);

      claims.ShouldContain(c => c.Type == "hd" && c.Value == "ArbitraryHostedDomain");
   }

   [Test]
   public void add_id_token_claims_does_not_add_hd_claim_when_user_has_no_hosted_domain()
   {
      var testSubject = new GoogleProviderCustomiser();

      var user = new User(Subject: "arbitrary-subject", Claims: []);
      var claims = new List<Claim>();

      testSubject.AddIdTokenClaims(user, Scope.Empty, claims);

      claims.ShouldNotContain(c => c.Type == "hd");
   }

   [Test]
   public void add_id_token_claims_does_not_add_hd_claim_when_user_is_null()
   {
      var testSubject = new GoogleProviderCustomiser();

      var claims = new List<Claim>();

      testSubject.AddIdTokenClaims(null, Scope.Empty, claims);

      claims.ShouldBeEmpty();
   }
}
