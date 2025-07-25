namespace Stackage.OAuth2.Fake.Endpoints;

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public static class AuthorizationEndpoint
{
   public static void MapAuthorizationEndpoint(this WebApplication app)
   {
      // This would normally redirect to an intermediate URL to allow the user to logon
      app.MapGet(
         "/oauth2/authorize", IResult (
            [FromQuery(Name = "state")] string state,
            [FromQuery(Name = "redirect_uri")] string redirectUri
         ) => TypedResults.Redirect($"{redirectUri}?code={Guid.NewGuid()}&state={state}"));
   }
}
