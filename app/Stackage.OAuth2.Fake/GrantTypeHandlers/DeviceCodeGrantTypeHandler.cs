namespace Stackage.OAuth2.Fake.GrantTypeHandlers;

using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Stackage.OAuth2.Fake.Services;

public class DeviceCodeGrantTypeHandler : IGrantTypeHandler
{
   private const int TokenExpirySecs = 20 * 60;

   private readonly DeviceCodeCache _deviceCodeCache;
   private readonly Settings _settings;
   private readonly ITokenGenerator _tokenGenerator;

   public DeviceCodeGrantTypeHandler(
      DeviceCodeCache deviceCodeCache,
      Settings settings,
      ITokenGenerator tokenGenerator)
   {
      _deviceCodeCache = deviceCodeCache;
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

      if (!_deviceCodeCache.DeviceIsVerified(deviceCode.ToString()))
      {
         return Error.InvalidGrant("The given device_code was not found");
      }

      _deviceCodeCache.Remove(deviceCode.ToString());

      var claims = new List<Claim>
      {
         new(JwtRegisteredClaimNames.Sub, _settings.DefaultSubject)
      };

      var response = new TokenResponse(
         AccessToken: _tokenGenerator.Generate(claims, TokenExpirySecs),
         TokenType: "Bearer",
         ExpiresInSeconds: TokenExpirySecs
      );

      return TypedResults.Json(response, statusCode: 200);
   }

   private record TokenResponse(
      [property: JsonPropertyName("access_token")] string AccessToken,
      [property: JsonPropertyName("token_type")] string TokenType,
      [property: JsonPropertyName("expires_in")] int ExpiresInSeconds);
}
