namespace Stackage.OAuth2.Fake.OutsideIn.Tests;

using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Stackage.OAuth2.Fake.OutsideIn.Tests.Model;

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

   public static async Task<OpenIdConfigurationResponse> GetWellKnownOpenIdConfigurationAsync(this HttpClient httpClient)
   {
      var httpResponse = await httpClient.GetAsync(".well-known/openid-configuration");

      return await httpResponse.ParseAsync<OpenIdConfigurationResponse>();
   }
}
