namespace Stackage.OAuth2.Fake.Tests.Services;

using System.Linq;
using System.Security.Claims;
using System.Text.Json.Nodes;
using NUnit.Framework;
using Shouldly;
using Stackage.OAuth2.Fake.Services;

public class ClaimsParserTests
{
   [Test]
   public void try_parse_returns_true_and_empty_list_for_empty_object()
   {
      var testSubject = CreateParser();

      var result = testSubject.TryParse(new JsonObject(), out var claims);

      Assert.That(result, Is.True);
      Assert.That(claims, Is.Empty);
   }

   [Test]
   public void try_parse_returns_true_and_list_with_single_item_for_single_literal_claim()
   {
      var testSubject = CreateParser();

      var claimsObject = new JsonObject
      {
         ["sub"] = "any-subject"
      };

      var result = testSubject.TryParse(claimsObject, out var claims);

      Assert.That(result, Is.True);
      Assert.That(claims.Length, Is.EqualTo(1));
      Assert.That(claims[0].Type, Is.EqualTo("sub"));
      Assert.That(claims[0].Value, Is.EqualTo("any-subject"));
      Assert.That(claims[0].ValueType, Is.EqualTo("http://www.w3.org/2001/XMLSchema#string"));
   }

   [TestCase(1379)]
   [TestCase(true)]
   [TestCase(null)]
   public void try_parse_returns_false_and_empty_list_for_non_string_literal(object? value)
   {
      var testSubject = CreateParser();

      var claimsObject = new JsonObject
      {
         ["sub"] = JsonValue.Create(value)
      };

      var result = testSubject.TryParse(claimsObject, out var claims);

      Assert.That(result, Is.False);
      Assert.That(claims, Is.Empty);
   }

   [Test]
   public void try_parse_returns_true_and_list_with_single_item_for_array_claim_with_one_item()
   {
      var testSubject = CreateParser();

      var claimsObject = new JsonObject
      {
         ["any-claim"] = new JsonArray { "any-value" }
      };

      var result = testSubject.TryParse(claimsObject, out var claims);

      Assert.That(result, Is.True);
      Assert.That(claims.Length, Is.EqualTo(1));
      Assert.That(claims[0].Type, Is.EqualTo("any-claim"));
      Assert.That(claims[0].Value, Is.EqualTo("[\"any-value\"]"));
      Assert.That(claims[0].ValueType, Is.EqualTo("JSON_ARRAY"));
   }

   [Test]
   public void try_parse_returns_true_and_list_with_single_item_for_array_claim_with_two_items()
   {
      var testSubject = CreateParser();

      var claimsObject = new JsonObject
      {
         ["any-claim"] = new JsonArray { "value-one", "value-two" }
      };

      var result = testSubject.TryParse(claimsObject, out var claims);

      Assert.That(result, Is.True);
      Assert.That(claims.Length, Is.EqualTo(1));
      Assert.That(claims[0].Type, Is.EqualTo("any-claim"));
      Assert.That(claims[0].Value, Is.EqualTo("[\"value-one\",\"value-two\"]"));
      Assert.That(claims[0].ValueType, Is.EqualTo("JSON_ARRAY"));
   }

   [TestCase(1379)]
   [TestCase(true)]
   [TestCase(null)]
   public void try_parse_returns_false_and_empty_list_for_array_claim_with_non_string_literal(object? value)
   {
      var testSubject = CreateParser();

      var claimsObject = new JsonObject
      {
         ["any-claim"] = new JsonArray { "value-one", value }
      };

      var result = testSubject.TryParse(claimsObject, out var claims);

      Assert.That(result, Is.False);
      Assert.That(claims, Is.Empty);
   }

   [TestCase("NAME", "name")]
   [TestCase("Name", "name")]
   [TestCase("NICKNAME", "nickname")]
   [TestCase("Nickname", "nickname")]
   [TestCase("PICTURE", "picture")]
   [TestCase("Picture",  "picture")]
   public void try_parse_normalises_known_claim_name(string name, string expectedName)
   {
      var testSubject = CreateParser();

      var claimsObject = new JsonObject
      {
         [name] = "any-value"
      };

      var result = testSubject.TryParse(claimsObject, out var claims);

      var expectedClaims = new Claim[]
      {
         new(expectedName, "any-value"),
      };

      Assert.That(result, Is.True);
      Assert.That(claims.Length, Is.EqualTo(1));

      claims.ToArray().ShouldBeEquivalentTo(expectedClaims);
   }

   [TestCase("UPPER")]
   [TestCase("lower")]
   [TestCase("Capitalized")]
   public void try_parse_does_not_normalise_unknown_claim_name(string name)
   {
      var testSubject = CreateParser();

      var claimsObject = new JsonObject
      {
         [name] = "any-value"
      };

      var result = testSubject.TryParse(claimsObject, out var claims);

      var expectedClaims = new Claim[]
      {
         new(name, "any-value"),
      };

      Assert.That(result, Is.True);
      Assert.That(claims.Length, Is.EqualTo(1));

      claims.ToArray().ShouldBeEquivalentTo(expectedClaims);
   }

   private static ClaimsParser CreateParser()
   {
      return new ClaimsParser();
   }
}
