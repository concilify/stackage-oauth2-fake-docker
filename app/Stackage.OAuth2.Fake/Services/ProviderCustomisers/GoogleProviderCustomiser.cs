namespace Stackage.OAuth2.Fake.Services.ProviderCustomisers;

using System.Collections.Generic;
using System.Security.Claims;
using Stackage.OAuth2.Fake.Model;

public class GoogleProviderCustomiser : IProviderCustomiser
{
   private const string HdClaimName = "hd";

   public string ProviderMode => ProviderModes.Google;

   public void AddAccessTokenClaims(User? user, IList<Claim> claims)
   {
   }

   public void AddIdTokenClaims(User? user, Scope scope, IList<Claim> claims)
   {
      if (user?.HostedDomain != null)
      {
         claims.Add(new Claim(HdClaimName, user.HostedDomain));
      }
   }
}
