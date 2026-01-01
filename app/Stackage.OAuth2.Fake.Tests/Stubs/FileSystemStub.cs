namespace Stackage.OAuth2.Fake.Tests.Stubs;

using System.IO.Abstractions;
using Moq;

public static class FileSystemStub
{
   public static IFileSystem Valid() => With(FileStub.DoesNotExist());

   public static IFileSystem With(IFile file)
   {
      var mock = new Mock<IFileSystem>();

      mock.Setup(m => m.File).Returns(file);

      return mock.Object;
   }
}
