namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.OAuth2.Token.RefreshToken;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

// ReSharper disable once InconsistentNaming
public class get_token_with_seeded_refresh_token
{
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      var openIdConfigurationResponse = await httpClient.GetWellKnownOpenIdConfigurationAsync();

      var tokenResponse = await httpClient.InternalCreateTokenAsync(
         scopes: ["offline_access"]);

      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
         ["client_id"] = "AnyClientId",
         ["grant_type"] = "refresh_token",
         ["refresh_token"] = tokenResponse.RefreshToken ?? string.Empty,
      });

      _httpResponse = await httpClient.PostAsync(
         openIdConfigurationResponse.TokenEndpoint,
         content);
   }

   [Test]
   public void METHOD()
   {
      // seed refresh token and call auth
      Assert.Fail();
   }
}
