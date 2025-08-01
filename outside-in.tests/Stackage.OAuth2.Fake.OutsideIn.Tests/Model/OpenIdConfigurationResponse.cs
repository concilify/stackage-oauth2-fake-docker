namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Model;

using System.Text.Json.Serialization;

public record OpenIdConfigurationResponse(
   [property: JsonPropertyName("issuer")] string Issuer,
   [property: JsonPropertyName("jwks_uri")] string JwksUri,
   [property: JsonPropertyName("token_endpoint")] string TokenEndpoint,
   [property: JsonPropertyName("authorization_endpoint")] string AuthorizationEndpoint,
   [property: JsonPropertyName("device_authorization_endpoint")] string DeviceAuthorizationEndpoint,
   [property: JsonPropertyName("grant_types_supported")] string[] GrantTypesSupported);
