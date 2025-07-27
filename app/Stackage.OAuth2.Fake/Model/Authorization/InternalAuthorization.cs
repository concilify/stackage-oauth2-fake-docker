namespace Stackage.OAuth2.Fake.Model.Authorization;

using System.Collections.Generic;
using System.Security.Claims;

public record InternalAuthorization(string Subject, IEnumerable<Claim> Claims) : IAuthorizationWithClaims
{
   // TODO: Future PR - scope for device_code grants will be implemented later
   public Scope Scope => Scope.Empty;
}
