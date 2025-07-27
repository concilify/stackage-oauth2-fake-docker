namespace Stackage.OAuth2.Fake.Tests.Model;

using System;
using NUnit.Framework;
using Stackage.OAuth2.Fake.Model;

public class ScopeTests
{
   [Test]
   public void empty_field_creates_empty_scope()
   {
      var testSubject = Scope.Empty;

      Assert.That(testSubject, Is.Empty);
      Assert.That(testSubject.IsEmpty, Is.True);
      Assert.That((string)testSubject, Is.Empty);
      Assert.That(testSubject.ToString(), Is.Empty);
   }

   [Test]
   public void empty_field_returns_same_instance()
   {
      var testSubject = Scope.Empty;

      Assert.That(testSubject, Is.SameAs(Scope.Empty));
   }

   [TestCaseSource(nameof(ScopeTestCases))]
   public void explicit_cast_creates_scope_with_expected_properties(ScopeTestCase testCase)
   {
      var testSubject = (Scope)testCase.GivenScope;

      Assert.That(testSubject, Is.EqualTo(testCase.ExpectedScopes));
      Assert.That(testSubject.IsEmpty, Is.EqualTo(testCase.ExpectedScopes.Length == 0));
      Assert.That((string)testSubject, Is.EqualTo(testCase.ExpectedScope));
      Assert.That(testSubject.ToString(), Is.EqualTo(testCase.ExpectedScope));
   }

   [Test]
   public void explicit_case_of_null_string_returns_null()
   {
      string? nullString = null;

      var testSubject = (Scope?)nullString;

      Assert.That(testSubject, Is.Null);
   }

   private static TestCaseData[] ScopeTestCases()
   {
      return
      [
         new TestCaseData(new ScopeTestCase(
            GivenScope: string.Empty,
            ExpectedScope: string.Empty,
            ExpectedScopes: []))
            .SetName("Empty"),
         new TestCaseData(new ScopeTestCase(
            GivenScope: "single_item",
            ExpectedScope: "single_item",
            ExpectedScopes: ["single_item"]))
            .SetName("Single scope"),
         new TestCaseData(new ScopeTestCase(
            GivenScope: "first_item second_item",
            ExpectedScope: "first_item second_item",
            ExpectedScopes: ["first_item", "second_item"]))
            .SetName("Two scopes"),
         new TestCaseData(new ScopeTestCase(
            GivenScope: $" first_item {Environment.NewLine} second_item ",
            ExpectedScope: "first_item second_item",
            ExpectedScopes: ["first_item", "second_item"]))
            .SetName("Two scopes w/ whitespace"),
      ];
   }

   public record ScopeTestCase(
      string GivenScope,
      string ExpectedScope,
      string[] ExpectedScopes);
}
