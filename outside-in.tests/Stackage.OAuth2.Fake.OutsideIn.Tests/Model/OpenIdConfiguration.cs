namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Model;

using System.Text.Json.Serialization;

public record OpenIdConfiguration(
   [property:JsonPropertyName("issuer")]string Issuer,
   [property:JsonPropertyName("device_authorization_endpoint")]string DeviceAuthorizationEndpoint);
