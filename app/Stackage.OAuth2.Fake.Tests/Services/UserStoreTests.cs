namespace Stackage.OAuth2.Fake.Tests.Services;

using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Shouldly;
using Stackage.OAuth2.Fake.Services;
using Stackage.OAuth2.Fake.Tests.Stubs;

public class UserStoreTests
{
   [Test]
   public void try_get_returns_false_and_null_when_user_does_not_exist()
   {
      var testSubject = CreateStore(
         configuration: ConfigurationStub.Empty());

      var result = testSubject.TryGet("any-subject", out var user);

      Assert.That(result, Is.False);
      Assert.That(user, Is.Null);
   }

   [Test]
   public void try_get_returns_true_and_user_when_user_exists()
   {
      var users = new Dictionary<string, string?>
      {
         ["Users:0:Subject"] = "existing-subject",
         ["Users:0:Claims:Nickname"] = "any-nickname"
      };

      var testSubject = CreateStore(
         configuration: ConfigurationStub.With(users));

      var result = testSubject.TryGet("existing-subject", out var user);

      Assert.That(result, Is.True);
      Assert.That(user, Is.Not.Null);
      Assert.That(user!.Subject, Is.EqualTo("existing-subject"));
   }

   [Test]
   public void try_get_returns_user_with_claims_when_claims_exists()
   {
      var users = new Dictionary<string, string?>
      {
         ["Users:0:Subject"] = "existing-subject",
         ["Users:0:Claims:Nickname"] = "any-nickname",
         ["Users:0:Claims:Picture"] = "any-picture"
      };

      var testSubject = CreateStore(
         configuration: ConfigurationStub.With(users));

      testSubject.TryGet("existing-subject", out var user);

      string[] names = [JwtRegisteredClaimNames.Nickname, JwtRegisteredClaimNames.Picture];

      var expectedClaims = new Claim[]
      {
         new(JwtRegisteredClaimNames.Nickname, "any-nickname"),
         new(JwtRegisteredClaimNames.Picture, "any-picture"),
      };

      Assert.That(user, Is.Not.Null);

      user!.GetClaims(names).ToArray().ShouldBeEquivalentTo(expectedClaims);
   }

   private static UserStore CreateStore(
      IConfiguration? configuration = null)
   {
      configuration ??= ConfigurationStub.Empty();

      return new UserStore(configuration);
   }
}
