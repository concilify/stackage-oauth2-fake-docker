namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Internal.RefreshToken;

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Stackage.OAuth2.Fake.OutsideIn.Tests.Model;

// ReSharper disable once InconsistentNaming
public class create_without_refresh_token
{
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      var content = JsonContent.Create(new { });

      _httpResponse = await httpClient.PostAsync(".internal/refresh-token", content);
   }

   [Test]
   public void response_status_should_be_bad_request()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
   }

   [Test]
   public async Task response_content_should_contain_error()
   {
      var errorResponse = await _httpResponse!.ParseAsync<ErrorResponse>();

      Assert.That(errorResponse.Error, Is.EqualTo("invalid_request"));
   }

   [Test]
   public async Task response_content_should_contain_error_description()
   {
      var errorResponse = await _httpResponse!.ParseAsync<ErrorResponse>();

      Assert.That(errorResponse.ErrorDescription, Is.EqualTo("The refreshToken property was missing"));
   }
}
