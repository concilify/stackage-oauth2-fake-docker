namespace Stackage.OAuth2.Fake.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stackage.OAuth2.Fake.Services;

public static class AuthorizationEndpoint
{
   public static void MapAuthorizationEndpoint(this WebApplication app)
   {
      // This would normally redirect to an intermediate URL to allow the user to logon
      app.MapGet(
         "/oauth2/authorize",
         (
            [FromQuery(Name = "state")] string state,
            [FromQuery(Name = "redirect_uri")] string redirectUri,
            AuthorizationCodeCache authorizationCodeCache
         ) =>
         {
            var code = authorizationCodeCache.Create();

            return TypedResults.Redirect($"{redirectUri}?code={code}&state={state}");
         });
   }
}
