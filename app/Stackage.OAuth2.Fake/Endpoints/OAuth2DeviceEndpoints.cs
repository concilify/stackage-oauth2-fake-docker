namespace Stackage.OAuth2.Fake.Endpoints;

using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Stackage.OAuth2.Fake.Services;

public static class OAuth2DeviceEndpoints
{
   public static void MapOAuth2DeviceEndpoints(this WebApplication app)
   {
      app.MapPost(
         "/oauth2/device/authorize",
         (
            DeviceCodeCache deviceCodeCache,
            Settings settings
         ) =>
         {
            var (deviceCode, userCode) = deviceCodeCache.Create();

            var content = new AuthorizeResponse(
               DeviceCode: deviceCode,
               UserCode: userCode,
               VerificationEndpoint: $"{settings.IssuerUrl}{settings.DeviceVerificationPath}",
               VerificationCompleteEndpoint: $"{settings.IssuerUrl}{settings.DeviceVerificationPath}?user_code={userCode}",
               ExpiresInSeconds: 600,
               IntervalSeconds: 5);

            return TypedResults.Json(content);
         });
   }

   private record AuthorizeResponse(
      [property: JsonPropertyName("device_code")] string DeviceCode,
      [property: JsonPropertyName("user_code")] string UserCode,
      [property: JsonPropertyName("verification_uri")] string VerificationEndpoint,
      [property: JsonPropertyName("verification_uri_complete")] string VerificationCompleteEndpoint,
      [property: JsonPropertyName("expires_in")] int ExpiresInSeconds,
      [property: JsonPropertyName("interval")] int IntervalSeconds);
}
