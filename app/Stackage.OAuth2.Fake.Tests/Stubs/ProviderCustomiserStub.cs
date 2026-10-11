namespace Stackage.OAuth2.Fake.Tests.Stubs;

using System.Collections.Generic;
using System.Security.Claims;
using Moq;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Services.ProviderCustomisers;

public static class ProviderCustomiserStub
{
   public static IProviderCustomiser AddingClaims(string providerMode, Claim accessTokenClaim, Claim idTokenClaim)
   {
      var mock = new Mock<IProviderCustomiser>();

      mock.Setup(m => m.ProviderMode).Returns(providerMode);

      mock
         .Setup(m => m.AddAccessTokenClaims(It.IsAny<User?>(), It.IsAny<IList<Claim>>()))
         .Callback<User?, IList<Claim>>((_, claims) => claims.Add(accessTokenClaim));

      mock
         .Setup(m => m.AddIdTokenClaims(It.IsAny<User?>(), It.IsAny<Scope>(), It.IsAny<IList<Claim>>()))
         .Callback<User?, Scope, IList<Claim>>((_, _, claims) => claims.Add(idTokenClaim));

      return mock.Object;
   }
}
