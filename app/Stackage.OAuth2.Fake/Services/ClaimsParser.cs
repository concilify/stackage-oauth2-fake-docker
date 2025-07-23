namespace Stackage.OAuth2.Fake.Services;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;

public class ClaimsParser : IClaimsParser
{
   public bool TryParse(
      JsonObject claimsObject,
      [MaybeNullWhen(false)] out IList<Claim> claims)
   {
      claims = new List<Claim>();

      foreach (var (key, claimNode) in claimsObject)
      {
         if (claimNode is JsonValue claimValue && claimValue.GetValueKind() == JsonValueKind.String)
         {
            claims.Add(new Claim(key, claimValue.GetValue<string>()));
         }
         else if (claimNode is JsonArray claimArray && TryParseArrayOfStrings(claimArray, out var strings))
         {
            claims.Add(new Claim(key, JsonSerializer.Serialize(strings), JsonClaimValueTypes.JsonArray));
         }
         else
         {
            claims = null;
            return false;
         }
      }

      return true;
   }

   private static bool TryParseArrayOfStrings(
      JsonArray claimArray,
      [MaybeNullWhen(false)] out IList<string> strings)
   {
      strings = new List<string>();

      foreach (var claimArrayItemNode in claimArray)
      {
         if (claimArrayItemNode?.GetValueKind() == JsonValueKind.String)
         {
            strings.Add(claimArrayItemNode.GetValue<string>());
         }
         else
         {
            strings = null;
            return false;
         }
      }

      return true;
   }
}
