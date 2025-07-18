namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Model;

using System.Text.Json.Serialization;

public record DeviceAuthorizeResponse(
   [property:JsonPropertyName("device_code")]string DeviceCode,
   [property:JsonPropertyName("user_code")]string UserCode,
   [property:JsonPropertyName("verification_uri")]string VerificationUri,
   [property:JsonPropertyName("verification_uri_complete")]string VerificationUriComplete,
   [property:JsonPropertyName("expires_in")]int ExpiresIn,
   [property:JsonPropertyName("interval")]int Interval);
