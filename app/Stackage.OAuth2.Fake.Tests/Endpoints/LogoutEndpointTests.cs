namespace Stackage.OAuth2.Fake.Tests.Endpoints;

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using NUnit.Framework;

public class LogoutEndpointTests
{
   [TestCase("logout")]
   [TestCase("alternate/logout")]
   public async Task logout_path_can_be_varied(string path)
   {
      var factory = new OAuth2FakeWebApplicationFactory();

      factory.Settings = factory.Settings with { LogoutPath = $"/{path}" };
      factory.ClientOptions.AllowAutoRedirect = false;

      var httpClient = factory.CreateClient();

      var requestQuery = new Dictionary<string, string?>
      {
         ["client_id"] = "ValidClientId",
         ["returnTo"] = "http://arbitrary-host/logged-out",
      };

      var httpResponse = await httpClient.GetAsync(QueryHelpers.AddQueryString($"/{path}", requestQuery));

      Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));

      Assert.That(httpResponse.Headers.Location?.AbsoluteUri, Is.EqualTo("http://arbitrary-host/logged-out"));
   }

   [Test]
   public async Task logout_returns_error_when_returnTo_is_missing()
   {
      var factory = new OAuth2FakeWebApplicationFactory();
      factory.ClientOptions.AllowAutoRedirect = false;

      var httpClient = factory.CreateClient();

      var requestQuery = new Dictionary<string, string?>
      {
         ["client_id"] = "ValidClientId",
      };

      var httpResponse = await httpClient.GetAsync(QueryHelpers.AddQueryString("/logout", requestQuery));

      Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
   }

   [Test]
   public async Task logout_redirects_when_returnTo_is_provided()
   {
      var factory = new OAuth2FakeWebApplicationFactory();
      factory.ClientOptions.AllowAutoRedirect = false;

      var httpClient = factory.CreateClient();

      var requestQuery = new Dictionary<string, string?>
      {
         ["client_id"] = "ValidClientId",
         ["returnTo"] = "http://valid-host/callback",
      };

      var httpResponse = await httpClient.GetAsync(QueryHelpers.AddQueryString("/logout", requestQuery));

      Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
      Assert.That(httpResponse.Headers.Location?.AbsoluteUri, Is.EqualTo("http://valid-host/callback"));
   }

   [Test]
   public async Task logout_redirects_without_client_id()
   {
      var factory = new OAuth2FakeWebApplicationFactory();
      factory.ClientOptions.AllowAutoRedirect = false;

      var httpClient = factory.CreateClient();

      var requestQuery = new Dictionary<string, string?>
      {
         ["returnTo"] = "http://valid-host/callback",
      };

      var httpResponse = await httpClient.GetAsync(QueryHelpers.AddQueryString("/logout", requestQuery));

      Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
      Assert.That(httpResponse.Headers.Location?.AbsoluteUri, Is.EqualTo("http://valid-host/callback"));
   }
}
