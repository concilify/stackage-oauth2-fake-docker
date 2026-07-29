namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.OAuth2.Authorization;

using System;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using NUnit.Framework;

// ReSharper disable once InconsistentNaming
public class user_authorization_happy_path_with_user_b_login_hint
{
   private string? _authorizationCode;
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
         $"{openIdConfigurationResponse.AuthorizationEndpoint}?response_type=code&client_id=ValidClientId&state=ArbitraryState&redirect_uri=http://arbitrary-host/callback&login_hint=user-b@example.com";

      _httpResponse = await httpClient.GetAsync(authorizationUri);

      var queryString = HttpUtility.ParseQueryString(_httpResponse!.Headers.Location!.Query!);
      _authorizationCode = queryString["code"];
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
      Assert.That(_httpResponse?.Headers.Location?.Host, Is.EqualTo("arbitrary-host"));
      Assert.That(_httpResponse?.Headers.Location?.AbsolutePath, Is.EqualTo("/callback"));
      Assert.That(_httpResponse?.Headers.Location?.Query, Is.Not.Null);

      var queryString = HttpUtility.ParseQueryString(_httpResponse!.Headers.Location!.Query!);

      Assert.That(queryString.Keys, Contains.Item("code"));
      Assert.That(queryString.Keys, Contains.Item("state"));

      Assert.That(Guid.TryParse(queryString["code"], out _), Is.True);
      Assert.That(queryString["state"], Is.EqualTo("ArbitraryState"));
   }

   [Test]
   public async Task response_should_seed_internal_authorization_with_user_b_subject()
   {
      using var httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);
      Assert.That(_authorizationCode, Is.Not.Null);

      var httpResponse = await httpClient.GetAsync($".internal/user-authorization?code={_authorizationCode}");
      var authorizationResponse = await httpResponse.ParseAsync<AuthorizationResponse>();

      Assert.That(authorizationResponse.Subject, Is.EqualTo("user-b-subject"));
   }

   private record AuthorizationResponse([property: JsonPropertyName("subject")] string Subject);
}
