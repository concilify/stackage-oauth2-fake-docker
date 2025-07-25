namespace Stackage.OAuth2.Fake.GrantTypeHandlers;

using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Services;

public class DeviceCodeGrantTypeHandler : IGrantTypeHandler
{
   private const int TokenExpirySecs = 20 * 60;

   private readonly AuthorizationCache<DeviceAuthorization> _authorizationCache;
   private readonly Settings _settings;
   private readonly ITokenGenerator _tokenGenerator;

   public DeviceCodeGrantTypeHandler(
      AuthorizationCache<DeviceAuthorization> authorizationCache,
      Settings settings,
      ITokenGenerator tokenGenerator)
   {
      _authorizationCache = authorizationCache;
      _settings = settings;
      _tokenGenerator = tokenGenerator;
   }

   public string GrantType => GrantTypes.DeviceCode;

   public IResult Handle(HttpRequest httpRequest)
   {
      if (!httpRequest.Form.TryGetValue("device_code", out var deviceCode))
      {
         return Error.InvalidRequest("The device_code parameter was missing");
      }

      if (!_authorizationCache.TryGet(deviceCode.ToString(), out var authorization))
      {
         return Error.InvalidGrant("The given device_code was not found");
      }

      _authorizationCache.Remove(authorization);

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
