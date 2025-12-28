namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.OAuth2.Authorization;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using NUnit.Framework;

// ReSharper disable once InconsistentNaming
public class user_authorization_missing_response_type
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
         $"{openIdConfigurationResponse.AuthorizationEndpoint}?client_id=AnyClientId&state=AnyState&redirect_uri=http://any-host/callback";

      _httpResponse = await httpClient.GetAsync(authorizationUri);
   }

   [Test]
   public void response_status_should_be_redirect()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.Found));
   }

   [Test]
   public void response_should_contain_error_parameter()
   {
      var queryString = HttpUtility.ParseQueryString(_httpResponse!.Headers.Location!.Query!);

      Assert.That(queryString["error"], Is.EqualTo("invalid_request"));
      Assert.That(queryString["state"], Is.EqualTo("AnyState"));
   }
}
