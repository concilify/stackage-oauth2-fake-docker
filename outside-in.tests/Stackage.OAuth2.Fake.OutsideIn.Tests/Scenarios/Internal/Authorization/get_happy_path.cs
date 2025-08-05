namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Internal.Authorization;

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
         scopes = new[] { "scope-a", "scope-b" },
         subject = "SomeSubject"
      };

      var content = JsonContent.Create(body);

      await httpClient.PostAsync(".internal/authorization", content);

      _httpResponse = await httpClient.GetAsync($".internal/authorization?code={_code}");
   }

   [Test]
   public void response_status_should_be_okay()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.OK));
   }

   [Test]
   public async Task response_content_should_contain_code()
   {
      var authorizationResponse = await _httpResponse!.ParseAsync<AuthorizationResponse>();

      Assert.That(authorizationResponse.Code, Is.EqualTo(_code));
   }

   [Test]
   public async Task response_content_should_contain_scopes()
   {
      var authorizationResponse = await _httpResponse!.ParseAsync<AuthorizationResponse>();

      Assert.That(authorizationResponse.Scopes, Is.EqualTo(["scope-a", "scope-b"]));
   }

   [Test]
   public async Task response_content_should_contain_subject()
   {
      var authorizationResponse = await _httpResponse!.ParseAsync<AuthorizationResponse>();

      Assert.That(authorizationResponse.Subject, Is.EqualTo("SomeSubject"));
   }

   private record AuthorizationResponse(
      [property: JsonPropertyName("code")] string Code,
      [property: JsonPropertyName("scopes")] string[] Scopes,
      [property: JsonPropertyName("subject")] string Subject);
}
