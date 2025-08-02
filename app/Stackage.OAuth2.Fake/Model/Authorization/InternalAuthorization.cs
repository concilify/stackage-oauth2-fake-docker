namespace Stackage.OAuth2.Fake.Model.Authorization;

using System.Collections.Generic;
using System.Security.Claims;

public record InternalAuthorization(
   Scope Scope,
   string Subject,
   int? TokenExpirySeconds,
   IEnumerable<Claim> Claims) : IAuthorizationWithClaims;
