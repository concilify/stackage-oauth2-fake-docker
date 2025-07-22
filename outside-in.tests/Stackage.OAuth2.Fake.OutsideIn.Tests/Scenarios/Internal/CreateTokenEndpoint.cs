namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Internal;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using Stackage.OAuth2.Fake.OutsideIn.Tests.Model;

public class CreateTokenEndpoint
{
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      // var content1 = new FormUrlEncodedContent(new Dictionary<string, string>
      // {
      //    ["client_id"] = "AnyClientId",
      //    ["grant_type"] = "urn:ietf:params:oauth:grant-type:device_code",
      //    ["device_code"] = deviceAuthorizationResponse.DeviceCode,
      // });
      /*
       *          var body = new
         {
            sub = userExternalRef,
            permissions = new[]
            {
               $"create:tenant:{urlPrefix}"
            },
            tenants = new[]
            {
               urlPrefix
            }
         };
       */
      var body = new JsonObject
      {
         ["http://oauth2.fake/permissions"] = new JsonArray { "create:tenant:default" },
         ["http://oauth2.fake/tenants"] = new JsonArray { "create:tenant:default" },
      };

      var content = JsonContent.Create(body);

      _httpResponse = await httpClient.PostAsync(".internal/create-token", content);
   }

   [Test]
   public void response_status_should_be_okay()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.OK));
   }

   [Test]
   public async Task response_content_should_contain_access_token_signed_by_public_key()
   {
      var tokenResponse = await _httpResponse!.ParseAsync<TokenResponse>();

      var parameters = new TokenValidationParameters
      {
         IssuerSigningKey = (await Support.GetJsonWebKeySetAsync()).Keys[0],
         ValidIssuer = Configuration.IssuerUrl,
         ValidateAudience = false
      };

      new JwtSecurityTokenHandler().ValidateToken(
         tokenResponse.AccessToken,
         parameters,
         out var securityToken);

      Assert.That(securityToken, Is.InstanceOf<JwtSecurityToken>());
   }

   [Test]
   public async Task response_content_should_contain_access_token_with_issuer()
   {
      var tokenResponse = await _httpResponse!.ParseAsync<TokenResponse>();

      var securityToken = new JwtSecurityTokenHandler().ReadToken(tokenResponse.AccessToken);

      Assert.That(securityToken.Issuer, Is.EqualTo(Configuration.IssuerUrl));
   }

   [Test]
   public async Task response_content_should_contain_access_token_with_sub()
   {
      var tokenResponse = await _httpResponse!.ParseAsync<TokenResponse>();

      var securityToken = new JwtSecurityTokenHandler().ReadToken(tokenResponse.AccessToken);

      Assert.That(securityToken, Is.InstanceOf<JwtSecurityToken>());

      var jwtSecurityToken = (JwtSecurityToken)securityToken;

      Assert.That(jwtSecurityToken.Subject, Is.EqualTo("default-user-id"));
   }

   [Test]
   public async Task response_content_should_contain_token_type()
   {
      var tokenResponse = await _httpResponse!.ParseAsync<TokenResponse>();

      Assert.That(tokenResponse.TokenType, Is.EqualTo("Bearer"));
   }

   [Test]
   public async Task response_content_should_contain_expires_in()
   {
      var tokenResponse = await _httpResponse!.ParseAsync<TokenResponse>();

      Assert.That(tokenResponse.ExpiresIn, Is.EqualTo(1200));
   }
}
