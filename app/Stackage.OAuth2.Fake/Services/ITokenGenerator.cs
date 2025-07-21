namespace Stackage.OAuth2.Fake.Services;

using System.Collections.Generic;
using System.Security.Claims;

public interface ITokenGenerator
{
   string Generate(IList<Claim> claims, int expirySeconds);
}
