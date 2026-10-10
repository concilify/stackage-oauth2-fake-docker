namespace Stackage.OAuth2.Fake.Services;

using System.Collections.Generic;
using System.Security.Claims;
using Stackage.OAuth2.Fake.Model;

public class GoogleProviderClaimsStrategy : IProviderClaimsStrategy
{
   public string ProviderMode => ProviderModes.Google;

   public void AddAccessTokenClaims(User? user, List<Claim> claims)
   {
   }

   public void AddIdTokenClaims(User? user, List<Claim> claims)
   {
      if (user?.HostedDomain != null)
      {
         claims.Add(new Claim(ProviderClaimNames.Hd, user.HostedDomain));
      }
   }
}
