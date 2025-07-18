namespace Stackage.OAuth2.Fake;

using Microsoft.Extensions.Configuration;

public class Settings
{
   [ConfigurationKeyName("ISSUER_URL")]
   public string IssuerUrl { get; init; } = string.Empty;
}
