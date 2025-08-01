namespace Stackage.OAuth2.Fake.Tests.Endpoints;

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NUnit.Framework;

public class TokenEndpointTests
{
   [TestCase("oauth2/token")]
   [TestCase("alternate/token-path")]
   public async Task token_path_can_be_varied(string path)
   {
      var factory = new OAuth2FakeWebApplicationFactory();

      factory.Settings = factory.Settings with { TokenPath = $"/{path}" };

      var httpClient = factory.CreateClient();

      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
         ["grant_type"] = "UnsupportedGrantType",
      });

      var httpResponse = await httpClient.PostAsync(
         path,
         content);

      Assert.That(httpResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

      var errorResponse = await httpResponse!.ParseAsync<ErrorResponse>();

      Assert.That(errorResponse.Error, Is.EqualTo("unsupported_grant_type"));
   }

   private record ErrorResponse(
      [property: JsonPropertyName("error")] string Error,
      [property: JsonPropertyName("error_description")] string ErrorDescription);
}
