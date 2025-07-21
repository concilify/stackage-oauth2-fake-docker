namespace Stackage.OAuth2.Fake.Tests.Stubs;

using Moq;
using Stackage.OAuth2.Fake.Services;

public static class TokenGeneratorStub
{
   public static ITokenGenerator Valid() => Mock.Of<ITokenGenerator>();
}
