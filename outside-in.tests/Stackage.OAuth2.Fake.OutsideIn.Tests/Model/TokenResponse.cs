namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Model;

using System.Text.Json.Serialization;

public record TokenResponse(
   [property: JsonPropertyName("access_token")] string AccessToken,
   [property: JsonPropertyName("token_type")] string TokenType,
   [property: JsonPropertyName("expires_in")] int ExpiresIn);
