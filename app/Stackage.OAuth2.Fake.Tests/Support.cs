namespace Stackage.OAuth2.Fake.Tests;

using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

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
}
