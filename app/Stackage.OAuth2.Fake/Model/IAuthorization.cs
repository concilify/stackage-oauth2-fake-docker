namespace Stackage.OAuth2.Fake.Model;

using System.Collections.Generic;
using System.Security.Claims;

public interface IAuthorization
{
   Scope Scope { get; }

   string Subject { get; }

   IEnumerable<Claim> Claims { get; }
}
