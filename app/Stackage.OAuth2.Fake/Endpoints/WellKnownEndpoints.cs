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
         (Configuration configuration) =>
         {
            var response = new ConfigurationResponse(configuration.IssuerUrl);

            return TypedResults.Json(response);
         });
   }

   private record ConfigurationResponse(
      [property:JsonPropertyName("issuer")]string IssuerUrl);
}
