namespace Stackage.OAuth2.Fake.Endpoints;

using System;
using System.Collections.Generic;
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
      app.MapGet("/.internal/health", () => TypedResults.Ok());

      app.MapInternalCreateTokenEndpoint();
      app.MapInternalAuthorizationEndpoints();
      app.MapInternalRefreshTokenEndpoints();
      app.MapInternalUsersEndpoints();
      app.MapInternalHistoryEndpoints();
   }

   private static void MapInternalCreateTokenEndpoint(this WebApplication app)
   {
      app.MapPost(
         "/.internal/create-token",
         (
            [FromBody] CreateTokenRequest? request,
            [FromServices] IClaimsParser claimsParser,
            ITokenGenerator tokenGenerator) =>
         {
            if (request == null)
            {
               return OAuth2Results.InvalidRequest("The request body was missing");
            }

            if (request.ClientId == null)
            {
               return OAuth2Results.InvalidRequest("The clientId property was missing");
            }

            if (request.Subject == null)
            {
               return OAuth2Results.InvalidRequest("The subject property was missing");
            }

            if (request.Claims == null)
            {
               return OAuth2Results.InvalidRequest("The claims property was missing");
            }

            if (!claimsParser.TryParse(request.Claims, out var claims))
            {
               return OAuth2Results.InvalidRequest("The claims property must contain string properties or string array properties");
            }

            var authorization = new InternalAuthorization(
               ClientId: request.ClientId,
               Scope: (Scope?)request.Scopes ?? Scope.Empty,
               request.Audiences,
               Subject: request.Subject,
               TokenExpirySeconds: request.TokenExpirySeconds,
               Claims: claims);

            var response = tokenGenerator.Generate(authorization);

            return TypedResults.Json(response, statusCode: 200);
         });
   }

   private static void MapInternalAuthorizationEndpoints(this WebApplication app)
   {
      app.MapPost(
         "/.internal/user-authorization",
         (
            [FromBody] PostUserAuthorizationRequest? request,
            Settings settings,
            AuthorizationCache<UserAuthorization> authorizationCache) =>
         {
            if (request == null)
            {
               return OAuth2Results.InvalidRequest("The request body was missing");
            }

            if (request.Code == null)
            {
               return OAuth2Results.InvalidRequest("The code property was missing");
            }

            if (request.ClientId == null)
            {
               return OAuth2Results.InvalidRequest("The clientId property was missing");
            }

            var authorization = new UserAuthorization(
               request.Code,
               request.ClientId,
               (Scope?)request.Scopes ?? Scope.Empty,
               request.Audiences);

            authorization.Authenticate(request.Subject ?? settings.DefaultSubject);

            return authorizationCache.TryAdd(authorization)
               ? TypedResults.Ok()
               : OAuth2Results.InvalidRequest("The given code already exists");
         });

      app.MapPost(
         "/.internal/device-authorization",
         (
            [FromBody] PostDeviceAuthorizationRequest? request,
            Settings settings,
            AuthorizationCache<DeviceAuthorization> authorizationCache) =>
         {
            if (request == null)
            {
               return OAuth2Results.InvalidRequest("The request body was missing");
            }

            if (request.DeviceCode == null)
            {
               return OAuth2Results.InvalidRequest("The deviceCode property was missing");
            }

            if (request.UserCode == null)
            {
               return OAuth2Results.InvalidRequest("The userCode property was missing");
            }

            if (request.ClientId == null)
            {
               return OAuth2Results.InvalidRequest("The clientId property was missing");
            }

            var authorization = new DeviceAuthorization(
               request.DeviceCode,
               request.UserCode,
               request.ClientId,
               (Scope?)request.Scopes ?? Scope.Empty,
               request.Audiences);

            authorization.Authenticate(request.Subject ?? settings.DefaultSubject);

            return authorizationCache.TryAdd(authorization)
               ? TypedResults.Ok()
               : OAuth2Results.InvalidRequest("The given deviceCode already exists");
         });

      app.MapGet(
         "/.internal/user-authorization",
         (
            [FromQuery(Name = "code")] string? code,
            AuthorizationCache<UserAuthorization> authorizationCache) =>
         {
            if (code == null)
            {
               return OAuth2Results.InvalidRequest("The code parameter was missing");
            }

            if (!authorizationCache.TryGet(code, out var authorization))
            {
               return OAuth2Results.InvalidRequest("The given code was not found");
            }

            var response = new
            {
               code = authorization.Code,
               clientId = authorization.ClientId,
               scopes = authorization.Scope.ToArray(),
               subject = authorization.Subject,
               audiences = authorization.Audiences,
            };

            return TypedResults.Json(response, statusCode: 200);
         });

      app.MapGet(
         "/.internal/device-authorization",
         (
            [FromQuery(Name = "deviceCode")] string? deviceCode,
            AuthorizationCache<DeviceAuthorization> authorizationCache) =>
         {
            if (deviceCode == null)
            {
               return OAuth2Results.InvalidRequest("The deviceCode parameter was missing");
            }

            if (!authorizationCache.TryGet(deviceCode, out var authorization))
            {
               return OAuth2Results.InvalidRequest("The given deviceCode was not found");
            }

            var response = new
            {
               deviceCode = authorization.DeviceCode,
               userCode = authorization.UserCode,
               clientId = authorization.ClientId,
               scopes = authorization.Scope.ToArray(),
               subject = authorization.Subject,
               audiences = authorization.Audiences,
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
            AuthorizationCache<RefreshAuthorization> authorizationCache) =>
         {
            if (request == null)
            {
               return OAuth2Results.InvalidRequest("The request body was missing");
            }

            if (request.RefreshToken == null)
            {
               return OAuth2Results.InvalidRequest("The refreshToken property was missing");
            }

            if (request.ClientId == null)
            {
               return OAuth2Results.InvalidRequest("The clientId property was missing");
            }

            var authorization = new RefreshAuthorization(
               request.RefreshToken,
               request.ClientId,
               (Scope?)request.Scopes ?? Scope.Empty,
               request.Subject ?? settings.DefaultSubject);

            return authorizationCache.TryAdd(authorization)
               ? TypedResults.Ok()
               : OAuth2Results.InvalidRequest("The given refreshToken already exists");
         });

      app.MapGet(
         "/.internal/refresh-token",
         (
            [FromQuery(Name = "refresh_token")] string? refreshToken,
            AuthorizationCache<RefreshAuthorization> authorizationCache) =>
         {
            if (refreshToken == null)
            {
               return OAuth2Results.InvalidRequest("The refresh_token parameter was missing");
            }

            if (!authorizationCache.TryGet(refreshToken, out var authorization))
            {
               return OAuth2Results.InvalidRequest("The given refresh_token was not found");
            }

            var response = new
            {
               refresh_token = authorization.RefreshToken,
               clientId = authorization.ClientId,
               scopes = authorization.Scope.ToArray(),
               subject = authorization.Subject,
            };

            return TypedResults.Json(response, statusCode: 200);
         });
   }

   private static void MapInternalUsersEndpoints(this WebApplication app)
   {
      app.MapPost(
         "/.internal/users",
         (
            [FromBody] PostUserRequest? request,
            [FromServices] IClaimsParser claimsParser,
            IUserStore userStore) =>
         {
            if (request == null)
            {
               return OAuth2Results.InvalidRequest("The request body was missing");
            }

            if (request.Subject == null)
            {
               return OAuth2Results.InvalidRequest("The subject property was missing");
            }

            if (request.Claims == null)
            {
               return OAuth2Results.InvalidRequest("The claims property was missing");
            }

            if (!claimsParser.TryParse(request.Claims, out var claims))
            {
               return OAuth2Results.InvalidRequest("The claims property must contain string properties or string array properties");
            }

            var user = new User(request.Subject, claims);

            return userStore.TryAdd(user)
               ? TypedResults.Ok()
               : OAuth2Results.InvalidRequest("The given subject already exists");
         });

      app.MapGet(
         "/.internal/users",
         (
            [FromQuery(Name = "subject")] string? subject,
            IUserStore userStore) =>
         {
            IReadOnlyList<User> users;

            if (subject == null)
            {
               users = userStore.GetAll();
            }
            else
            {
               if (!userStore.TryGet(subject, out var user))
               {
                  return OAuth2Results.InvalidRequest("The given subject was not found");
               }

               users = [user];
            }

            var response = users
               .Select(u => new
               {
                  subject = u.Subject,
                  claims = u.Claims.ToDictionary(c => c.Type, c => c.Value),
               })
               .ToArray();

            return TypedResults.Json(response, statusCode: 200);
         });
   }

   private static void MapInternalHistoryEndpoints(this WebApplication app)
   {
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
      [property: JsonPropertyName("clientId")] string? ClientId,
      [property: JsonPropertyName("scopes")] string[]? Scopes,
      [property: JsonPropertyName("subject")] string? Subject,
      [property: JsonPropertyName("audiences")] string[]? Audiences,
      [property: JsonPropertyName("tokenExpirySeconds")] int? TokenExpirySeconds,
      [property: JsonPropertyName("claims")] JsonObject? Claims)
   {
      // ReSharper disable once UnusedMember.Local
      public static ValueTask<CreateTokenRequest?> BindAsync(HttpContext context) => BindAsync<CreateTokenRequest>(context);
   }

   private record PostUserAuthorizationRequest(
      [property: JsonPropertyName("code")] string? Code,
      [property: JsonPropertyName("clientId")] string? ClientId,
      [property: JsonPropertyName("scopes")] string[]? Scopes,
      [property: JsonPropertyName("subject")] string? Subject,
      [property: JsonPropertyName("audiences")] string[]? Audiences)
   {
      // ReSharper disable once UnusedMember.Local
      public static ValueTask<PostUserAuthorizationRequest?> BindAsync(HttpContext context) => BindAsync<PostUserAuthorizationRequest>(context);
   }

   private record PostDeviceAuthorizationRequest(
      [property: JsonPropertyName("deviceCode")] string? DeviceCode,
      [property: JsonPropertyName("userCode")] string? UserCode,
      [property: JsonPropertyName("clientId")] string? ClientId,
      [property: JsonPropertyName("scopes")] string[]? Scopes,
      [property: JsonPropertyName("subject")] string? Subject,
      [property: JsonPropertyName("audiences")] string[]? Audiences)
   {
      // ReSharper disable once UnusedMember.Local
      public static ValueTask<PostDeviceAuthorizationRequest?> BindAsync(HttpContext context) => BindAsync<PostDeviceAuthorizationRequest>(context);
   }

   private record PostRefreshTokenRequest(
      [property: JsonPropertyName("refreshToken")] string? RefreshToken,
      [property: JsonPropertyName("clientId")] string? ClientId,
      [property: JsonPropertyName("scopes")] string[]? Scopes,
      [property: JsonPropertyName("subject")] string? Subject)
   {
      // ReSharper disable once UnusedMember.Local
      public static ValueTask<PostRefreshTokenRequest?> BindAsync(HttpContext context) => BindAsync<PostRefreshTokenRequest>(context);
   }

   private record PostUserRequest(
      [property: JsonPropertyName("subject")] string? Subject,
      [property: JsonPropertyName("claims")] JsonObject? Claims)
   {
      // ReSharper disable once UnusedMember.Local
      public static ValueTask<PostUserRequest?> BindAsync(HttpContext context) => BindAsync<PostUserRequest>(context);
   }
}
