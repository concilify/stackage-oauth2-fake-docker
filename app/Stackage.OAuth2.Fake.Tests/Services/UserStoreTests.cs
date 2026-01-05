namespace Stackage.OAuth2.Fake.Tests.Services;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Nodes;
using NUnit.Framework;
using Shouldly;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Services;
using Stackage.OAuth2.Fake.Tests.Stubs;

public class UserStoreTests
{
   private string _usersPath = string.Empty;

   [OneTimeSetUp]
   public void setup_once_before_all_tests()
   {
      _usersPath = Path.Combine(AppContext.BaseDirectory, "users.json");
   }

   [Test]
   public void try_get_returns_false_and_null_when_users_file_does_not_exist()
   {
      var fileSystem = FileSystemStub.With(FileStub.Empty());

      var testSubject = CreateStore(fileSystem: fileSystem);

      var result = testSubject.TryGet("valid-subject", out var user);

      Assert.That(result, Is.False);
      Assert.That(user, Is.Null);
   }

   [Test]
   public void try_get_returns_false_and_null_when_users_file_is_empty_json()
   {
      var files = new Dictionary<string, string>
      {
         [_usersPath] = "[]",
      };
      var fileSystem = FileSystemStub.With(FileStub.Using(files));

      var testSubject = CreateStore(fileSystem: fileSystem);

      var result = testSubject.TryGet("valid-subject", out var user);

      Assert.That(result, Is.False);
      Assert.That(user, Is.Null);
   }

   [Test]
   public void try_get_returns_true_and_user_when_user_exists()
   {
      var files = new Dictionary<string, string>
      {
         [_usersPath] = """
                          [
                             {
                                "subject": "arbitrary-subject",
                                "claims": {}
                             }
                          ]
                          """,
      };
      var fileSystem = FileSystemStub.With(FileStub.Using(files));

      var testSubject = CreateStore(fileSystem: fileSystem);

      var result = testSubject.TryGet("arbitrary-subject", out var user);

      Assert.That(result, Is.True);
      Assert.That(user, Is.Not.Null);
      Assert.That(user!.Subject, Is.EqualTo("arbitrary-subject"));
   }

   [Test]
   public void try_get_returns_user_with_multiple_claims_when_multiple_claims_exists()
   {
      var files = new Dictionary<string, string>
      {
         [_usersPath] = """
                          [
                             {
                                "subject": "arbitrary-subject",
                                "claims": {}
                             }
                          ]
                          """,
      };
      var fileSystem = FileSystemStub.With(FileStub.Using(files));

      var claimsParser = ClaimsSerializerStub.DeserializeReturns(
         new Claim("nickname", "arbitrary-nickname"),
         new Claim("picture", "arbitrary-picture"));

      var testSubject = CreateStore(
         fileSystem: fileSystem,
         claimsSerializer: claimsParser);

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
   public void try_add_creates_file_when_does_not_exist()
   {
      var files = new Dictionary<string, string>();
      var fileSystem = FileSystemStub.With(FileStub.Using(files));
      var claimsSerializer = ClaimsSerializerStub.SerializeReturns(new JsonObject
      {
         ["arbitrary-claim"] = "arbitrary-value",
      });

      var testSubject = CreateStore(
         fileSystem: fileSystem,
         claimsSerializer: claimsSerializer);

      var result = testSubject.TryAdd(new User("arbitrary-subject", []));

      var expectedUsers = new JsonArray
      {
         new JsonObject
         {
            ["subject"] = "arbitrary-subject",
            ["claims"] = new JsonObject
            {
               ["arbitrary-claim"] = "arbitrary-value",
            },
         },
      };

      Assert.That(result, Is.True);
      Assert.That(files.ContainsKey(_usersPath), Is.True);
      Assert.That(files[_usersPath], Is.EqualTo(expectedUsers.ToJsonString()));
   }

   [Test]
   public void try_add_adds_user_to_empty_json_file()
   {
      var files = new Dictionary<string, string>
      {
         [_usersPath] = "[]",
      };
      var fileSystem = FileSystemStub.With(FileStub.Using(files));
      var claimsSerializer = ClaimsSerializerStub.SerializeReturns(new JsonObject
      {
         ["arbitrary-claim"] = "arbitrary-value",
      });

      var testSubject = CreateStore(
         fileSystem: fileSystem,
         claimsSerializer: claimsSerializer);

      var result = testSubject.TryAdd(new User("arbitrary-subject", []));

      var expectedUsers = new JsonArray
      {
         new JsonObject
         {
            ["subject"] = "arbitrary-subject",
            ["claims"] = new JsonObject
            {
               ["arbitrary-claim"] = "arbitrary-value",
            },
         },
      };

      Assert.That(result, Is.True);
      Assert.That(files.ContainsKey(_usersPath), Is.True);
      Assert.That(files[_usersPath], Is.EqualTo(expectedUsers.ToJsonString()));
   }

   private static UserStore CreateStore(
      IFileSystem? fileSystem = null,
      IClaimsSerializer? claimsSerializer = null)
   {
      fileSystem ??= FileSystemStub.Valid();
      claimsSerializer ??= ClaimsSerializerStub.Valid();

      return new UserStore(
         fileSystem,
         claimsSerializer);
   }
}
