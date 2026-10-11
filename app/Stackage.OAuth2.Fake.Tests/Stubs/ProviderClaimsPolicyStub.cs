namespace Stackage.OAuth2.Fake.Tests.Stubs;

using System.Collections.Immutable;
using System.Security.Claims;
using Moq;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Services.ProviderClaimsPolicies;

public static class ProviderClaimsPolicyStub
{
   public static IProviderClaimsPolicy ReturningNoClaims(string providerMode)
   {
      var mock = new Mock<IProviderClaimsPolicy>();

      mock.Setup(m => m.ProviderMode).Returns(providerMode);
      mock.Setup(m => m.GetAccessTokenClaims(It.IsAny<User>(), It.IsAny<Scope>())).Returns(ImmutableArray<Claim>.Empty);
      mock.Setup(m => m.GetIdTokenClaims(It.IsAny<User>(), It.IsAny<Scope>())).Returns(ImmutableArray<Claim>.Empty);

      return mock.Object;
   }

   public static IProviderClaimsPolicy AddingClaims(string providerMode, Claim accessTokenClaim, Claim idTokenClaim)
   {
      var mock = new Mock<IProviderClaimsPolicy>();

      mock.Setup(m => m.ProviderMode).Returns(providerMode);
      mock.Setup(m => m.GetAccessTokenClaims(It.IsAny<User>(), It.IsAny<Scope>())).Returns(ImmutableArray.Create(accessTokenClaim));
      mock.Setup(m => m.GetIdTokenClaims(It.IsAny<User>(), It.IsAny<Scope>())).Returns(ImmutableArray.Create(idTokenClaim));

      return mock.Object;
   }
}
