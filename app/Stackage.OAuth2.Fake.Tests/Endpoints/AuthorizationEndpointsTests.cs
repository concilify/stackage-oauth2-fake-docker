namespace Stackage.OAuth2.Fake.Tests.Endpoints;

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using NUnit.Framework;

public class AuthorizationEndpointsTests
{
   [TestCase("oauth2/authorize")]
   [TestCase("alternate/authorize")]
   public async Task authorize_path_can_be_varied(string path)
   {
      var factory = new OAuth2FakeWebApplicationFactory();

      factory.Settings = factory.Settings with { AuthorizationPath = $"/{path}" };
      factory.ClientOptions.AllowAutoRedirect = false;

      var httpClient = factory.CreateClient();

      var requestQuery = new Dictionary<string, string?>
      {
         ["state"] = "AnyState",
         ["redirect_uri"] = "http://any-host/callback"
      };

      var httpResponse = await httpClient.GetAsync(QueryHelpers.AddQueryString(path, requestQuery));

      Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));

      Assert.That(httpResponse.Headers.Location?.AbsoluteUri, Does.StartWith("http://any-host/callback"));
      Assert.That(httpResponse.Headers.Location?.Query, Does.Contain("state=AnyState"));
   }

   [TestCase("oauth2/device/authorize", "oauth2/device/verify")]
   [TestCase("alternate/device/authorize", "alternate/device/verify")]
   public async Task device_authorize_and_verify_path_can_be_varied(string authorizePath, string verifyPath)
   {
      var factory = new OAuth2FakeWebApplicationFactory();

      factory.Settings = factory.Settings with
      {
         DeviceAuthorizationPath = $"/{authorizePath}",
         DeviceVerificationPath = $"/{verifyPath}"
      };

      var httpClient = factory.CreateClient();

      var httpResponse = await httpClient.PostAsync(
         authorizePath,
         new FormUrlEncodedContent(new Dictionary<string, string?>()));

      Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

      var deviceAuthorizationResponse = await httpResponse!.ParseAsync<DeviceAuthorizationResponse>();

      Assert.That(deviceAuthorizationResponse.VerificationUri, Is.EqualTo($"/{verifyPath}"));
   }

   private record DeviceAuthorizationResponse(
      [property: JsonPropertyName("verification_uri")] string VerificationUri);
}
