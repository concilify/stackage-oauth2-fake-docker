namespace Stackage.OAuth2.Fake.Services;

using System.Collections.Immutable;
using System.Security.Claims;
using System.Text.Json.Nodes;

public interface IClaimsParser
{
   bool TryParse(
      JsonObject claimsObject,
      out ImmutableArray<Claim> claims);
}
