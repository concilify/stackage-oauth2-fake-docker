namespace Stackage.OAuth2.Fake.GrantTypeHandlers;

using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

public class DeviceCodeGrantTypeHandler : IGrantTypeHandler
{
   private readonly DeviceCodeCache _deviceCodeCache;

   public DeviceCodeGrantTypeHandler(DeviceCodeCache deviceCodeCache)
   {
      _deviceCodeCache = deviceCodeCache;
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

      // TODO: Future PR - create a signed JWT when keys are available for signing and verification
      var response = new TokenResponse(
         AccessToken: "FakeAccessToken",
         TokenType: "Bearer",
         ExpiresInSeconds: 1200
      );

      return TypedResults.Json(response, statusCode: 200);
   }

   private record TokenResponse(
      [property: JsonPropertyName("access_token")] string AccessToken,
      [property: JsonPropertyName("token_type")] string TokenType,
      [property: JsonPropertyName("expires_in")] int ExpiresInSeconds);
}
