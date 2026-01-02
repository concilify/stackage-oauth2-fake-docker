namespace Stackage.OAuth2.Fake.Services;

using System.Collections.Immutable;
using System.Security.Claims;
using System.Text.Json.Nodes;

public interface IClaimsSerializer
{
   bool TryDeserialize(JsonObject claimsObject, out ImmutableArray<Claim> claims);

   JsonObject Serialize(ImmutableArray<Claim> claims);
}
