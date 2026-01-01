namespace Stackage.OAuth2.Fake.Tests.Services;

using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using NUnit.Framework;
using Shouldly;
using Stackage.OAuth2.Fake.Services;
using Stackage.OAuth2.Fake.Tests.Stubs;

public class UserStoreTests
{
   private const string UsersFilePath = "users.json";

   [SetUp]
   public void setup()
   {
      CleanupUsersFile();
   }

   [TearDown]
   public void teardown()
   {
      CleanupUsersFile();
   }

   [Test]
   public void try_get_returns_false_and_null_when_user_does_not_exist()
   {
      var testSubject = CreateStore();

      var result = testSubject.TryGet("valid-subject", out var user);

      Assert.That(result, Is.False);
      Assert.That(user, Is.Null);
   }

   [Test]
   public void try_get_returns_true_and_user_when_user_exists()
   {
      var usersJson = """
         [
            {
               "subject": "arbitrary-subject",
               "claims": {
                  "nickname": "valid-nickname"
               }
            }
         ]
         """;

      File.WriteAllText(UsersFilePath, usersJson);

      var testSubject = CreateStore();

      var result = testSubject.TryGet("arbitrary-subject", out var user);

      Assert.That(result, Is.True);
      Assert.That(user, Is.Not.Null);
      Assert.That(user!.Subject, Is.EqualTo("arbitrary-subject"));
   }

   [Test]
   public void try_get_returns_user_with_multiple_claims_when_multiple_claims_exists()
   {
      var usersJson = """
         [
            {
               "subject": "arbitrary-subject",
               "claims": {
                  "nickname": "arbitrary-nickname",
                  "picture": "arbitrary-picture"
               }
            }
         ]
         """;

      File.WriteAllText(UsersFilePath, usersJson);

      var claimsParser = ClaimsParserStub.Returns(
         new Claim("nickname", "arbitrary-nickname"),
         new Claim("picture", "arbitrary-picture"));

      var testSubject = CreateStore(claimsParser: claimsParser);

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

   private static UserStore CreateStore(IClaimsParser? claimsParser = null)
   {
      claimsParser ??= ClaimsParserStub.Valid();

      return new UserStore(claimsParser);
   }

   private static void CleanupUsersFile()
   {
      if (File.Exists(UsersFilePath))
      {
         File.Delete(UsersFilePath);
      }
   }
}
