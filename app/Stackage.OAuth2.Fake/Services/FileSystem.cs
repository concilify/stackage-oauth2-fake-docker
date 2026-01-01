namespace Stackage.OAuth2.Fake.Services;

using System.IO;

public class FileSystem : IFileSystem
{
   public bool FileExists(string path)
   {
      return File.Exists(path);
   }

   public string ReadAllText(string path)
   {
      return File.ReadAllText(path);
   }
}
