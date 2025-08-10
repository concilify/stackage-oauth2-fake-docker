namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Internal.History;

using System;
using System.Net;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using NUnit.Framework;

// ReSharper disable once InconsistentNaming
public class get_without_requests
{
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      await httpClient.DeleteAsync(".internal/history");

      _httpResponse = await httpClient.GetAsync(".internal/history/requests");
   }

   [Test]
   public void response_status_should_be_okay()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.OK));
   }

   [Test]
   public async Task response_content_should_be_an_empty_array()
   {
      var requests = await _httpResponse!.ParseAsync<JsonArray>();

      Assert.That(requests, Is.Empty);
   }
}
