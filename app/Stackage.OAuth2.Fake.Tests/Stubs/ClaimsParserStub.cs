namespace Stackage.OAuth2.Fake.Tests.Stubs;

using System.Collections.Immutable;
using System.Security.Claims;
using System.Text.Json.Nodes;
using Moq;
using Stackage.OAuth2.Fake.Services;

public static class ClaimsParserStub
{
   public static IClaimsParser Valid() => Returns();

   public static IClaimsParser Returns(params Claim[] claims)
   {
      var claimsArray = claims.ToImmutableArray();

      var mock = new Mock<IClaimsParser>();

      mock.Setup(m => m.TryParse(It.IsAny<JsonObject>(), out claimsArray)).Returns(true);

      return mock.Object;
   }
}
