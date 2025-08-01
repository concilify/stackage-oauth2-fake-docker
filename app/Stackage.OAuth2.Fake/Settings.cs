namespace Stackage.OAuth2.Fake;

using Microsoft.Extensions.Configuration;

public record Settings
{
   [ConfigurationKeyName("ISSUER_URL")]
   public string IssuerUrl { get; init; } = string.Empty;

   [ConfigurationKeyName("TOKEN_PATH")]
   public string TokenPath { get; init; } = "/oauth2/token";

   [ConfigurationKeyName("AUTHORIZATION_PATH")]
   public string AuthorizationPath { get; init; } = "/oauth2/authorize";

   [ConfigurationKeyName("DEVICE_AUTHORIZATION_PATH")]
   public string DeviceAuthorizationPath { get; init; } = "/oauth2/device/authorize";

   [ConfigurationKeyName("DEVICE_VERIFICATION_PATH")]
   public string DeviceVerificationPath { get; init; } = "/oauth2/device/verify";

   [ConfigurationKeyName("DEFAULT_SUBJECT")]
   public string DefaultSubject { get; init; } = string.Empty;
}
