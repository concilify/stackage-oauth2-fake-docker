namespace Stackage.OAuth2.Fake.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

public static class LogoutEndpoint
{
   public static void MapLogoutEndpoint(this WebApplication app)
   {
      var settings = app.Services.GetRequiredService<Settings>();

      app.MapGet(
         settings.LogoutPath,
         (
            [FromQuery(Name = "client_id")] string? clientId,
            [FromQuery(Name = "returnTo")] string? returnTo) =>
         {
            if (string.IsNullOrEmpty(returnTo))
            {
               return OAuth2Results.InvalidRequest("The returnTo parameter is required");
            }

            return Results.Redirect(returnTo);
         });
   }
}
