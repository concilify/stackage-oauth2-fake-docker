namespace Stackage.OAuth2.Fake.GrantTypeHandlers;

using Microsoft.AspNetCore.Http;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Services;

public class DeviceCodeGrantTypeHandler : IGrantTypeHandler
{
   private readonly AuthorizationCache<DeviceAuthorization> _authorizationCache;
   private readonly ITokenGenerator _tokenGenerator;

   public DeviceCodeGrantTypeHandler(
      AuthorizationCache<DeviceAuthorization> authorizationCache,
      ITokenGenerator tokenGenerator)
   {
      _authorizationCache = authorizationCache;
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

      var response = _tokenGenerator.Generate(authorization);

      return TypedResults.Json(response, statusCode: 200);
   }
}
