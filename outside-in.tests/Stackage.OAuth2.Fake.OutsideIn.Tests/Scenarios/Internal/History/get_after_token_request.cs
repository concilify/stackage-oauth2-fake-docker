namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Internal.History;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Shouldly;
using NUnit.Framework;

// ReSharper disable once InconsistentNaming
public class get_after_token_request
{
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      await httpClient.DeleteAsync(".internal/history");

      var openIdConfigurationResponse = await httpClient.GetWellKnownOpenIdConfigurationAsync();

      await httpClient.SeedAuthorizationAsync("the-code");

      await httpClient.ExchangeAuthorizationCodeAsync(
         openIdConfigurationResponse,
         "the-code");

      _httpResponse = await httpClient.GetAsync(".internal/history/requests");
   }

   [Test]
   public void response_status_should_be_okay()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.OK));
   }

   [Test]
   public async Task response_content_should_contain_two_items()
   {
      var requests = await _httpResponse!.ParseAsync<JsonArray>();

      Assert.That(requests.Count, Is.EqualTo(2));
   }

   [Test]
   public async Task response_content_should_contain_token_endpoint_as_most_recent()
   {
      var requests = await _httpResponse!.ParseAsync<JsonArray>();

      Assert.That(requests[0], Is.Not.Null);
      Assert.That(requests[0]!["method"]?.GetValue<string>(), Is.EqualTo("POST"));
      Assert.That(requests[0]!["path"]?.GetValue<string>(), Is.EqualTo("/oauth2/token"));

      var expectedTokenExchange = new Dictionary<string, string>
      {
         ["client_id"] = "AnyClientId",
         ["grant_type"] = "authorization_code",
         ["code"] = "the-code",
      };

      var queryString = requests[0]!.ParseQueryString();

      queryString.ShouldBeEquivalentTo(expectedTokenExchange);
   }

   [Test]
   public async Task response_content_should_contain_well_known_endpoint_as_least_recent()
   {
      var requests = await _httpResponse!.ParseAsync<JsonArray>();

      Assert.That(requests[1], Is.Not.Null);
      Assert.That(requests[1]!["method"]?.GetValue<string>(), Is.EqualTo("GET"));
      Assert.That(requests[1]!["path"]?.GetValue<string>(), Is.EqualTo("/.well-known/openid-configuration"));
      Assert.That(requests[1]!["bodyBase64"]?.GetValue<string>(), Is.Empty);
   }
}
