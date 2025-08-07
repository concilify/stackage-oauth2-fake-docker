namespace Stackage.OAuth2.Fake.Model.Authorization;

using System.Collections.Immutable;
using System.Security.Claims;

public record InternalAuthorization(
   Scope Scope,
   string Subject,
   int? TokenExpirySeconds,
   ImmutableArray<Claim> Claims) : IAuthorizationWithClaims
{
   public bool IsAuthenticated => true;
}
