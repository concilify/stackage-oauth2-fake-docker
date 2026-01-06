namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Logout;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

// ReSharper disable once InconsistentNaming
public class logout_missing_return_to
{
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var handler = new HttpClientHandler();
      handler.AllowAutoRedirect = false;

      var httpClient = new HttpClient(handler);
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      var logoutUri = "/logout?client_id=ValidClientId";

      _httpResponse = await httpClient.GetAsync(logoutUri);
   }

   [Test]
   public void response_status_should_be_bad_request()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
   }
}
