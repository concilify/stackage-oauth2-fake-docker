namespace Stackage.OAuth2.Fake.Services;

using System.Collections.Generic;
using System.Security.Claims;
using Stackage.OAuth2.Fake.Model;

public class EntraProviderClaimsStrategy : IProviderClaimsStrategy
{
   public string ProviderMode => ProviderModes.Entra;

   public void AddAccessTokenClaims(User? user, List<Claim> claims)
   {
      AddTenantIdClaim(user, claims);
   }

   public void AddIdTokenClaims(User? user, List<Claim> claims)
   {
      AddTenantIdClaim(user, claims);
   }

   private static void AddTenantIdClaim(User? user, List<Claim> claims)
   {
      if (user?.TenantId != null)
      {
         claims.Add(new Claim(ProviderClaimNames.Tid, user.TenantId));
      }
   }
}
