namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.WellKnown;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Stackage.OAuth2.Fake.OutsideIn.Tests.Model;

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
      var openIdConfigurationResponse = await _httpResponse!.ParseAsync<OpenIdConfigurationResponse>();

      Assert.That(openIdConfigurationResponse.Issuer, Is.EqualTo(Configuration.IssuerUrl));
   }

   [Test]
   public async Task response_content_should_contain_jwks_uri()
   {
      var openIdConfigurationResponse = await _httpResponse!.ParseAsync<OpenIdConfigurationResponse>();

      Assert.That(
         openIdConfigurationResponse.JwksUri,
         Is.EqualTo($"{Configuration.IssuerUrl}/.well-known/jwks.json"));
   }

   [Test]
   public async Task response_content_should_contain_token_endpoint()
   {
      var openIdConfigurationResponse = await _httpResponse!.ParseAsync<OpenIdConfigurationResponse>();

      Assert.That(
         openIdConfigurationResponse.TokenEndpoint,
         Is.EqualTo($"{Configuration.IssuerUrl}/oauth2/token"));
   }

   [Test]
   public async Task response_content_should_contain_authorization_endpoint()
   {
      var openIdConfigurationResponse = await _httpResponse!.ParseAsync<OpenIdConfigurationResponse>();

      Assert.That(
         openIdConfigurationResponse.AuthorizationEndpoint,
         Is.EqualTo($"{Configuration.IssuerUrl}/oauth2/authorize"));
   }

   [Test]
   public async Task response_content_should_contain_device_authorization_endpoint()
   {
      var openIdConfigurationResponse = await _httpResponse!.ParseAsync<OpenIdConfigurationResponse>();

      Assert.That(
         openIdConfigurationResponse.DeviceAuthorizationEndpoint,
         Is.EqualTo($"{Configuration.IssuerUrl}/oauth2/device/authorize"));
   }

   [Test]
   public async Task response_content_should_contain_grant_types_supported()
   {
      var openIdConfigurationResponse = await _httpResponse!.ParseAsync<OpenIdConfigurationResponse>();

      Assert.That(
         openIdConfigurationResponse.GrantTypesSupported,
         Is.EqualTo(["urn:ietf:params:oauth:grant-type:device_code"]));
   }
}
