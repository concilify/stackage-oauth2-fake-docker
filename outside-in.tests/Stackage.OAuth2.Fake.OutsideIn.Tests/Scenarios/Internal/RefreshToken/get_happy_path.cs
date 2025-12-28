namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Internal.RefreshToken;

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Internal.RefreshToken.Model;

// ReSharper disable once InconsistentNaming
public class get_happy_path
{
   private string? _refreshToken;
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      _refreshToken = Guid.NewGuid().ToString();

      var body = new
      {
         refreshToken = _refreshToken,
         scopes = new[] { "scope-a", "scope-b" },
         subject = "SomeSubject"
      };

      var content = JsonContent.Create(body);

      await httpClient.PostAsync(".internal/refresh-token", content);

      _httpResponse = await httpClient.GetAsync($".internal/refresh-token?refresh_token={_refreshToken}");
   }

   [Test]
   public void response_status_should_be_okay()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.OK));
   }

   [Test]
   public async Task response_content_should_contain_refresh_token()
   {
      var refreshTokenResponse = await _httpResponse!.ParseAsync<RefreshTokenResponse>();

      Assert.That(refreshTokenResponse.RefreshToken, Is.EqualTo(_refreshToken));
   }

   [Test]
   public async Task response_content_should_contain_scopes()
   {
      var refreshTokenResponse = await _httpResponse!.ParseAsync<RefreshTokenResponse>();

      Assert.That(refreshTokenResponse.Scopes, Is.EqualTo(["scope-a", "scope-b"]));
   }

   [Test]
   public async Task response_content_should_contain_subject()
   {
      var refreshTokenResponse = await _httpResponse!.ParseAsync<RefreshTokenResponse>();

      Assert.That(refreshTokenResponse.Subject, Is.EqualTo("SomeSubject"));
   }
}
