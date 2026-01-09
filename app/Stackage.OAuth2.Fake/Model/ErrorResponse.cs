namespace Stackage.OAuth2.Fake.Model;

using System.Text.Json.Serialization;

public record ErrorResponse
{
   [JsonPropertyName("error")]
   public required string Error { get; init; }

   [JsonPropertyName("error_description")]
   public required string Description { get; init; }
}
