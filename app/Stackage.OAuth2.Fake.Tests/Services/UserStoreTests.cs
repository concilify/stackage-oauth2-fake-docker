namespace Stackage.OAuth2.Fake.Tests.Services;

using System.IdentityModel.Tokens.Jwt;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Claims;
using NUnit.Framework;
using Shouldly;
using Stackage.OAuth2.Fake.Services;
using Stackage.OAuth2.Fake.Tests.Stubs;

public class UserStoreTests
{
   [Test]
   public void try_get_returns_false_and_null_when_users_file_does_not_exist()
   {
      var fileSystem = FileSystemStub.With(FileStub.DoesNotExist());

      var testSubject = CreateStore(fileSystem: fileSystem);

      var result = testSubject.TryGet("valid-subject", out var user);

      Assert.That(result, Is.False);
      Assert.That(user, Is.Null);
   }

   [Test]
   public void try_get_returns_false_and_null_when_users_file_is_empty()
   {
      const string usersJson = "[]";

      var fileSystem = FileSystemStub.With(FileStub.Exists(usersJson));

      var testSubject = CreateStore(fileSystem: fileSystem);

      var result = testSubject.TryGet("valid-subject", out var user);

      Assert.That(result, Is.False);
      Assert.That(user, Is.Null);
   }

   [Test]
   public void try_get_returns_true_and_user_when_user_exists()
   {
      const string usersJson = """
                               [
                                  {
                                     "subject": "arbitrary-subject"
                                  }
                               ]
                               """;

      var fileSystem = FileSystemStub.With(FileStub.Exists(usersJson));

      var testSubject = CreateStore(fileSystem: fileSystem);

      var result = testSubject.TryGet("arbitrary-subject", out var user);

      Assert.That(result, Is.True);
      Assert.That(user, Is.Not.Null);
      Assert.That(user!.Subject, Is.EqualTo("arbitrary-subject"));
   }

   [Test]
   public void try_get_returns_user_with_multiple_claims_when_multiple_claims_exists()
   {
      const string usersJson = """
                               [
                                  {
                                     "subject": "arbitrary-subject"
                                  }
                               ]
                               """;

      var fileSystem = FileSystemStub.With(FileStub.Exists(usersJson));

      var claimsParser = ClaimsParserStub.Returns(
         new Claim("nickname", "arbitrary-nickname"),
         new Claim("picture", "arbitrary-picture"));

      var testSubject = CreateStore(
         fileSystem: fileSystem,
         claimsParser: claimsParser);

      testSubject.TryGet("arbitrary-subject", out var user);

      string[] names = [JwtRegisteredClaimNames.Nickname, JwtRegisteredClaimNames.Picture];

      var expectedClaims = new Claim[]
      {
         new(JwtRegisteredClaimNames.Nickname, "arbitrary-nickname"),
         new(JwtRegisteredClaimNames.Picture, "arbitrary-picture"),
      };

      Assert.That(user, Is.Not.Null);

      user!.GetClaims(names).ToArray().ShouldBeEquivalentTo(expectedClaims);
   }

   [Test]
   public void Add()
   {
      Assert.Fail();
   }

   private static UserStore CreateStore(
      IFileSystem? fileSystem = null,
      IClaimsParser? claimsParser = null)
   {
      fileSystem ??= FileSystemStub.Valid();
      claimsParser ??= ClaimsParserStub.Valid();

      return new UserStore(fileSystem, claimsParser);
   }
}
