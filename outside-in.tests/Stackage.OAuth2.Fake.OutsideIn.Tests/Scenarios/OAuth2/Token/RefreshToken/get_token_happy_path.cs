namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.OAuth2.Token.RefreshToken;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using Stackage.OAuth2.Fake.OutsideIn.Tests.Model;

// ReSharper disable once InconsistentNaming
public class get_token_happy_path
{
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var handler = new HttpClientHandler();
      handler.AllowAutoRedirect = false;

      var httpClient = new HttpClient(handler);
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      var openIdConfigurationResponse = await httpClient.GetWellKnownOpenIdConfigurationAsync();

      var authorizationResponse = await httpClient.StartAuthorizationAsync(
         openIdConfigurationResponse,
         scopes: ["offline_access"]);

      var tokenResponse = await httpClient.ExchangeAuthorizationCodeAsync(
         openIdConfigurationResponse,
         authorizationResponse.Code);

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
   public void response_status_should_be_okay()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.OK));
   }

   [Test]
   public async Task response_content_should_contain_access_token_signed_by_public_key()
   {
      var tokenResponse = await _httpResponse!.ParseAsync<TokenResponse>();

      var jsonWebKeySet = await Support.GetJsonWebKeySetAsync();

      tokenResponse.AssertAccessTokenIsSigned(jsonWebKeySet.Keys[0]);
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

      var jwtSecurityToken = tokenResponse.ParseJwtSecurityToken();

      Assert.That(jwtSecurityToken.Subject, Is.EqualTo("default-subject"));
   }

   [Test]
   public async Task response_content_should_contain_access_token_with_scope()
   {
      var tokenResponse = await _httpResponse!.ParseAsync<TokenResponse>();

      var scope = tokenResponse.ParseClaim("scope");

      Assert.That(scope, Is.Not.Null);
      Assert.That(scope!.Value, Is.EqualTo("offline_access"));
   }

   [Test]
   public async Task response_content_should_contain_refresh_token()
   {
      var tokenResponse = await _httpResponse!.ParseAsync<TokenResponse>();

      Assert.That(Guid.TryParse(tokenResponse.RefreshToken, out _), Is.True);
   }

   [Test]
   public async Task response_content_should_contain_scope()
   {
      var tokenResponse = await _httpResponse!.ParseAsync<TokenResponse>();

      Assert.That(tokenResponse.Scope, Is.EqualTo("offline_access"));
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
