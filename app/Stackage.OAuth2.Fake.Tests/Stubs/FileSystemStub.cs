namespace Stackage.OAuth2.Fake.Tests.Stubs;

using Moq;
using Stackage.OAuth2.Fake.Services;

public static class FileSystemStub
{
   public static IFileSystem Empty()
   {
      return WithContent(fileExists: false, fileContent: string.Empty);
   }

   public static IFileSystem WithContent(string fileContent)
   {
      return WithContent(fileExists: true, fileContent: fileContent);
   }

   private static IFileSystem WithContent(bool fileExists, string fileContent)
   {
      var mock = new Mock<IFileSystem>();

      mock.Setup(m => m.FileExists(It.IsAny<string>())).Returns(fileExists);
      mock.Setup(m => m.ReadAllText(It.IsAny<string>())).Returns(fileContent);

      return mock.Object;
   }
}
