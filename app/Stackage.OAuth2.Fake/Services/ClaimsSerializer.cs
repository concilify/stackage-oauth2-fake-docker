namespace Stackage.OAuth2.Fake.Services;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;

public class ClaimsSerializer : IClaimsSerializer
{
   private readonly ILogger<ClaimsSerializer> _logger;

   public ClaimsSerializer(ILogger<ClaimsSerializer> logger)
   {
      _logger = logger;
   }

   public bool TryDeserialize(JsonObject claimsObject, out ImmutableArray<Claim> claims)
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
            _logger.LogWarning("Unexpected claim type {ClaimType}.", claimNode?.GetValueKind().ToString());

            claims = [];
            return false;
         }
      }

      claims = results.ToImmutable();
      return true;
   }

   public JsonObject Serialize(ImmutableArray<Claim> claims)
   {
      var claimsObject = new JsonObject();

      foreach (var claim in claims)
      {
         if (claim.ValueType == JsonClaimValueTypes.JsonArray)
         {
            claimsObject[claim.Type] = JsonNode.Parse(claim.Value);
         }
         else if (claim.ValueType == ClaimValueTypes.String)
         {
            claimsObject[claim.Type] = claim.Value;
         }
      }

      return claimsObject;
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
