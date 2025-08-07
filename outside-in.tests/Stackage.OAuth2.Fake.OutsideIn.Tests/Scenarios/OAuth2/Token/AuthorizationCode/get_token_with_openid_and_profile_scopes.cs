namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.OAuth2.Token.AuthorizationCode;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using Shouldly;
using Stackage.OAuth2.Fake.OutsideIn.Tests.Model;

// ReSharper disable once InconsistentNaming
public class get_token_with_openid_and_profile_scopes
{
   private string? _code;
   private string? _subject;
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var handler = new HttpClientHandler();
      handler.AllowAutoRedirect = false;

      var httpClient = new HttpClient(handler);
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      var openIdConfigurationResponse = await httpClient.GetWellKnownOpenIdConfigurationAsync();

      _code = Guid.NewGuid().ToString();
      _subject = Guid.NewGuid().ToString();

      await httpClient.SeedAuthorizationAsync(
         code: _code,
         scopes: ["openid", "profile" ],
         subject: _subject);

      await httpClient.SeedUserAsync(
         subject: _subject,
         claims: new Dictionary<string, string>
         {
            ["name"] = $"{_subject}-name",
            ["nickname"] = $"{_subject}-nickname",
            ["picture"] = $"{_subject}-picture"
         });

      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
         ["client_id"] = "AnyClientId",
         ["grant_type"] = "authorization_code",
         ["code"] = _code
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

      var jwtSecurityToken = tokenResponse.ParseAccessTokenAsJwtSecurityToken();

      Assert.That(jwtSecurityToken.Subject, Is.EqualTo(_subject));
   }

   [Test]
   public async Task response_content_should_contain_access_token_with_scope()
   {
      var tokenResponse = await _httpResponse!.ParseAsync<TokenResponse>();

      var scope = tokenResponse.ParseAccessTokenClaim("scope");

      Assert.That(scope, Is.Not.Null);
      Assert.That(scope!.Value, Is.EqualTo("openid profile"));
   }

   [Test]
   public async Task response_content_should_contain_id_token_with_sub()
   {
      var tokenResponse = await _httpResponse!.ParseAsync<TokenResponse>();

      var jwtSecurityToken = tokenResponse.ParseIdTokenAsJwtSecurityToken();

      Assert.That(jwtSecurityToken.Subject, Is.EqualTo(_subject));
   }

   [Test]
   public async Task response_content_should_contain_id_token_with_claims()
   {
      var tokenResponse = await _httpResponse!.ParseAsync<TokenResponse>();

      var claims = tokenResponse.ParseIdTokenClaims("name", "nickname", "picture");

      var expectedClaims = new Dictionary<string, StringValues>
      {
         [JwtRegisteredClaimNames.Name] = $"{_subject}-name",
         [JwtRegisteredClaimNames.Nickname] = $"{_subject}-nickname",
         [JwtRegisteredClaimNames.Picture] = $"{_subject}-picture"
      };

      claims.ShouldBeEquivalentTo(expectedClaims);
   }

   [Test]
   public async Task response_content_should_not_contain_refresh_token()
   {
      var tokenResponse = await _httpResponse!.ParseAsync<TokenResponse>();

      Assert.That(tokenResponse.RefreshToken, Is.Null);
   }

   [Test]
   public async Task response_content_should_contain_scope()
   {
      var tokenResponse = await _httpResponse!.ParseAsync<TokenResponse>();

      Assert.That(tokenResponse.Scope, Is.EqualTo("openid profile"));
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
