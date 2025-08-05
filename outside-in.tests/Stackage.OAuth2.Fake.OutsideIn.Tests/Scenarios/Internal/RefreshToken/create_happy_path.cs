namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Internal.RefreshToken;

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NUnit.Framework;

// ReSharper disable once InconsistentNaming
public class create_happy_path
{
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      var body = new
      {
         refreshToken = "SomeRefreshToken"
      };

      var content = JsonContent.Create(body);

      _httpResponse = await httpClient.PostAsync(".internal/refresh-token", content);
   }

   [Test]
   public void response_status_should_be_okay()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.OK));
   }
}
