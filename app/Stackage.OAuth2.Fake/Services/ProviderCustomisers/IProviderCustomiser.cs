namespace Stackage.OAuth2.Fake.Services.ProviderCustomisers;

using System.Collections.Generic;
using System.Security.Claims;
using Stackage.OAuth2.Fake.Model;

public interface IProviderCustomiser
{
   string ProviderMode { get; }

   void AddAccessTokenClaims(User? user, IList<Claim> claims);

   void AddIdTokenClaims(User? user, Scope scope, IList<Claim> claims);
}
