namespace Stackage.OAuth2.Fake.Model;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;

public record User(string Subject, ImmutableArray<Claim> Claims)
{
   public IEnumerable<Claim> GetClaims(IEnumerable<string> names)
   {
      return Claims.Join(
         names,
         claim => claim.Type,
         name => name,
         (claim, _) => claim,
         StringComparer.InvariantCultureIgnoreCase);
   }
}
