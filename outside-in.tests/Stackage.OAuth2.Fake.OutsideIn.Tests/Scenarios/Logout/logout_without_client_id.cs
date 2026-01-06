namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Logout;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

// ReSharper disable once InconsistentNaming
public class logout_without_client_id
{
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var handler = new HttpClientHandler();
      handler.AllowAutoRedirect = false;

      var httpClient = new HttpClient(handler);
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      const string logoutUri = "/logout?returnTo=http%3A%2F%2Farbitrary-host%3A9002";

      _httpResponse = await httpClient.GetAsync(logoutUri);
   }

   [Test]
   public void response_status_should_be_redirect()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.Found));
   }

   [Test]
   public void response_headers_should_contain_location_with_return_to_url()
   {
      Assert.That(_httpResponse?.Headers.Location, Is.Not.Null);
      Assert.That(_httpResponse?.Headers.Location?.AbsoluteUri, Is.EqualTo("http://arbitrary-host:9002/"));
   }
}
