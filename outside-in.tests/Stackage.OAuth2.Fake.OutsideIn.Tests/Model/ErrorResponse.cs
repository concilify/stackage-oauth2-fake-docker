namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Model;

using System.Text.Json.Serialization;

public record ErrorResponse(
   [property: JsonPropertyName("error")] string Error,
   [property: JsonPropertyName("error_description")] string ErrorDescription);
