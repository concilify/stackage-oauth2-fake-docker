namespace Stackage.OAuth2.Fake.Services.ProviderClaimsPolicies;

using System.Collections.Immutable;
using System.Security.Claims;
using Stackage.OAuth2.Fake.Model;

public interface IProviderClaimsPolicy
{
   string ProviderMode { get; }

   ImmutableArray<Claim> GetAccessTokenClaims(User user, Scope scope);

   ImmutableArray<Claim> GetIdTokenClaims(User user, Scope scope);
}
