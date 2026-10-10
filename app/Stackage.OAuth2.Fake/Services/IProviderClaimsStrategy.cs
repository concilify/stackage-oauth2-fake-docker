namespace Stackage.OAuth2.Fake.Services;

using System.Collections.Generic;
using System.Security.Claims;
using Stackage.OAuth2.Fake.Model;

public interface IProviderClaimsStrategy
{
   string ProviderMode { get; }

   void AddAccessTokenClaims(User? user, List<Claim> claims);

   void AddIdTokenClaims(User? user, List<Claim> claims);
}
