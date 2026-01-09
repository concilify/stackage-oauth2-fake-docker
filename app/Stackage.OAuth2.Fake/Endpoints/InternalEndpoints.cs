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
using Microsoft.AspNetCore.Http.HttpResults;
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
         async Task<Results<JsonHttpResult<TokenResponse>, JsonHttpResult<ErrorResponse>, BadRequest<ErrorResponse>>> (
            HttpContext httpContext,
            [FromServices] IClaimsSerializer claimsSerializer,
            ITokenGenerator tokenGenerator) =>
         {
            return await HandleJsonRequestAsync<CreateTokenRequest, JsonHttpResult<TokenResponse>>(httpContext, request =>
            {
               if (request.ClientId == null)
               {
                  return OAuth2Results.InvalidRequestBadRequest("The clientId property was missing");
               }

               if (request.Subject == null)
               {
                  return OAuth2Results.InvalidRequestBadRequest("The subject property was missing");
               }

               if (request.Claims == null)
               {
                  return OAuth2Results.InvalidRequestBadRequest("The claims property was missing");
               }

               if (!claimsSerializer.TryDeserialize(request.Claims, out var claims))
               {
                  return OAuth2Results.InvalidRequestBadRequest("The claims property must contain string properties or string array properties");
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
         });
   }

   private static void MapInternalAuthorizationEndpoints(this WebApplication app)
   {
      app.MapPost(
         "/.internal/user-authorization",
         async Task<Results<Ok, JsonHttpResult<ErrorResponse>, BadRequest<ErrorResponse>>> (
            HttpContext httpContext,
            Settings settings,
            AuthorizationCache<UserAuthorization> authorizationCache) =>
         {
            return await HandleJsonRequestAsync<PostUserAuthorizationRequest, Ok>(httpContext, request =>
            {
               if (request.Code == null)
               {
                  return OAuth2Results.InvalidRequestBadRequest("The code property was missing");
               }

               if (request.ClientId == null)
               {
                  return OAuth2Results.InvalidRequestBadRequest("The clientId property was missing");
               }

               var authorization = new UserAuthorization(
                  request.Code,
                  request.ClientId,
                  (Scope?)request.Scopes ?? Scope.Empty,
                  request.Audiences);

               authorization.Authenticate(request.Subject ?? settings.DefaultSubject);

               return authorizationCache.TryAdd(authorization)
                  ? TypedResults.Ok()
                  : OAuth2Results.InvalidRequestBadRequest("The given code already exists");
            });
         });

      app.MapPost(
         "/.internal/device-authorization",
         async Task<Results<Ok, JsonHttpResult<ErrorResponse>, BadRequest<ErrorResponse>>> (
            HttpContext httpContext,
            Settings settings,
            AuthorizationCache<DeviceAuthorization> authorizationCache) =>
         {
            return await HandleJsonRequestAsync<PostDeviceAuthorizationRequest, Ok>(httpContext, request =>
            {
               if (request.DeviceCode == null)
               {
                  return OAuth2Results.InvalidRequestBadRequest("The deviceCode property was missing");
               }

               if (request.UserCode == null)
               {
                  return OAuth2Results.InvalidRequestBadRequest("The userCode property was missing");
               }

               if (request.ClientId == null)
               {
                  return OAuth2Results.InvalidRequestBadRequest("The clientId property was missing");
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
                  : OAuth2Results.InvalidRequestBadRequest("The given deviceCode already exists");
            });
         });

      app.MapGet(
         "/.internal/user-authorization",
         Results<JsonHttpResult<GetUserAuthorizationResponse>, BadRequest<ErrorResponse>> (
            [FromQuery(Name = "code")] string? code,
            AuthorizationCache<UserAuthorization> authorizationCache) =>
         {
            if (code == null)
            {
               return OAuth2Results.InvalidRequestBadRequest("The code parameter was missing");
            }

            if (!authorizationCache.TryGet(code, out var authorization))
            {
               return OAuth2Results.InvalidRequestBadRequest("The given code was not found");
            }

            var response = new GetUserAuthorizationResponse(
               Code: authorization.Code,
               ClientId: authorization.ClientId,
               Scopes: authorization.Scope.ToArray(),
               Subject: authorization.Subject,
               Audiences: authorization.Audiences);

            return TypedResults.Json(response, statusCode: 200);
         });

      app.MapGet(
         "/.internal/device-authorization",
         Results<JsonHttpResult<GetDeviceAuthorizationResponse>, BadRequest<ErrorResponse>> (
            [FromQuery(Name = "deviceCode")] string? deviceCode,
            AuthorizationCache<DeviceAuthorization> authorizationCache) =>
         {
            if (deviceCode == null)
            {
               return OAuth2Results.InvalidRequestBadRequest("The deviceCode parameter was missing");
            }

            if (!authorizationCache.TryGet(deviceCode, out var authorization))
            {
               return OAuth2Results.InvalidRequestBadRequest("The given deviceCode was not found");
            }

            var response = new GetDeviceAuthorizationResponse(
               DeviceCode: authorization.DeviceCode,
               UserCode: authorization.UserCode,
               ClientId: authorization.ClientId,
               Scopes: authorization.Scope.ToArray(),
               Subject: authorization.Subject,
               Audiences: authorization.Audiences);

            return TypedResults.Json(response, statusCode: 200);
         });
   }

   private static void MapInternalRefreshTokenEndpoints(this WebApplication app)
   {
      app.MapPost(
         "/.internal/refresh-token",
         async Task<Results<Ok, JsonHttpResult<ErrorResponse>, BadRequest<ErrorResponse>>> (
            HttpContext httpContext,
            Settings settings,
            AuthorizationCache<RefreshAuthorization> authorizationCache) =>
         {
            return await HandleJsonRequestAsync<PostRefreshTokenRequest, Ok>(httpContext, request =>
            {
               if (request.RefreshToken == null)
               {
                  return OAuth2Results.InvalidRequestBadRequest("The refreshToken property was missing");
               }

               if (request.ClientId == null)
               {
                  return OAuth2Results.InvalidRequestBadRequest("The clientId property was missing");
               }

               var authorization = new RefreshAuthorization(
                  request.RefreshToken,
                  request.ClientId,
                  (Scope?)request.Scopes ?? Scope.Empty,
                  request.Subject ?? settings.DefaultSubject);

               return authorizationCache.TryAdd(authorization)
                  ? TypedResults.Ok()
                  : OAuth2Results.InvalidRequestBadRequest("The given refreshToken already exists");
            });
         });

      app.MapGet(
         "/.internal/refresh-token",
         Results<JsonHttpResult<PostRefreshTokenResponse>, BadRequest<ErrorResponse>> (
            [FromQuery(Name = "refresh_token")] string? refreshToken,
            AuthorizationCache<RefreshAuthorization> authorizationCache) =>
         {
            if (refreshToken == null)
            {
               return OAuth2Results.InvalidRequestBadRequest("The refresh_token parameter was missing");
            }

            if (!authorizationCache.TryGet(refreshToken, out var authorization))
            {
               return OAuth2Results.InvalidRequestBadRequest("The given refresh_token was not found");
            }

            var response = new PostRefreshTokenResponse(
               RefreshToken: authorization.RefreshToken,
               ClientId: authorization.ClientId,
               Scopes: authorization.Scope.ToArray(),
               Subject: authorization.Subject);

            return TypedResults.Json(response, statusCode: 200);
         });
   }

   private static void MapInternalUsersEndpoints(this WebApplication app)
   {
      app.MapPost(
         "/.internal/users",
         async Task<Results<Ok, JsonHttpResult<ErrorResponse>, BadRequest<ErrorResponse>>> (
            HttpContext httpContext,
            [FromServices] IClaimsSerializer claimsSerializer,
            IUserStore userStore) =>
         {
            return await HandleJsonRequestAsync<PostUserRequest, Ok>(httpContext, request =>
            {
               if (request.Subject == null)
               {
                  return OAuth2Results.InvalidRequestBadRequest("The subject property was missing");
               }

               if (request.Claims == null)
               {
                  return OAuth2Results.InvalidRequestBadRequest("The claims property was missing");
               }

               if (!claimsSerializer.TryDeserialize(request.Claims, out var claims))
               {
                  return OAuth2Results.InvalidRequestBadRequest("The claims property must contain string properties or string array properties");
               }

               var user = new User(request.Subject, claims);

               return userStore.TryAdd(user)
                  ? TypedResults.Ok()
                  : OAuth2Results.InvalidRequestBadRequest("The given subject already exists");
            });
         });

      app.MapGet(
         "/.internal/users",
         Results<JsonHttpResult<IReadOnlyList<GetUserResponse.User>>, BadRequest<ErrorResponse>> (
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
                  return OAuth2Results.InvalidRequestBadRequest("The given subject was not found");
               }

               users = [user];
            }

            var response = users
               .Select(u => new GetUserResponse.User(
                  Subject: u.Subject,
                  Claims: u.Claims.ToDictionary(c => c.Type, c => c.Value)))
               .ToList();

            return TypedResults.Json<IReadOnlyList<GetUserResponse.User>>(response, statusCode: 200);
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

   private static async Task<Results<TResponse, JsonHttpResult<ErrorResponse>, BadRequest<ErrorResponse>>> HandleJsonRequestAsync<TRequest, TResponse>(
      HttpContext httpContext,
      Func<TRequest, Results<TResponse, JsonHttpResult<ErrorResponse>, BadRequest<ErrorResponse>>> handler)
      where TResponse : IResult
   {
      if (!httpContext.Request.HasJsonContentType())
      {
         return OAuth2Results.InvalidRequestUnsupportedMediaType("The Content-Type header must be application/json");
      }

      if (httpContext.Request.ContentLength == 0)
      {
         return OAuth2Results.InvalidRequestBadRequest("The request body was missing");
      }

      TRequest? request;

      try
      {
         request = await httpContext.Request.ReadFromJsonAsync<TRequest>();
      }
      catch (JsonException)
      {
         return OAuth2Results.InvalidRequestBadRequest("The request body contained malformed JSON");
      }

      if (request == null)
      {
         // This shouldn't be possible
         throw new JsonException("Failed to deserialize the request body.");
      }

      return handler(request);
   }

   private record CreateTokenRequest(
      [property: JsonPropertyName("clientId")] string? ClientId,
      [property: JsonPropertyName("scopes")] string[]? Scopes,
      [property: JsonPropertyName("subject")] string? Subject,
      [property: JsonPropertyName("audiences")] string[]? Audiences,
      [property: JsonPropertyName("tokenExpirySeconds")] int? TokenExpirySeconds,
      [property: JsonPropertyName("claims")] JsonObject? Claims);

   private record PostUserAuthorizationRequest(
      [property: JsonPropertyName("code")] string? Code,
      [property: JsonPropertyName("clientId")] string? ClientId,
      [property: JsonPropertyName("scopes")] string[]? Scopes,
      [property: JsonPropertyName("subject")] string? Subject,
      [property: JsonPropertyName("audiences")] string[]? Audiences);

   private record GetUserAuthorizationResponse(
      [property: JsonPropertyName("code")] string Code,
      [property: JsonPropertyName("clientId")] string ClientId,
      [property: JsonPropertyName("scopes")] string[] Scopes,
      [property: JsonPropertyName("subject")] string? Subject,
      [property: JsonPropertyName("audiences")] string[]? Audiences);

   private record PostDeviceAuthorizationRequest(
      [property: JsonPropertyName("deviceCode")] string? DeviceCode,
      [property: JsonPropertyName("userCode")] string? UserCode,
      [property: JsonPropertyName("clientId")] string? ClientId,
      [property: JsonPropertyName("scopes")] string[]? Scopes,
      [property: JsonPropertyName("subject")] string? Subject,
      [property: JsonPropertyName("audiences")] string[]? Audiences);

   private record GetDeviceAuthorizationResponse(
      [property: JsonPropertyName("deviceCode")] string DeviceCode,
      [property: JsonPropertyName("userCode")] string UserCode,
      [property: JsonPropertyName("clientId")] string ClientId,
      [property: JsonPropertyName("scopes")] string[] Scopes,
      [property: JsonPropertyName("subject")] string? Subject,
      [property: JsonPropertyName("audiences")] string[]? Audiences);

   private record PostRefreshTokenRequest(
      [property: JsonPropertyName("refreshToken")] string? RefreshToken,
      [property: JsonPropertyName("clientId")] string? ClientId,
      [property: JsonPropertyName("scopes")] string[]? Scopes,
      [property: JsonPropertyName("subject")] string? Subject);

   private record PostRefreshTokenResponse(
      [property: JsonPropertyName("refresh_token")] string RefreshToken,
      [property: JsonPropertyName("clientId")] string ClientId,
      [property: JsonPropertyName("scopes")] string[] Scopes,
      [property: JsonPropertyName("subject")] string? Subject);

   private record PostUserRequest(
      [property: JsonPropertyName("subject")] string? Subject,
      [property: JsonPropertyName("claims")] JsonObject? Claims);

   private class GetUserResponse
   {
      public record User(string Subject, IDictionary<string, string> Claims);
   }
}
