namespace Stackage.OAuth2.Fake.Tests.Model.Authorization;

using System;
using NUnit.Framework;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Model.Authorization;

public class DeviceAuthorizationTests
{
   [Test]
   public void is_authorized_is_initially_false()
   {
      var testSubject = CreateAuthorization();

      Assert.That(testSubject.IsAuthorized, Is.False);
   }

   [Test]
   public void subject_throws_exception_initially()
   {
      var testSubject = CreateAuthorization();

      Assert.Throws<InvalidOperationException>(() => _ = testSubject.Subject);
   }

   [Test]
   public void is_authorized_is_true_after_authorize()
   {
      var testSubject = CreateAuthorization();

      testSubject.Authorize("AnySubject");

      Assert.That(testSubject.IsAuthorized, Is.True);
   }

   [Test]
   public void subject_is_available_after_authorize()
   {
      var testSubject = CreateAuthorization();

      testSubject.Authorize("AnySubject");

      Assert.That(testSubject.Subject, Is.EqualTo("AnySubject"));
   }

   private static DeviceAuthorization CreateAuthorization(string? scope = null)
   {
      return DeviceAuthorization.Create(scope: (Scope?)scope ?? Scope.Empty);
   }
}
