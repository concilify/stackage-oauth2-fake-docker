namespace Stackage.OAuth2.Fake.Services;

using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Model.Authorization;

public interface ITokenGenerator
{
   TokenResponse Generate(IAuthorization authorization);
}
