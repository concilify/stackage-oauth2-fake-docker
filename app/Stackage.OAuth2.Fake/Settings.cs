namespace Stackage.OAuth2.Fake;

using Microsoft.Extensions.Configuration;

public class Settings
{
   [ConfigurationKeyName("ISSUER_URL")]
   public string IssuerUrl { get; init; } = string.Empty;

   [ConfigurationKeyName("TOKEN_PATH")]
   public string TokenPath { get; init; } = string.Empty;

   [ConfigurationKeyName("AUTHORIZATION_PATH")]
   public string AuthorizationPath { get; init; } = string.Empty;

   [ConfigurationKeyName("DEVICE_AUTHORIZATION_PATH")]
   public string DeviceAuthorizationPath { get; init; } = string.Empty;

   [ConfigurationKeyName("DEVICE_VERIFICATION_PATH")]
   public string DeviceVerificationPath { get; init; } = string.Empty;

   [ConfigurationKeyName("DEFAULT_SUBJECT")]
   public string DefaultSubject { get; init; } = string.Empty;
}
