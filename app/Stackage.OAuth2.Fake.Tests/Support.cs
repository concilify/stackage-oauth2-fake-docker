namespace Stackage.OAuth2.Fake.Tests;

using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

public static class Support
{
   public static async Task<TResponse> ParseAsync<TResponse>(this HttpResponseMessage httpResponseMessage)
   {
      var response = JsonSerializer.Deserialize<TResponse>(await httpResponseMessage.Content.ReadAsStringAsync());

      if (response == null)
      {
         throw new JsonException("Failed to deserialize the response content.");
      }

      return response;
   }

   public static IDictionary<string, StringValues> ParseClaims(this JwtSecurityToken jwtSecurityToken, params string[] names)
   {
      return jwtSecurityToken.Claims
         .Where(c => names.Contains(c.Type))
         .GroupBy(c => c.Type)
         .ToDictionary(
            c => c.Key,
            claims => new StringValues(claims.Select(c => c.Value).ToArray()));
   }
}
