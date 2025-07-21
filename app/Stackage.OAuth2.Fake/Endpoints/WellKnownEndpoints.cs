namespace Stackage.OAuth2.Fake.Endpoints;

using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

public static class WellKnownEndpoints
{
   public static void MapWellKnownEndpoints(this WebApplication app)
   {
      app.MapGet(
         "/.well-known/openid-configuration",
         (Settings settings) =>
         {
            var content = new OpenIdConfigurationResponse(
               settings.IssuerUrl,
               TokenEndpoint: $"{settings.IssuerUrl}{settings.TokenPath}",
               DeviceAuthorizationEndpoint: $"{settings.IssuerUrl}{settings.DeviceAuthorizationPath}",
               GrantTypesSupported: GrantTypes.Supported);

            return TypedResults.Json(content);
         });
   }

   private record OpenIdConfigurationResponse(
      [property: JsonPropertyName("issuer")] string IssuerUrl,
      [property: JsonPropertyName("token_endpoint")] string TokenEndpoint,
      [property: JsonPropertyName("device_authorization_endpoint")] string DeviceAuthorizationEndpoint,
      [property: JsonPropertyName("grant_types_supported")] string[] GrantTypesSupported);
}
