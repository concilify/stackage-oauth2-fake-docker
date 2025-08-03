namespace Stackage.OAuth2.Fake.Endpoints;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
      app.MapGet("/.internal/health", () => Results.Ok());

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
               Scope: (Scope?)request.Scope ?? Scope.Empty,
               Subject: request.Subject ?? settings.DefaultSubject,
               TokenExpirySeconds: request.TokenExpirySeconds,
               Claims: claims);

            // TODO: Need to be able to create expired token (eg. pass -ve expiry seconds)
            var response = tokenGenerator.Generate(authorization);

            return TypedResults.Json(response, statusCode: 200);
         });

      app.MapPost(
         "/.internal/seed/user-token",
         (
            [FromBody] SeedUserTokenRequest? request,
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
               (Scope?)request.Scope ?? Scope.Empty);

            authorization.Authorize(request.Subject ?? settings.DefaultSubject);

            authorizationCache.Add(authorization);

            return TypedResults.Ok();
         });

      app.MapPost(
         "/.internal/seed/refresh-token",
         (
            [FromBody] SeedRefreshTokenRequest? request,
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
               (Scope?)request.Scope ?? Scope.Empty,
               request.Subject ?? settings.DefaultSubject);

            authorizationCache.Add(authorization);

            return TypedResults.Ok();
         });

      app.MapGet(
         "/.internal/verify/refresh-token",
         (
            [FromQuery(Name = "refresh_token")] string? refreshToken,
            [FromQuery(Name = "subject")] string? subject,
            AuthorizationCache<RefreshAuthorization> authorizationCache
         ) =>
         {
            if (refreshToken == null)
            {
               return Error.InvalidRequest("The refresh_token parameter was missing");
            }

            if (subject == null)
            {
               return Error.InvalidRequest("The subject parameter was missing");
            }

            if (!authorizationCache.TryGet(refreshToken, out var authorization))
            {
               return Error.InvalidRequest("The given refresh_token was not found");
            }

            if (authorization.Subject != subject)
            {
               return TypedResults.Conflict(new { error = "subject_mismatch" });
            }

            return TypedResults.Ok();
         });

      app.MapPost(
         "/.internal/seed/user",
         (
            [FromBody] SeedUserRequest? request,
            IUserStore userStore
         ) =>
         {
            if (request == null)
            {
               return Error.InvalidRequest("The request body was missing");
            }

            if (request.Subject == null)
            {
               return Error.InvalidRequest("The subject property was missing");
            }

            if (request.Claims == null)
            {
               return Error.InvalidRequest("The claims property was missing");
            }

            var user = new User(
               request.Subject,
               [..request.Claims.Select(c => new Claim(c.Key, c.Value))]);

            userStore.Add(user);

            return TypedResults.Ok();
         });

      app.MapGet("/.internal/history/requests", (CapturedRequestCache capturedRequestCache) =>
      {
         var response = capturedRequestCache.GetAll().Reverse();

         return TypedResults.Json(response, statusCode: 200);
      });

      app.MapDelete("/.internal/history", async (CapturedRequestCache capturedRequestCache) =>
      {
         await capturedRequestCache.ClearAsync();

         return TypedResults.Ok();
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
      [property: JsonPropertyName("scope")] string? Scope,
      [property: JsonPropertyName("tokenExpirySeconds")] int? TokenExpirySeconds,
      [property: JsonPropertyName("claims")] JsonObject? Claims)
   {
      public static ValueTask<CreateTokenRequest?> BindAsync(HttpContext context) => BindAsync<CreateTokenRequest>(context);
   }

   private record SeedUserTokenRequest(
      [property: JsonPropertyName("code")] string? Code,
      [property: JsonPropertyName("scope")] string? Scope,
      [property: JsonPropertyName("subject")] string? Subject)
   {
      public static ValueTask<SeedUserTokenRequest?> BindAsync(HttpContext context) => BindAsync<SeedUserTokenRequest>(context);
   }

   private record SeedRefreshTokenRequest(
      [property: JsonPropertyName("refreshToken")] string? RefreshToken,
      [property: JsonPropertyName("scope")] string? Scope,
      [property: JsonPropertyName("subject")] string? Subject)
   {
      public static ValueTask<SeedRefreshTokenRequest?> BindAsync(HttpContext context) => BindAsync<SeedRefreshTokenRequest>(context);
   }

   private record SeedUserRequest(
      [property: JsonPropertyName("subject")] string? Subject,
      [property: JsonPropertyName("claims")] IDictionary<string, string>? Claims)
   {
      public static ValueTask<SeedUserRequest?> BindAsync(HttpContext context) => BindAsync<SeedUserRequest>(context);
   }
}
