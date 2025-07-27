namespace Stackage.OAuth2.Fake.Tests.Stubs;

using Moq;
using Stackage.OAuth2.Fake.Model;

public static class AuthorizationStub
{
   public static IAuthorization Valid() => With();

   public static IAuthorization With(
      Scope? scope = null,
      string? subject = null)
   {
      scope ??= Scope.Empty;
      subject ??= string.Empty;

      var mock = new Mock<IAuthorization>();

      mock.Setup(m => m.Scope).Returns(scope);
      mock.Setup(m => m.Subject).Returns(subject);

      return mock.Object;
   }
}
