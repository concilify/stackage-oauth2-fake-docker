namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.OAuth2.Authorization;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using NUnit.Framework;

// ReSharper disable once InconsistentNaming
public class user_authorization_happy_path
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
         $"{openIdConfigurationResponse.AuthorizationEndpoint}?response_type=code&state=AnyState&redirect_uri=http://any-host/callback";

      _httpResponse = await httpClient.GetAsync(authorizationUri);
   }

   [Test]
   public void response_status_should_be_redirect()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.Found));
   }

   [Test]
   public void response_headers_should_contain_location_with_code_and_state_query_parameters()
   {
      Assert.That(_httpResponse?.Headers.Location, Is.Not.Null);
      Assert.That(_httpResponse?.Headers.Location?.Host, Is.EqualTo("any-host"));
      Assert.That(_httpResponse?.Headers.Location?.AbsolutePath, Is.EqualTo("/callback"));
      Assert.That(_httpResponse?.Headers.Location?.Query, Is.Not.Null);

      var queryString = HttpUtility.ParseQueryString(_httpResponse!.Headers.Location!.Query!);

      Assert.That(queryString.Keys, Contains.Item("code"));
      Assert.That(queryString.Keys, Contains.Item("state"));

      Assert.That(Guid.TryParse(queryString["code"], out _), Is.True);
      Assert.That(queryString["state"], Is.EqualTo("AnyState"));
   }
}
