namespace Stackage.OAuth2.Fake.Endpoints;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stackage.OAuth2.Fake.Services;

public static class InternalEndpoints
{
   private const int TokenExpirySecs = 20 * 60;

   public static void MapInternalEndpoints(this WebApplication app)
   {
      app.MapPost(
         "/.internal/create-token",
         (
            [FromBody] CreateTokenRequest request,
            [FromServices] IClaimsParser claimsParser,
            Settings settings,
            ITokenGenerator tokenGenerator
         ) =>
         {
            if (!claimsParser.TryParse(request.Claims, out var claims))
            {
               // TODO:
               throw new Exception();
            }

            if (claims.All(c => c.Type != JwtRegisteredClaimNames.Sub))
            {
               claims.Insert(0, new Claim(JwtRegisteredClaimNames.Sub, settings.DefaultSubject));
            }

            var response = new
            {
               access_token = tokenGenerator.Generate(claims, TokenExpirySecs),
               token_type = "Bearer",
               expires_in = TokenExpirySecs,
            };

            return TypedResults.Json(response, statusCode: 200);
         });
   }

   private record CreateTokenRequest(
      [property: JsonPropertyName("claims"), JsonRequired] JsonObject Claims);
}
