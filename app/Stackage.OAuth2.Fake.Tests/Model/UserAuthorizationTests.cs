namespace Stackage.OAuth2.Fake.Tests.Model;

using System;
using NUnit.Framework;
using Stackage.OAuth2.Fake.Model;

public class UserAuthorizationTests
{
   [TestCaseSource(nameof(ScopeTestCases))]
   public void scope_properties_equal_the_expected_values(ScopeTestCase testCase)
   {
      var testSubject = CreateAuthorization(
         scope: testCase.GivenScope);

      Assert.That(testSubject.Scope, Is.EqualTo(testCase.ExpectedScope));
      Assert.That(testSubject.Scopes, Is.EqualTo(testCase.ExpectedScopes));
      Assert.That(testSubject.IncludeScope, Is.EqualTo(testCase.ExpectedIncludeScope));
      Assert.That(testSubject.IncludeRefreshToken, Is.EqualTo(testCase.ExpectedIncludeRefreshToken));
   }

   private static TestCaseData[] ScopeTestCases()
   {
      return
      [
         new TestCaseData(new ScopeTestCase(
            GivenScope: string.Empty,
            ExpectedScope: string.Empty,
            ExpectedScopes: [],
            ExpectedIncludeScope: false,
            ExpectedIncludeRefreshToken: false))
            .SetName("Empty"),
         new TestCaseData(new ScopeTestCase(
            GivenScope: "single_item",
            ExpectedScope: "single_item",
            ExpectedScopes: ["single_item"],
            ExpectedIncludeScope: true,
            ExpectedIncludeRefreshToken: false))
            .SetName("Single scope"),
         new TestCaseData(new ScopeTestCase(
            GivenScope: "first_item second_item",
            ExpectedScope: "first_item second_item",
            ExpectedScopes: ["first_item", "second_item"],
            ExpectedIncludeScope: true,
            ExpectedIncludeRefreshToken: false))
            .SetName("Two scopes"),
         new TestCaseData(new ScopeTestCase(
            GivenScope: $" first_item {Environment.NewLine} second_item ",
            ExpectedScope: "first_item second_item",
            ExpectedScopes: ["first_item", "second_item"],
            ExpectedIncludeScope: true,
            ExpectedIncludeRefreshToken: false))
            .SetName("Two scopes w/ whitespace"),
         new TestCaseData(new ScopeTestCase(
            GivenScope: "offline_access",
            ExpectedScope: "offline_access",
            ExpectedScopes: ["offline_access"],
            ExpectedIncludeScope: true,
            ExpectedIncludeRefreshToken: true))
            .SetName("Offline access alone"),
         new TestCaseData(new ScopeTestCase(
            GivenScope: "offline_access second_item",
            ExpectedScope: "offline_access second_item",
            ExpectedScopes: ["offline_access", "second_item"],
            ExpectedIncludeScope: true,
            ExpectedIncludeRefreshToken: true))
            .SetName("Offline access first of two"),
         new TestCaseData(new ScopeTestCase(
            GivenScope: "first_item offline_access",
            ExpectedScope: "first_item offline_access",
            ExpectedScopes: ["first_item", "offline_access"],
            ExpectedIncludeScope: true,
            ExpectedIncludeRefreshToken: true))
            .SetName("Offline access second of two"),
      ];
   }

   private static UserAuthorization CreateAuthorization(string scope)
   {
      return UserAuthorization.Create(scope: scope);
   }

   public record ScopeTestCase(
      string GivenScope,
      string ExpectedScope,
      string[] ExpectedScopes,
      bool ExpectedIncludeScope,
      bool ExpectedIncludeRefreshToken);
}
