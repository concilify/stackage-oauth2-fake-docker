namespace Stackage.OAuth2.Fake.GrantTypeHandlers;

using Microsoft.AspNetCore.Http;

public class RefreshTokenGrantTypeHandler : IGrantTypeHandler
{
   public string GrantType => GrantTypes.RefreshToken;

   public IResult Handle(HttpRequest httpRequest)
   {
      // refresh_token must exist
      // use AuthorizationCache<RefreshTokenAuthorization>
      // additional scopes are ignored if included

      throw new System.NotImplementedException();
   }
}
