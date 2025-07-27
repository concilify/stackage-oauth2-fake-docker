namespace Stackage.OAuth2.Fake.GrantTypeHandlers;

using Microsoft.AspNetCore.Http;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Services;

public class RefreshTokenGrantTypeHandler : IGrantTypeHandler
{
   private readonly AuthorizationCache<RefreshAuthorization> _authorizationCache;
   private readonly ITokenGenerator _tokenGenerator;

   public RefreshTokenGrantTypeHandler(
      AuthorizationCache<RefreshAuthorization> authorizationCache,
      ITokenGenerator tokenGenerator)
   {
      _authorizationCache = authorizationCache;
      _tokenGenerator = tokenGenerator;
   }

   public string GrantType => GrantTypes.RefreshToken;

   public IResult Handle(HttpRequest httpRequest)
   {
      if (!httpRequest.Form.TryGetValue("refresh_token", out var refreshToken))
      {
         return Error.InvalidRequest("The refresh_token parameter was missing");
      }

      if (!_authorizationCache.TryGet(refreshToken.ToString(), out var authorization))
      {
         return Error.InvalidGrant("The given refresh_token was not found");
      }

      _authorizationCache.Remove(authorization);

      var response = _tokenGenerator.Generate(authorization);

      return TypedResults.Json(response, statusCode: 200);
   }
}
