namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Logout;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

// ReSharper disable once InconsistentNaming
public class logout_happy_path
{
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var handler = new HttpClientHandler();
      handler.AllowAutoRedirect = false;

      var httpClient = new HttpClient(handler);
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      var logoutUri = $"/logout?client_id=ValidClientId&returnTo=http%3A%2F%2Flocalhost%3A9002";

      _httpResponse = await httpClient.GetAsync(logoutUri);
   }

   [Test]
   public void response_status_should_be_redirect()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.Found));
   }

   [Test]
   public void response_headers_should_contain_location_with_returnTo_url()
   {
      Assert.That(_httpResponse?.Headers.Location, Is.Not.Null);
      Assert.That(_httpResponse?.Headers.Location?.AbsoluteUri, Is.EqualTo("http://localhost:9002/"));
   }
}
