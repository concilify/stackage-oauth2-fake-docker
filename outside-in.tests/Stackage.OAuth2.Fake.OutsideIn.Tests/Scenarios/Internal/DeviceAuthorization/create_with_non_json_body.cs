namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Internal.DeviceAuthorization;

using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Stackage.OAuth2.Fake.OutsideIn.Tests.Model;

// ReSharper disable once InconsistentNaming
public class create_with_non_json_body
{
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      var content = new StringContent("NonEmptyBody", Encoding.UTF8, "text/plain");

      _httpResponse = await httpClient.PostAsync(".internal/device-authorization", content);
   }

   [Test]
   public void response_status_should_be_unsupported_media_type()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.UnsupportedMediaType));
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

      Assert.That(errorResponse.ErrorDescription, Is.EqualTo("The Content-Type header must be application/json"));
   }
}
