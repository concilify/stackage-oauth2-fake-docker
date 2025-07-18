namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.WellKnown;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

public class OpenIdConfigurationEndpoint
{
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      _httpResponse = await httpClient.GetAsync(".well-known/openid-configuration");
   }

   [Test]
   public void response_status_should_be_okay()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.OK));
   }

   [Test]
   public async Task response_content_should_contain_issuer()
   {
      var content = await _httpResponse!.ParseContentAsJson();

      Assert.That(content["issuer"]?.GetValue<string>(), Is.EqualTo(Configuration.IssuerUrl));
   }
}
