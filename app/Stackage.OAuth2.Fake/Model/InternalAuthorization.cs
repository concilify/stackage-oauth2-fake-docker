namespace Stackage.OAuth2.Fake.Model;

using System.Collections.Generic;
using System.Security.Claims;

public record InternalAuthorization(Scope Scope, string Subject, IEnumerable<Claim> Claims) : IAuthorization;
