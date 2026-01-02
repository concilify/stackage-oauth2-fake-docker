namespace Stackage.OAuth2.Fake.Tests.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Shouldly;
using Stackage.OAuth2.Fake.Services;

public class ClaimsSerializerTests
{
   [Test]
   public void try_deserialize_returns_true_and_empty_list_for_empty_object()
   {
      var testSubject = CreateSerializer();

      var result = testSubject.TryDeserialize(new JsonObject(), out var claims);

      Assert.That(result, Is.True);
      Assert.That(claims, Is.Empty);
   }

   [Test]
   public void try_deserialize_returns_true_and_list_with_single_item_for_single_literal_claim()
   {
      var testSubject = CreateSerializer();

      var claimsObject = new JsonObject
      {
         ["sub"] = "arbitrary-subject",
      };

      var result = testSubject.TryDeserialize(claimsObject, out var claims);

      Assert.That(result, Is.True);
      Assert.That(claims.Length, Is.EqualTo(1));
      Assert.That(claims[0].Type, Is.EqualTo("sub"));
      Assert.That(claims[0].Value, Is.EqualTo("arbitrary-subject"));
      Assert.That(claims[0].ValueType, Is.EqualTo("http://www.w3.org/2001/XMLSchema#string"));
   }

   [TestCase(1379)]
   [TestCase(true)]
   [TestCase(null)]
   public void try_deserialize_returns_false_and_empty_list_for_non_string_literal(object? value)
   {
      var testSubject = CreateSerializer();

      var claimsObject = new JsonObject
      {
         ["sub"] = JsonValue.Create(value),
      };

      var result = testSubject.TryDeserialize(claimsObject, out var claims);

      Assert.That(result, Is.False);
      Assert.That(claims, Is.Empty);
   }

   [Test]
   public void try_deserialize_returns_true_and_list_with_single_item_for_array_claim_with_one_item()
   {
      var testSubject = CreateSerializer();

      var claimsObject = new JsonObject
      {
         ["arbitrary-claim"] = new JsonArray { "arbitrary-value" },
      };

      var result = testSubject.TryDeserialize(claimsObject, out var claims);

      Assert.That(result, Is.True);
      Assert.That(claims.Length, Is.EqualTo(1));
      Assert.That(claims[0].Type, Is.EqualTo("arbitrary-claim"));
      Assert.That(claims[0].Value, Is.EqualTo("[\"arbitrary-value\"]"));
      Assert.That(claims[0].ValueType, Is.EqualTo("JSON_ARRAY"));
   }

   [Test]
   public void try_deserialize_returns_true_and_list_with_single_item_for_array_claim_with_two_items()
   {
      var testSubject = CreateSerializer();

      var claimsObject = new JsonObject
      {
         ["arbitrary-claim"] = new JsonArray { "arbitrary-value-one", "arbitrary-value-two" },
      };

      var result = testSubject.TryDeserialize(claimsObject, out var claims);

      Assert.That(result, Is.True);
      Assert.That(claims.Length, Is.EqualTo(1));
      Assert.That(claims[0].Type, Is.EqualTo("arbitrary-claim"));
      Assert.That(claims[0].Value, Is.EqualTo("[\"arbitrary-value-one\",\"arbitrary-value-two\"]"));
      Assert.That(claims[0].ValueType, Is.EqualTo("JSON_ARRAY"));
   }

   [TestCase(1379)]
   [TestCase(true)]
   [TestCase(null)]
   public void try_deserialize_returns_false_and_empty_list_for_array_claim_with_non_string_literal(object? value)
   {
      var testSubject = CreateSerializer();

      var claimsObject = new JsonObject
      {
         ["valid-claim"] = new JsonArray { "valid-value", value },
      };

      var result = testSubject.TryDeserialize(claimsObject, out var claims);

      Assert.That(result, Is.False);
      Assert.That(claims, Is.Empty);
   }

   [TestCase("NAME", "name")]
   [TestCase("Name", "name")]
   [TestCase("NICKNAME", "nickname")]
   [TestCase("Nickname", "nickname")]
   [TestCase("PICTURE", "picture")]
   [TestCase("Picture", "picture")]
   public void try_deserialize_normalises_known_claim_name(string name, string expectedName)
   {
      var testSubject = CreateSerializer();

      var claimsObject = new JsonObject
      {
         [name] = "arbitrary-value",
      };

      var result = testSubject.TryDeserialize(claimsObject, out var claims);

      var expectedClaims = new Claim[]
      {
         new(expectedName, "arbitrary-value"),
      };

      Assert.That(result, Is.True);
      Assert.That(claims.Length, Is.EqualTo(1));

      claims.ToArray().ShouldBeEquivalentTo(expectedClaims);
   }

   [TestCase("UPPER")]
   [TestCase("lower")]
   [TestCase("Capitalized")]
   public void try_deserialize_does_not_normalise_unknown_claim_name(string name)
   {
      var testSubject = CreateSerializer();

      var claimsObject = new JsonObject
      {
         [name] = "arbitrary-value",
      };

      var result = testSubject.TryDeserialize(claimsObject, out var claims);

      var expectedClaims = new Claim[]
      {
         new(name, "arbitrary-value"),
      };

      Assert.That(result, Is.True);
      Assert.That(claims.Length, Is.EqualTo(1));

      claims.ToArray().ShouldBeEquivalentTo(expectedClaims);
   }

   [Test]
   public void serialize_handles_string_claim()
   {
      var testSubject = CreateSerializer();

      var result = testSubject.Serialize([new Claim("arbitrary-name", "arbitrary-value")]);

      Assert.That(result.ToJsonString(), Is.EqualTo("{\"arbitrary-name\":\"arbitrary-value\"}"));
   }

   [Test]
   public void serialize_handles_string_array_claim()
   {
      var testSubject = CreateSerializer();

      var result = testSubject.Serialize([new Claim("arbitrary-name", "[\"arbitrary-value-a\",\"arbitrary-value-b\"]", JsonClaimValueTypes.JsonArray)]);

      Assert.That(result.ToJsonString(), Is.EqualTo("{\"arbitrary-name\":[\"arbitrary-value-a\",\"arbitrary-value-b\"]}"));
   }

   private static ClaimsSerializer CreateSerializer(
      ILogger<ClaimsSerializer>? logger = null)
   {
      logger ??= NullLogger<ClaimsSerializer>.Instance;

      return new ClaimsSerializer(logger);
   }
}
