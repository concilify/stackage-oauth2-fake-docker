namespace Stackage.OAuth2.Fake.Model;

using System.Collections.Generic;
using System.Security.Claims;

public interface IAuthorizationWithClaims : IAuthorization
{
   IEnumerable<Claim> Claims { get; }
}
