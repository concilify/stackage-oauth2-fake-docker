namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.OAuth2.Authorization;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

// ReSharper disable once InconsistentNaming
public class user_authorization_missing_redirect_uri
{
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var handler = new HttpClientHandler();
      handler.AllowAutoRedirect = false;

      var httpClient = new HttpClient(handler);
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      var openIdConfigurationResponse = await httpClient.GetWellKnownOpenIdConfigurationAsync();

      var authorizationUri =
         $"{openIdConfigurationResponse.AuthorizationEndpoint}?response_type=code&client_id=AnyClientId&state=AnyState";

      _httpResponse = await httpClient.GetAsync(authorizationUri);
   }

   [Test]
   public void response_status_should_be_bad_request()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
   }
}
