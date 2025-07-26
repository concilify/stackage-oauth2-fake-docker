namespace Stackage.OAuth2.Fake.GrantTypeHandlers;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Services;

public class AuthorizationCodeGrantTypeHandler : IGrantTypeHandler
{
   private const int TokenExpirySecs = 20 * 60;

   private readonly AuthorizationCache<UserAuthorization> _authorizationCache;
   private readonly Settings _settings;
   private readonly ITokenGenerator _tokenGenerator;

   public AuthorizationCodeGrantTypeHandler(
      AuthorizationCache<UserAuthorization> authorizationCache,
      Settings settings,
      ITokenGenerator tokenGenerator)
   {
      _authorizationCache = authorizationCache;
      _settings = settings;
      _tokenGenerator = tokenGenerator;
   }

   public string GrantType => GrantTypes.AuthorizationCode;

   public IResult Handle(HttpRequest httpRequest)
   {
      if (!httpRequest.Form.TryGetValue("code", out var code))
      {
         return Error.InvalidRequest("The code parameter was missing");
      }

      if (!_authorizationCache.TryGet(code.ToString(), out var authorization))
      {
         return Error.InvalidGrant("The given code was not found");
      }

      _authorizationCache.Remove(authorization);

      var claims = new List<Claim>
      {
         new(JwtRegisteredClaimNames.Sub, _settings.DefaultSubject)
      };

      if (scope.Length != 0)
      {
         claims.Add(new Claim("scope", string.Join(" ", scope)));
      }

      var response = new JsonObject
      {
         ["access_token"] = _tokenGenerator.Generate(claims, TokenExpirySecs)
      };

      if (scope.Length != 0)
      {
         if (scope.Contains("offline_access"))
         {
            response["refresh_token"] = Guid.NewGuid().ToString();
         }

         response["scope"] = string.Join(" ", scope);
      }

      response["expires_in"] = TokenExpirySecs;
      response["token_type"] = "Bearer";

      return TypedResults.Json(response, statusCode: 200);
   }
}
