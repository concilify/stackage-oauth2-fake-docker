namespace Stackage.OAuth2.Fake.Model.Authorization;

using System.Collections.Immutable;
using System.Security.Claims;

public interface IAuthorizationWithClaims : IAuthorization
{
   ImmutableArray<Claim> Claims { get; }
}
