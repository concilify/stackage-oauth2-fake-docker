namespace Stackage.OAuth2.Fake.Tests.Services.ProviderClaimsPolicies;

using System.Collections.Generic;
using System.Security.Claims;
using NUnit.Framework;
using Shouldly;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Services.ProviderClaimsPolicies;

// Every IProviderClaimsPolicy derives from DefaultProviderClaimsPolicy and must preserve its
// profile-claim filtering. This fixture exercises that shared behaviour against each concrete
// policy so it is not left to be re-implemented and re-tested per provider.
public class ProviderClaimsPolicyDefaultBehaviourTests
{
   private static readonly string[] ProfileClaimTypes = ["name", "nickname", "picture", "email"];

   [TestCaseSource(nameof(Policies))]
   public void get_access_token_claims_includes_non_profile_user_claims(IProviderClaimsPolicy testSubject)
   {
      var user = new User(
         Subject: "arbitrary-subject",
         Claims: [new Claim("arbitrary-claim", "ArbitraryValue")]);

      var claims = testSubject.GetAccessTokenClaims(user, Scope.Empty);

      claims.ShouldContain(c => c.Type == "arbitrary-claim" && c.Value == "ArbitraryValue");
   }

   [TestCaseSource(nameof(PoliciesWithProfileClaimTypes))]
   public void get_access_token_claims_excludes_profile_claims(IProviderClaimsPolicy testSubject, string claimType)
   {
      var user = new User(
         Subject: "arbitrary-subject",
         Claims: [new Claim(claimType, "ArbitraryValue")]);

      var claims = testSubject.GetAccessTokenClaims(user, Scope.Empty);

      claims.ShouldNotContain(c => c.Type == claimType);
   }

   [TestCaseSource(nameof(Policies))]
   public void get_id_token_claims_includes_profile_claims_when_scope_includes_profile(IProviderClaimsPolicy testSubject)
   {
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

   [TestCaseSource(nameof(Policies))]
   public void get_id_token_claims_excludes_non_profile_user_claims_when_scope_includes_profile(IProviderClaimsPolicy testSubject)
   {
      var user = new User(
         Subject: "arbitrary-subject",
         Claims: [new Claim("arbitrary-claim", "ArbitraryValue")]);

      var claims = testSubject.GetIdTokenClaims(user, (Scope)"profile");

      claims.ShouldNotContain(c => c.Type == "arbitrary-claim");
   }

   [TestCaseSource(nameof(Policies))]
   public void get_id_token_claims_is_empty_when_scope_does_not_include_profile(IProviderClaimsPolicy testSubject)
   {
      var user = new User(
         Subject: "arbitrary-subject",
         Claims: [new Claim("name", "ArbitraryName")]);

      var claims = testSubject.GetIdTokenClaims(user, Scope.Empty);

      claims.ShouldBeEmpty();
   }

   private static IEnumerable<TestCaseData> Policies()
   {
      foreach (var policy in CreatePolicies())
      {
         yield return new TestCaseData(policy).SetArgDisplayNames(policy.GetType().Name);
      }
   }

   private static IEnumerable<TestCaseData> PoliciesWithProfileClaimTypes()
   {
      foreach (var policy in CreatePolicies())
      {
         foreach (var claimType in ProfileClaimTypes)
         {
            yield return new TestCaseData(policy, claimType).SetArgDisplayNames(policy.GetType().Name, claimType);
         }
      }
   }

   private static IEnumerable<IProviderClaimsPolicy> CreatePolicies()
   {
      yield return new DefaultProviderClaimsPolicy();
      yield return new EntraProviderClaimsPolicy();
      yield return new GoogleProviderClaimsPolicy();
   }
}
