namespace Stackage.OAuth2.Fake.GrantTypeHandlers;

using Microsoft.AspNetCore.Http;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Services;

public class AuthorizationCodeGrantTypeHandler : IGrantTypeHandler
{
   private readonly AuthorizationCache<UserAuthorization> _authorizationCache;
   private readonly ITokenGenerator _tokenGenerator;

   public AuthorizationCodeGrantTypeHandler(
      AuthorizationCache<UserAuthorization> authorizationCache,
      ITokenGenerator tokenGenerator)
   {
      _authorizationCache = authorizationCache;
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

      // TODO: Share the following with refresh_code

      _authorizationCache.Remove(authorization);

      var response = _tokenGenerator.Generate(authorization);

      return TypedResults.Json(response, statusCode: 200);
   }
}
