namespace Stackage.OAuth2.Fake.GrantTypeHandlers;

using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Stackage.OAuth2.Fake.Services;

public class AuthorizationCodeGrantTypeHandler : IGrantTypeHandler
{
   private const int TokenExpirySecs = 20 * 60;

   private readonly AuthorizationCodeCache _authorizationCodeCache;
   private readonly Settings _settings;
   private readonly ITokenGenerator _tokenGenerator;

   public AuthorizationCodeGrantTypeHandler(
      AuthorizationCodeCache authorizationCodeCache,
      Settings settings,
      ITokenGenerator tokenGenerator)
   {
      _authorizationCodeCache = authorizationCodeCache;
      _settings = settings;
      _tokenGenerator = tokenGenerator;
   }

   public string GrantType => "authorization_code";

   public IResult Handle(HttpRequest httpRequest)
   {
      if (!httpRequest.Form.TryGetValue("code", out var code))
      {
         return Error.InvalidRequest("The code parameter was missing");
      }

      if (!_authorizationCodeCache.CodeIsValid(code.ToString()))
      {
         return Error.InvalidGrant("The given code was not found");
      }

      _authorizationCodeCache.Remove(code.ToString());

      var claims = new List<Claim>
      {
         new(JwtRegisteredClaimNames.Sub, _settings.DefaultSubject)
      };

      var response = new
      {
         access_token = _tokenGenerator.Generate(claims, TokenExpirySecs),
         token_type = "Bearer",
         expires_in = TokenExpirySecs
      };

      return TypedResults.Json(response, statusCode: 200);
   }
}
