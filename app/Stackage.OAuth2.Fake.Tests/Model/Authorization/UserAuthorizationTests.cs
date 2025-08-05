namespace Stackage.OAuth2.Fake.Tests.Model.Authorization;

using NUnit.Framework;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Model.Authorization;

public class UserAuthorizationTests
{
   [Test]
   public void is_authenticated_is_initially_false()
   {
      var testSubject = CreateAuthorization();

      Assert.That(testSubject.IsAuthenticated, Is.False);
   }

   [Test]
   public void subject_is_initially_null()
   {
      var testSubject = CreateAuthorization();

      Assert.That(testSubject.Subject, Is.Null);
   }

   [Test]
   public void is_authenticated_is_true_after_authenticate()
   {
      var testSubject = CreateAuthorization();

      testSubject.Authenticate("AnySubject");

      Assert.That(testSubject.IsAuthenticated, Is.True);
   }

   [Test]
   public void subject_is_available_after_authenticate()
   {
      var testSubject = CreateAuthorization();

      testSubject.Authenticate("AnySubject");

      Assert.That(testSubject.Subject, Is.EqualTo("AnySubject"));
   }

   private static UserAuthorization CreateAuthorization(string? scope = null)
   {
      return UserAuthorization.Create(scope: (Scope?)scope ?? Scope.Empty);
   }
}
