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
      return Claims.Where(claim => names.Any(name => string.Equals(claim.Type, name, StringComparison.InvariantCultureIgnoreCase)));
   }

   public IEnumerable<Claim> GetClaimsWithout(IEnumerable<string> names)
   {
      return Claims.Where(claim => !names.Any(name => string.Equals(claim.Type, name, StringComparison.InvariantCultureIgnoreCase)));
   }
}
