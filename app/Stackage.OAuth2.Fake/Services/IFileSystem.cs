namespace Stackage.OAuth2.Fake.Services;

public interface IFileSystem
{
   bool FileExists(string path);

   string ReadAllText(string path);
}
