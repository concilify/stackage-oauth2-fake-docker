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
      var deviceAuthorizationResponse = await _httpResponse!.ParseAsync<DeviceAuthorizationResponse>();

      Assert.That(Guid.TryParse(deviceAuthorizationResponse.DeviceCode, out _), Is.True);
   }

   [Test]
   public async Task response_content_should_contain_user_code()
   {
      var deviceAuthorizationResponse = await _httpResponse!.ParseAsync<DeviceAuthorizationResponse>();

      Assert.That(deviceAuthorizationResponse.UserCode.Length, Is.EqualTo(4));
   }

   [Test]
   public async Task response_content_should_contain_verification_uri()
   {
      var deviceAuthorizationResponse = await _httpResponse!.ParseAsync<DeviceAuthorizationResponse>();

      Assert.That(
         deviceAuthorizationResponse.VerificationUri,
         Is.EqualTo($"{Configuration.IssuerUrl}/oauth2/device/verify"));
   }

   [Test]
   public async Task response_content_should_contain_verification_uri_complete()
   {
      var deviceAuthorizationResponse = await _httpResponse!.ParseAsync<DeviceAuthorizationResponse>();

      Assert.That(
         deviceAuthorizationResponse.VerificationUriComplete,
         Is.EqualTo($"{Configuration.IssuerUrl}/oauth2/device/verify?user_code={deviceAuthorizationResponse.UserCode}"));
   }

   [Test]
   public async Task response_content_should_contain_expires_in()
   {
      var deviceAuthorizationResponse = await _httpResponse!.ParseAsync<DeviceAuthorizationResponse>();

      Assert.That(deviceAuthorizationResponse.ExpiresIn, Is.EqualTo(600));
   }

   [Test]
   public async Task response_content_should_contain_interval()
   {
      var deviceAuthorizationResponse = await _httpResponse!.ParseAsync<DeviceAuthorizationResponse>();

      Assert.That(deviceAuthorizationResponse.Interval, Is.EqualTo(5));
   }
}
