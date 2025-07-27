namespace Stackage.OAuth2.Fake.Endpoints;

using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stackage.OAuth2.Fake.Model.Authorization;
using Stackage.OAuth2.Fake.Services;

public static class InternalEndpoints
{
   public static void MapInternalEndpoints(this WebApplication app)
   {
      app.MapPost(
         "/.internal/create-token",
         (
            [FromBody] CreateTokenRequest? request,
            [FromServices] IClaimsParser claimsParser,
            Settings settings,
            ITokenGenerator tokenGenerator
         ) =>
         {
            if (request == null)
            {
               return Error.InvalidRequest("The request body was missing");
            }

            if (request.Claims == null)
            {
               return Error.InvalidRequest("The claims property was missing");
            }

            if (!claimsParser.TryParse(request.Claims, out var claims))
            {
               return Error.InvalidRequest("The claims property must contain string properties or string array properties");
            }

            var authorization = new InternalAuthorization(
               Subject: request.Subject ?? settings.DefaultSubject,
               Claims: claims);

            var response = tokenGenerator.Generate(authorization);

            return TypedResults.Json(response, statusCode: 200);
         });
   }

   private record CreateTokenRequest(
      [property: JsonPropertyName("subject")] string? Subject,
      [property: JsonPropertyName("claims")] JsonObject? Claims)
   {
      public static ValueTask<CreateTokenRequest?> BindAsync(HttpContext context)
      {
         try
         {
            var request = JsonSerializer.Deserialize<CreateTokenRequest>(context.Request.Body);

            return ValueTask.FromResult(request);
         }
         catch (Exception)
         {
            return ValueTask.FromResult<CreateTokenRequest?>(null);
         }
      }
   }
}
