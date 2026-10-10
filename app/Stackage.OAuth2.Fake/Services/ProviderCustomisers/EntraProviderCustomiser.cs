namespace Stackage.OAuth2.Fake.Services.ProviderCustomisers;

using System.Collections.Generic;
using System.Security.Claims;
using Stackage.OAuth2.Fake.Model;

public class EntraProviderCustomiser : IProviderCustomiser
{
   public string ProviderMode => ProviderModes.Entra;

   public void AddAccessTokenClaims(User? user, IList<Claim> claims)
   {
      AddTenantIdClaim(user, claims);
   }

   public void AddIdTokenClaims(User? user, IList<Claim> claims)
   {
      AddTenantIdClaim(user, claims);
   }

   private static void AddTenantIdClaim(User? user, IList<Claim> claims)
   {
      if (user?.TenantId != null)
      {
         claims.Add(new Claim(ProviderClaimNames.Tid, user.TenantId));
      }
   }
}
