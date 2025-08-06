namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Internal.RefreshToken.Model;

using System.Text.Json.Serialization;

public record RefreshTokenResponse(
   [property: JsonPropertyName("refresh_token")] string RefreshToken,
   [property: JsonPropertyName("scopes")] string[] Scopes,
   [property: JsonPropertyName("subject")] string Subject);
