namespace Stackage.OAuth2.Fake.Tests.Stubs;

using System.Collections.Immutable;
using System.Security.Claims;
using System.Text.Json.Nodes;
using Moq;
using Stackage.OAuth2.Fake.Services;

public static class ClaimsSerializerStub
{
   public static IClaimsSerializer Valid() => DeserializeReturns();

   public static IClaimsSerializer DeserializeReturns(params Claim[] claims)
   {
      var claimsArray = claims.ToImmutableArray();

      var mock = new Mock<IClaimsSerializer>();

      mock.Setup(m => m.TryDeserialize(It.IsAny<JsonObject>(), out claimsArray)).Returns(true);

      return mock.Object;
   }

   public static IClaimsSerializer SerializeReturns(JsonObject claimsObject)
   {
      var mock = new Mock<IClaimsSerializer>();

      mock.Setup(m => m.Serialize(It.IsAny<ImmutableArray<Claim>>())).Returns(claimsObject);

      return mock.Object;
   }
}
