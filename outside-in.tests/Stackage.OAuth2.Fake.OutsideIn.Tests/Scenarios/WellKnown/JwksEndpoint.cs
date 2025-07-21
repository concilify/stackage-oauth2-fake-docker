namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.WellKnown;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;

public class JwksEndpoint
{
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      var openIdConfigurationResponse = await httpClient.GetWellKnownOpenIdConfigurationAsync();

      _httpResponse = await httpClient.GetAsync(openIdConfigurationResponse.JwksUri);
   }

   [Test]
   public void response_status_should_be_okay()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.OK));
   }

   [Test]
   public async Task response_content_should_contain_json_web_key_set_with_one_item()
   {
      var jwksResponse = await _httpResponse!.ParseAsync<JsonWebKeySet>();

      Assert.That(jwksResponse.Keys.Count, Is.EqualTo(1));
   }

   [Test]
   public async Task response_content_should_contain_json_web_key_with_alg()
   {
      var jwksResponse = await _httpResponse!.ParseAsync<JsonWebKeySet>();

      Assert.That(jwksResponse.Keys[0].Alg, Is.EqualTo("RS256"));
   }

   [Test]
   public async Task response_content_should_contain_json_web_key_with_use()
   {
      var jwksResponse = await _httpResponse!.ParseAsync<JsonWebKeySet>();

      Assert.That(jwksResponse.Keys[0].Use, Is.EqualTo("sig"));
   }

   [Test]
   public async Task response_content_should_contain_json_web_key_with_kty()
   {
      var jwksResponse = await _httpResponse!.ParseAsync<JsonWebKeySet>();

      Assert.That(jwksResponse.Keys[0].Kty, Is.EqualTo("RSA"));
   }
}
