namespace Stackage.OAuth2.Fake.Endpoints;

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Stackage.OAuth2.Fake.GrantTypeHandlers;

public static class TokenEndpoint
{
   public static void MapTokenEndpoint(this WebApplication app)
   {
      app.MapPost(
         "/oauth2/token", IResult (
            IEnumerable<IGrantTypeHandler> grantTypeHandlers,
            HttpContext httpContext
         ) =>
         {
            if (!httpContext.Request.Form.TryGetValue("grant_type", out var grantType))
            {
               return Error.InvalidRequest("The grant_type parameter was missing");
            }

            var grantTypeHandler = grantTypeHandlers.FirstOrDefault(h => h.GrantType == grantType);

            if (grantTypeHandler == null)
            {
               return Error.UnsupportedGrantType();
            }

            return grantTypeHandler.Handle(httpContext.Request);
         });
   }
}
