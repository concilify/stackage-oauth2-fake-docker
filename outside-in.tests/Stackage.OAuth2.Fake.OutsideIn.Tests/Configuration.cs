namespace Stackage.OAuth2.Fake.OutsideIn.Tests;

using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

public static class Configuration
{
   private static readonly IConfiguration Instance = GetConfiguration();

   public static readonly string AppUri = Instance["AppUri"] ?? string.Empty;

   public static readonly string IssuerUri = Instance["IssuerUri"] ?? string.Empty;

   private static IConfiguration GetConfiguration()
   {
      var configuration = new ConfigurationBuilder()
         .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".")
         .AddJsonFile("appsettings.json", optional: true)
         .AddEnvironmentVariables("STACKAGEOAUTH2FAKETESTS_")
         .Build();

      return configuration;
   }
}
