namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Internal.DeviceAuthorization;

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NUnit.Framework;

// ReSharper disable once InconsistentNaming
public class get_happy_path
{
   private string? _deviceCode;
   private string? _userCode;
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      _deviceCode = Guid.NewGuid().ToString();
      _userCode = Guid.NewGuid().ToString();

      var body = new
      {
         deviceCode = _deviceCode,
         userCode = _userCode,
         scopes = new[] { "arbitrary-scope-a", "arbitrary-scope-b" },
         clientId = "ArbitraryClientId",
         subject = "ArbitrarySubject",
      };

      var content = JsonContent.Create(body);

      await httpClient.PostAsync(".internal/device-authorization", content);

      _httpResponse = await httpClient.GetAsync($".internal/device-authorization?deviceCode={_deviceCode}");
   }

   [Test]
   public void response_status_should_be_okay()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.OK));
   }

   [Test]
   public async Task response_content_should_contain_device_code()
   {
      var authorizationResponse = await _httpResponse!.ParseAsync<AuthorizationResponse>();

      Assert.That(authorizationResponse.DeviceCode, Is.EqualTo(_deviceCode));
   }

   [Test]
   public async Task response_content_should_contain_user_code()
   {
      var authorizationResponse = await _httpResponse!.ParseAsync<AuthorizationResponse>();

      Assert.That(authorizationResponse.UserCode, Is.EqualTo(_userCode));
   }

   [Test]
   public async Task response_content_should_contain_scopes()
   {
      var authorizationResponse = await _httpResponse!.ParseAsync<AuthorizationResponse>();

      Assert.That(authorizationResponse.Scopes, Is.EqualTo(["arbitrary-scope-a", "arbitrary-scope-b"]));
   }

   [Test]
   public async Task response_content_should_contain_client_id()
   {
      var authorizationResponse = await _httpResponse!.ParseAsync<AuthorizationResponse>();

      Assert.That(authorizationResponse.ClientId, Is.EqualTo("ArbitraryClientId"));
   }

   [Test]
   public async Task response_content_should_contain_subject()
   {
      var authorizationResponse = await _httpResponse!.ParseAsync<AuthorizationResponse>();

      Assert.That(authorizationResponse.Subject, Is.EqualTo("ArbitrarySubject"));
   }

   private record AuthorizationResponse(
      [property: JsonPropertyName("deviceCode")] string DeviceCode,
      [property: JsonPropertyName("userCode")] string UserCode,
      [property: JsonPropertyName("scopes")] string[] Scopes,
      [property: JsonPropertyName("clientId")] string ClientId,
      [property: JsonPropertyName("subject")] string Subject);
}
