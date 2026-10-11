namespace Stackage.OAuth2.Fake.Services.ProviderCustomisers;

using System.Collections.Generic;
using System.Security.Claims;
using Stackage.OAuth2.Fake.Model;

public class EntraProviderCustomiser : IProviderCustomiser
{
   private const string TidClaimName = "tid";
   private const string OidClaimName = "oid";

   public string ProviderMode => ProviderModes.Entra;

   public void AddAccessTokenClaims(User? user, IList<Claim> claims)
   {
      AddTenantIdClaim(user, claims);
      AddObjectIdClaim(user, claims);
   }

   public void AddIdTokenClaims(User? user, Scope scope, IList<Claim> claims)
   {
      AddTenantIdClaim(user, claims);

      if (scope.Contains("profile"))
      {
         AddObjectIdClaim(user, claims);
      }
   }

   private static void AddTenantIdClaim(User? user, IList<Claim> claims)
   {
      if (user?.TenantId != null)
      {
         claims.Add(new Claim(TidClaimName, user.TenantId));
      }
   }

   private static void AddObjectIdClaim(User? user, IList<Claim> claims)
   {
      if (user?.ObjectId != null)
      {
         claims.Add(new Claim(OidClaimName, user.ObjectId));
      }
   }
}
