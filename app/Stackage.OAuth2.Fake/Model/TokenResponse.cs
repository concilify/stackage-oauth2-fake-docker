namespace Stackage.OAuth2.Fake.Model;

using System.Text.Json.Serialization;

public record TokenResponse
{
   [JsonPropertyName("access_token")]
   public required string AccessToken { get; init; }

   [JsonPropertyName("id_token")]
   public string? IdToken { get; init; }

   [JsonPropertyName("refresh_token")]
   public string? RefreshToken { get; init; }

   [JsonPropertyName("scope")]
   public string? Scope { get; init; }

   [JsonPropertyName("expires_in")]
   public required int ExpiresInSeconds { get; init; }

   [JsonPropertyName("token_type")]
   public string TokenType => "Bearer";
}
