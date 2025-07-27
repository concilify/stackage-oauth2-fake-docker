namespace Stackage.OAuth2.Fake.Services;

using Stackage.OAuth2.Fake.Model;

public interface ITokenGenerator
{
   TokenResponse Generate(IAuthorization authorization);
}
