namespace Stackage.OAuth2.Fake.OutsideIn.Tests;

using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Stackage.OAuth2.Fake.OutsideIn.Tests.Model;

public static class Support
{
   public static async Task<OpenIdConfiguration> GetWellKnownOpenIdConfiguration(this HttpClient httpClient)
   {
      var httpResponse = await httpClient.GetAsync(".well-known/openid-configuration");

      return await httpResponse.ParseWellKnownOpenIdConfiguration();
   }

   public static async Task<OpenIdConfiguration> ParseWellKnownOpenIdConfiguration(this HttpResponseMessage httpResponseMessage)
   {
      var openIdConfiguration = JsonSerializer.Deserialize<OpenIdConfiguration>(await httpResponseMessage.Content.ReadAsStringAsync());

      if (openIdConfiguration == null)
      {
         throw new JsonException("Failed to deserialize the response content.");
      }

      return openIdConfiguration;
   }

   public static async Task<OAuth2DeviceAuthorize> ParseOAuth2DeviceAuthorize(this HttpResponseMessage httpResponseMessage)
   {
      var oAuth2DeviceAuthorize = JsonSerializer.Deserialize<OAuth2DeviceAuthorize>(await httpResponseMessage.Content.ReadAsStringAsync());

      if (oAuth2DeviceAuthorize == null)
      {
         throw new JsonException("Failed to deserialize the response content.");
      }

      return oAuth2DeviceAuthorize;
   }
}
