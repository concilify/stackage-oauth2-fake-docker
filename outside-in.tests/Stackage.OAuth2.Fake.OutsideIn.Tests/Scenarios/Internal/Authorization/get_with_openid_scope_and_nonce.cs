namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Internal.Authorization;

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NUnit.Framework;

// ReSharper disable once InconsistentNaming
public class get_with_openid_scope_and_nonce
{
   private string? _code;
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      _code = Guid.NewGuid().ToString();

      var body = new
      {
         code = _code,
         scopes = new[] { "openid", "arbitrary-scope" },
         clientId = "ArbitraryClientId",
         subject = "ArbitrarySubject",
         nonce = "ArbitraryNonce",
      };

      var content = JsonContent.Create(body);

      await httpClient.PostAsync(".internal/user-authorization", content);

      _httpResponse = await httpClient.GetAsync($".internal/user-authorization?code={_code}");
   }

   [Test]
   public void response_status_should_be_okay()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.OK));
   }

   [Test]
   public async Task response_content_should_contain_nonce()
   {
      var authorizationResponse = await _httpResponse!.ParseAsync<AuthorizationResponse>();

      Assert.That(authorizationResponse.Nonce, Is.EqualTo("ArbitraryNonce"));
   }

   private record AuthorizationResponse(
      [property: JsonPropertyName("nonce")] string? Nonce);
}
