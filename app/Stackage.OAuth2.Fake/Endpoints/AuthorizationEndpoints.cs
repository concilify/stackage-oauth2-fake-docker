namespace Stackage.OAuth2.Fake.Endpoints;

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Model.Authorization;
using Stackage.OAuth2.Fake.Services;

public static class AuthorizationEndpoints
{
   public static void MapAuthorizationEndpoints(this WebApplication app)
   {
      var settings = app.Services.GetRequiredService<Settings>();

      app.MapGet(
         settings.AuthorizationPath,
         (
            [FromQuery(Name = "response_type")] string? responseType,
            [FromQuery(Name = "client_id")] string? clientId,
            [FromQuery(Name = "state")] string? state,
            [FromQuery(Name = "redirect_uri")] string? redirectUri,
            [FromQuery(Name = "scope")] string? scope,
            AuthorizationCache<UserAuthorization> authorizationCache
         ) =>
         {
            // RFC 6749 Section 4.1.2.1: If the request fails due to a missing, invalid, or mismatching
            // redirection URI, the authorization server SHOULD inform the resource owner of the error
            // and MUST NOT automatically redirect the user-agent to the invalid redirection URI.
            if (string.IsNullOrEmpty(redirectUri))
            {
               return Results.BadRequest(new
               {
                  error = "invalid_request",
                  error_description = "The redirect_uri parameter is required"
               });
            }

            // RFC 6749 Section 3.1: response_type is REQUIRED
            if (string.IsNullOrEmpty(responseType))
            {
               return Results.Redirect(BuildErrorRedirectUri(
                  redirectUri, 
                  "invalid_request", 
                  "The response_type parameter is required", 
                  state));
            }

            // RFC 6749 Section 3.1.1: For authorization code flow, response_type must be "code"
            if (responseType != "code")
            {
               return Results.Redirect(BuildErrorRedirectUri(
                  redirectUri, 
                  "unsupported_response_type", 
                  "The response_type must be code", 
                  state));
            }

            // RFC 6749 Section 3.1: client_id is REQUIRED
            if (string.IsNullOrEmpty(clientId))
            {
               return Results.Redirect(BuildErrorRedirectUri(
                  redirectUri, 
                  "invalid_request", 
                  "The client_id parameter is required", 
                  state));
            }

            var authorization = authorizationCache.Add(() => UserAuthorization.Create((Scope?)scope ?? Scope.Empty));

            // This would normally redirect to an intermediate URL to allow the user to logon, but the code returned here
            // can be used immediately with the /oauth2/token endpoint using grant type authorization_code
            authorization.Authenticate(settings.DefaultSubject);

            return TypedResults.Redirect(BuildSuccessRedirectUri(redirectUri, authorization.Code, state));
         });

      app.MapPost(
         settings.DeviceAuthorizationPath,
         (
            [FromForm(Name = "client_id")] string? clientId,
            [FromForm(Name = "scope")] string? scope,
            AuthorizationCache<DeviceAuthorization> authorizationCache
         ) =>
         {
            // RFC 8628 Section 3.1: client_id is REQUIRED
            if (string.IsNullOrEmpty(clientId))
            {
               return Error.InvalidRequest("The client_id parameter is required");
            }

            var authorization = authorizationCache.Add(() => DeviceAuthorization.Create((Scope?)scope ?? Scope.Empty));

            // This would normally need the user to visit the verification URL to allow the user to logon, but the code returned
            // here can be used immediately with the /oauth2/token endpoint using grant type urn:ietf:params:oauth:grant-type:device_code
            authorization.Authenticate(settings.DefaultSubject);

            var response = new
            {
               device_code = authorization.DeviceCode,
               user_code = authorization.UserCode,
               verification_uri = $"{settings.IssuerUrl}{settings.DeviceVerificationPath}",
               verification_uri_complete = $"{settings.IssuerUrl}{settings.DeviceVerificationPath}?user_code={authorization.UserCode}",
               expires_in = 600,
               interval = 5
            };

            return TypedResults.Json(response);
         })
         .DisableAntiforgery();
   }

   private static string BuildErrorRedirectUri(string redirectUri, string error, string errorDescription, string? state)
   {
      var uri = $"{redirectUri}?error={error}&error_description={Uri.EscapeDataString(errorDescription)}";
      if (!string.IsNullOrEmpty(state))
      {
         uri += $"&state={Uri.EscapeDataString(state)}";
      }
      return uri;
   }

   private static string BuildSuccessRedirectUri(string redirectUri, string code, string? state)
   {
      var uri = $"{redirectUri}?code={code}";
      if (!string.IsNullOrEmpty(state))
      {
         uri += $"&state={Uri.EscapeDataString(state)}";
      }
      return uri;
   }
}
