namespace Stackage.OAuth2.Fake;

using Microsoft.Extensions.Configuration;

public class Configuration
{
   [ConfigurationKeyName("ISSUER_URL")]
   public string IssuerUrl { get; set; } = string.Empty;
}
