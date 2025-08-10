namespace Stackage.OAuth2.Fake.Tests.Stubs;

using Moq;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Services;

public static class UserStoreStub
{
   public static IUserStore Valid() => Mock.Of<IUserStore>();

   public static IUserStore Returns(User user)
   {
      var mock = new Mock<IUserStore>();

      mock.Setup(m => m.TryGet(user.Subject, out user!)).Returns(true);

      return mock.Object;
   }

   public static IUserStore NotFound()
   {
      var mock = new Mock<IUserStore>();

      User? user = null;
      mock.Setup(m => m.TryGet(It.IsAny<string>(), out user)).Returns(false);

      return mock.Object;
   }
}
