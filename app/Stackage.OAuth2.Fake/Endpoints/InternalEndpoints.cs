namespace Stackage.OAuth2.Fake.Endpoints;

using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Model.Authorization;
using Stackage.OAuth2.Fake.Services;

public static class InternalEndpoints
{
   public static void MapInternalEndpoints(this WebApplication app)
   {
      app.MapInternalCreateTokenEndpoint();
      app.MapInternalAuthorizationEndpoints();
      app.MapInternalRefreshTokenEndpoints();
   }

   private static void MapInternalCreateTokenEndpoint(this WebApplication app)
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
               Scope: (Scope?)request.Scopes ?? Scope.Empty,
               Subject: request.Subject ?? settings.DefaultSubject,
               TokenExpirySeconds: request.TokenExpirySeconds,
               Claims: claims);

            var response = tokenGenerator.Generate(authorization);

            return TypedResults.Json(response, statusCode: 200);
         });
   }

   private static void MapInternalAuthorizationEndpoints(this WebApplication app)
   {
      app.MapPost(
         "/.internal/authorization",
         (
            [FromBody] PostAuthorizationRequest? request,
            Settings settings,
            AuthorizationCache<UserAuthorization> authorizationCache
         ) =>
         {
            if (request == null)
            {
               return Error.InvalidRequest("The request body was missing");
            }

            if (request.Code == null)
            {
               return Error.InvalidRequest("The code property was missing");
            }

            var authorization = new UserAuthorization(
               request.Code,
               (Scope?)request.Scopes ?? Scope.Empty);

            authorization.Authenticate(request.Subject ?? settings.DefaultSubject);

            authorizationCache.Add(authorization);

            return TypedResults.Ok();
         });

      app.MapGet(
         "/.internal/authorization",
         (
            [FromQuery(Name = "code")] string? code,
            AuthorizationCache<UserAuthorization> authorizationCache
         ) =>
         {
            if (code == null)
            {
               return Error.InvalidRequest("The code parameter was missing");
            }

            if (!authorizationCache.TryGet(code, out var authorization))
            {
               return Error.InvalidRequest("The given code was not found");
            }

            var response = new
            {
               code = authorization.Code,
               scopes = authorization.Scope.ToArray(),
               subject = authorization.Subject
            };

            return TypedResults.Json(response, statusCode: 200);
         });
   }

   private static void MapInternalRefreshTokenEndpoints(this WebApplication app)
   {
      app.MapPost(
         "/.internal/refresh-token",
         (
            [FromBody] PostRefreshTokenRequest? request,
            Settings settings,
            AuthorizationCache<RefreshAuthorization> authorizationCache
         ) =>
         {
            if (request == null)
            {
               return Error.InvalidRequest("The request body was missing");
            }

            if (request.RefreshToken == null)
            {
               return Error.InvalidRequest("The refreshToken property was missing");
            }

            var authorization = new RefreshAuthorization(
               request.RefreshToken,
               (Scope?)request.Scopes ?? Scope.Empty,
               request.Subject ?? settings.DefaultSubject);

            authorizationCache.Add(authorization);

            return TypedResults.Ok();
         });

      app.MapGet(
         "/.internal/refresh-token",
         (
            [FromQuery(Name = "refresh_token")] string? refreshToken,
            AuthorizationCache<RefreshAuthorization> authorizationCache
         ) =>
         {
            if (refreshToken == null)
            {
               return Error.InvalidRequest("The refresh_token parameter was missing");
            }

            if (!authorizationCache.TryGet(refreshToken, out var authorization))
            {
               return Error.InvalidRequest("The given refresh_token was not found");
            }

            var response = new
            {
               refresh_token = authorization.RefreshToken,
               scopes = authorization.Scope.ToArray(),
               subject = authorization.Subject
            };

            return TypedResults.Json(response, statusCode: 200);
         });
   }

   private static ValueTask<T?> BindAsync<T>(HttpContext context)
      where T : class
   {
      try
      {
         var request = JsonSerializer.Deserialize<T>(context.Request.Body);

         return ValueTask.FromResult(request);
      }
      catch (Exception)
      {
         return ValueTask.FromResult<T?>(null);
      }
   }

   private record CreateTokenRequest(
      [property: JsonPropertyName("subject")] string? Subject,
      [property: JsonPropertyName("scopes")] string[]? Scopes,
      [property: JsonPropertyName("tokenExpirySeconds")] int? TokenExpirySeconds,
      [property: JsonPropertyName("claims")] JsonObject? Claims)
   {
      public static ValueTask<CreateTokenRequest?> BindAsync(HttpContext context) => BindAsync<CreateTokenRequest>(context);
   }

   private record PostAuthorizationRequest(
      [property: JsonPropertyName("code")] string? Code,
      [property: JsonPropertyName("scopes")] string[]? Scopes,
      [property: JsonPropertyName("subject")] string? Subject)
   {
      public static ValueTask<PostAuthorizationRequest?> BindAsync(HttpContext context) => BindAsync<PostAuthorizationRequest>(context);
   }

   private record PostRefreshTokenRequest(
      [property: JsonPropertyName("refreshToken")] string? RefreshToken,
      [property: JsonPropertyName("scopes")] string[]? Scopes,
      [property: JsonPropertyName("subject")] string? Subject)
   {
      public static ValueTask<PostRefreshTokenRequest?> BindAsync(HttpContext context) => BindAsync<PostRefreshTokenRequest>(context);
   }
}
