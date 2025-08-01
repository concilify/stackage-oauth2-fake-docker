namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Model;

using System.Text.Json.Serialization;

public record TokenResponse(
   [property: JsonPropertyName("access_token")] string AccessToken,
   [property: JsonPropertyName("id_token")] string? IdToken,
   [property: JsonPropertyName("refresh_token")] string? RefreshToken,
   [property: JsonPropertyName("scope")] string? Scope,
   [property: JsonPropertyName("token_type")] string TokenType,
   [property: JsonPropertyName("expires_in")] int ExpiresIn);
