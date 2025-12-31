namespace Stackage.OAuth2.Fake.Model.Authorization;

using System.Collections.Immutable;
using System.Security.Claims;

public record InternalAuthorization(
   string ClientId,
   Scope Scope,
   string[]? Audiences,
   string Subject,
   int? TokenExpirySeconds,
   ImmutableArray<Claim> Claims) : IAuthorizationWithClaims, IAuthorizationWithAudiences
{
   public bool IsAuthenticated => true;
}
