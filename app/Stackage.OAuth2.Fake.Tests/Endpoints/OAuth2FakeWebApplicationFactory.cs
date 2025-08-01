namespace Stackage.OAuth2.Fake.Tests.Endpoints;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

internal class OAuth2FakeWebApplicationFactory : WebApplicationFactory<Program>
{
   public Settings Settings { get; set; } = new Settings();

   protected override void ConfigureWebHost(IWebHostBuilder builder)
   {
      builder.ConfigureTestServices(services =>
      {
         services.AddSingleton(Settings);
      });
   }
}
