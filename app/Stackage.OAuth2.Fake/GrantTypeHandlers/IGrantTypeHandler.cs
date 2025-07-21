namespace Stackage.OAuth2.Fake.GrantTypeHandlers;

using Microsoft.AspNetCore.Http;

public interface IGrantTypeHandler
{
   string GrantType { get; }

   IResult Handle(HttpRequest httpRequest);
}
