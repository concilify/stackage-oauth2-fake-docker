namespace Stackage.OAuth2.Fake.Tests.Stubs;

using System.Collections.Generic;
using System.IO.Abstractions;
using Moq;

public static class FileStub
{
   public static IFile Empty() => Using(new Dictionary<string, string>());

   public static IFile Using(IDictionary<string, string> contents)
   {
      var mock = new Mock<IFile>();

      mock.Setup(m => m.Exists(It.IsAny<string>()))
         .Returns<string>(contents.ContainsKey);
      mock.Setup(m => m.ReadAllText(It.IsAny<string>()))
         .Returns<string>(path => contents[path]);
      mock.Setup(m => m.WriteAllText(It.IsAny<string>(), It.IsAny<string>()))
         .Callback<string, string>((path, content) => contents[path] = content);

      return mock.Object;
   }
}
