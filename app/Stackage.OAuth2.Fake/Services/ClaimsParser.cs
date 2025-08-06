namespace Stackage.OAuth2.Fake.Services;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;

public class ClaimsParser : IClaimsParser
{
   public bool TryParse(
      JsonObject claimsObject,
      out ImmutableArray<Claim> claims)
   {
      var results = ImmutableArray.CreateBuilder<Claim>();

      foreach (var (key, claimNode) in claimsObject)
      {
         if (claimNode is JsonValue claimValue && claimValue.GetValueKind() == JsonValueKind.String)
         {
            results.Add(new Claim(NormalizeName(key), claimValue.GetValue<string>()));
         }
         else if (claimNode is JsonArray claimArray && TryParseArrayOfStrings(claimArray, out var strings))
         {
            results.Add(new Claim(NormalizeName(key), JsonSerializer.Serialize(strings), JsonClaimValueTypes.JsonArray));
         }
         else
         {
            claims = ImmutableArray<Claim>.Empty;
            return false;
         }
      }

      claims = results.ToImmutable();
      return true;
   }

   private static bool TryParseArrayOfStrings(
      JsonArray claimArray,
      [MaybeNullWhen(false)] out IList<string> strings)
   {
      var results = new List<string>();

      foreach (var claimArrayItemNode in claimArray)
      {
         if (claimArrayItemNode?.GetValueKind() == JsonValueKind.String)
         {
            results.Add(claimArrayItemNode.GetValue<string>());
         }
         else
         {
            strings = null;
            return false;
         }
      }

      strings = results;
      return true;
   }

   private static string NormalizeName(string name)
   {
      string[] knownClaims = ["name", "nickname", "picture"];

      foreach (string knownClaim in knownClaims)
      {
         if (string.Equals(name, knownClaim, StringComparison.InvariantCultureIgnoreCase))
         {
            return knownClaim;
         }
      }

      return name;
   }
}
