namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.OAuth2.Device;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

public class DeviceAuthorizationEndpoint
{
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      var openIdConfiguration = await httpClient.GetWellKnownOpenIdConfiguration();

      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
         ["client_id"] = "AnyClientId",
      });

      _httpResponse = await httpClient.PostAsync(
         openIdConfiguration.DeviceAuthorizationEndpoint,
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
      var oAuth2DeviceAuthorize = await _httpResponse!.ParseOAuth2DeviceAuthorize();

      Assert.That(Guid.TryParse(oAuth2DeviceAuthorize.DeviceCode, out _), Is.True);
   }

   [Test]
   public async Task response_content_should_contain_user_code()
   {
      var oAuth2DeviceAuthorize = await _httpResponse!.ParseOAuth2DeviceAuthorize();

      Assert.That(oAuth2DeviceAuthorize.UserCode.Length, Is.EqualTo(4));
   }

   [Test]
   public async Task response_content_should_contain_verification_uri()
   {
      var oAuth2DeviceAuthorize = await _httpResponse!.ParseOAuth2DeviceAuthorize();

      Assert.That(
         oAuth2DeviceAuthorize.VerificationUri,
         Is.EqualTo($"{Configuration.IssuerUrl}/oauth2/device/verify"));
   }

   [Test]
   public async Task response_content_should_contain_verification_uri_complete()
   {
      var oAuth2DeviceAuthorize = await _httpResponse!.ParseOAuth2DeviceAuthorize();

      Assert.That(
         oAuth2DeviceAuthorize.VerificationUriComplete,
         Is.EqualTo($"{Configuration.IssuerUrl}/oauth2/device/verify?user_code={oAuth2DeviceAuthorize.UserCode}"));
   }

   [Test]
   public async Task response_content_should_contain_expires_in()
   {
      var oAuth2DeviceAuthorize = await _httpResponse!.ParseOAuth2DeviceAuthorize();

      Assert.That(oAuth2DeviceAuthorize.ExpiresIn, Is.EqualTo(600));
   }

   [Test]
   public async Task response_content_should_contain_interval()
   {
      var oAuth2DeviceAuthorize = await _httpResponse!.ParseOAuth2DeviceAuthorize();

      Assert.That(oAuth2DeviceAuthorize.Interval, Is.EqualTo(5));
   }
}
