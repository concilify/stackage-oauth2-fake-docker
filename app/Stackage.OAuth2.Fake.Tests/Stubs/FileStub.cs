namespace Stackage.OAuth2.Fake.Tests.Stubs;

using System.IO.Abstractions;
using Moq;

public static class FileStub
{
   public static IFile DoesNotExist()
   {
      var mock = new Mock<IFile>();

      mock.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);

      return mock.Object;
   }

   public static IFile Exists(string content)
   {
      var mock = new Mock<IFile>();

      mock.Setup(m => m.Exists(It.IsAny<string>())).Returns(true);
      mock.Setup(m => m.ReadAllText(It.IsAny<string>())).Returns(content);

      return mock.Object;
   }
}
