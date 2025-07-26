namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.OAuth2.Authorization;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Stackage.OAuth2.Fake.OutsideIn.Tests.Model;

// ReSharper disable once InconsistentNaming
public class device_authorization_happy_path
{
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      var openIdConfigurationResponse = await httpClient.GetWellKnownOpenIdConfigurationAsync();

      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
         ["client_id"] = "AnyClientId",
      });

      _httpResponse = await httpClient.PostAsync(
         openIdConfigurationResponse.DeviceAuthorizationEndpoint,
         content);
   }

   [Test]
   public void response_status_should_be_okay()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.OK));
   }

   [Test]
   public async Task response_content_should_contain_device_code()
   {
      var deviceAuthorizeResponse = await _httpResponse!.ParseAsync<DeviceAuthorizeResponse>();

      Assert.That(Guid.TryParse(deviceAuthorizeResponse.DeviceCode, out _), Is.True);
   }

   [Test]
   public async Task response_content_should_contain_user_code()
   {
      var deviceAuthorizeResponse = await _httpResponse!.ParseAsync<DeviceAuthorizeResponse>();

      Assert.That(deviceAuthorizeResponse.UserCode.Length, Is.EqualTo(4));
   }

   [Test]
   public async Task response_content_should_contain_verification_uri()
   {
      var deviceAuthorizeResponse = await _httpResponse!.ParseAsync<DeviceAuthorizeResponse>();

      Assert.That(
         deviceAuthorizeResponse.VerificationUri,
         Is.EqualTo($"{Configuration.IssuerUrl}/oauth2/device/verify"));
   }

   [Test]
   public async Task response_content_should_contain_verification_uri_complete()
   {
      var deviceAuthorizeResponse = await _httpResponse!.ParseAsync<DeviceAuthorizeResponse>();

      Assert.That(
         deviceAuthorizeResponse.VerificationUriComplete,
         Is.EqualTo($"{Configuration.IssuerUrl}/oauth2/device/verify?user_code={deviceAuthorizeResponse.UserCode}"));
   }

   [Test]
   public async Task response_content_should_contain_expires_in()
   {
      var deviceAuthorizeResponse = await _httpResponse!.ParseAsync<DeviceAuthorizeResponse>();

      Assert.That(deviceAuthorizeResponse.ExpiresIn, Is.EqualTo(600));
   }

   [Test]
   public async Task response_content_should_contain_interval()
   {
      var deviceAuthorizeResponse = await _httpResponse!.ParseAsync<DeviceAuthorizeResponse>();

      Assert.That(deviceAuthorizeResponse.Interval, Is.EqualTo(5));
   }
}
