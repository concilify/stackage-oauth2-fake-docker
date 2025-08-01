namespace Stackage.OAuth2.Fake.Endpoints;

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
            [FromQuery(Name = "state")] string state,
            [FromQuery(Name = "redirect_uri")] string redirectUri,
            [FromQuery(Name = "scope")] string? scope,
            AuthorizationCache<UserAuthorization> authorizationCache
         ) =>
         {
            var authorization = authorizationCache.Add(() => UserAuthorization.Create((Scope?)scope ?? Scope.Empty));

            // This would normally redirect to an intermediate URL to allow the user to logon, but the code returned here
            // can be used immediately with the /oauth2/token endpoint using grant type authorization_code
            authorization.Authorize(settings.DefaultSubject);

            return TypedResults.Redirect($"{redirectUri}?code={authorization.Code}&state={state}");
         });

      app.MapPost(
         settings.DeviceAuthorizationPath,
         (
            [FromForm(Name = "scope")] string? scope,
            AuthorizationCache<DeviceAuthorization> authorizationCache
         ) =>
         {
            var authorization = authorizationCache.Add(() => DeviceAuthorization.Create((Scope?)scope ?? Scope.Empty));

            // This would normally need the user to visit the verification URL to allow the user to logon, but the code returned
            // here can be used immediately with the /oauth2/token endpoint using grant type urn:ietf:params:oauth:grant-type:device_code
            authorization.Authorize(settings.DefaultSubject);

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
}
