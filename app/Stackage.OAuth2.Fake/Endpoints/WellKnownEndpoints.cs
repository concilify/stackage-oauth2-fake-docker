namespace Stackage.OAuth2.Fake.Endpoints;

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Stackage.OAuth2.Fake.GrantTypeHandlers;
using Stackage.OAuth2.Fake.Services;

public static class WellKnownEndpoints
{
   public static void MapWellKnownEndpoints(this WebApplication app)
   {
      app.MapGet(
         "/.well-known/openid-configuration",
         (
            Settings settings,
            IEnumerable<IGrantTypeHandler> grantTypeHandlers
         ) =>
         {
            return TypedResults.Json(new
            {
               issuer = settings.IssuerUrl,
               jwks_uri = $"{settings.IssuerUrl}/.well-known/jwks.json",
               token_endpoint = $"{settings.IssuerUrl}{settings.TokenPath}",
               device_authorization_endpoint = $"{settings.IssuerUrl}{settings.DeviceAuthorizationPath}",
               grant_types_supported = grantTypeHandlers.Select(h => h.GrantType).ToArray(),
            });
         });

      app.MapGet(
         "/.well-known/jwks.json",
         (JsonWebKeyCache jsonWebKeyCache) =>
         {
            return TypedResults.Json(new
            {
               keys = jsonWebKeyCache.JsonWebKeys.Select(jwk => new
               {
                  alg = jwk.Alg,
                  kid = jwk.Kid,
                  use = jwk.Use,
                  kty = jwk.Kty,
                  n = jwk.N,
                  e = jwk.E
               })
            });
         });
   }
}
