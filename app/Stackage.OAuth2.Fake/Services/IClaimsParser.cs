namespace Stackage.OAuth2.Fake.Services;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Json.Nodes;

public interface IClaimsParser
{
   bool TryParse(
      JsonObject claimsObject,
      [MaybeNullWhen(false)] out IList<Claim> claims);
}
