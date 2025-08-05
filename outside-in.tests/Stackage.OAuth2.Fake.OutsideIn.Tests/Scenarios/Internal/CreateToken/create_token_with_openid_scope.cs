namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Internal.CreateToken;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Stackage.OAuth2.Fake.OutsideIn.Tests.Model;

// ReSharper disable once InconsistentNaming
public class create_token_with_openid_scope
{
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      var body = new
      {
         scopes = new[] { "any_scope", "openid" },
         claims = new { }
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
      Assert.That(scope!.Value, Is.EqualTo("any_scope openid"));
   }

   [Test]
   public async Task response_content_should_contain_id_token_with_sub()
   {
      var tokenResponse = await _httpResponse!.ParseAsync<TokenResponse>();

      var jwtSecurityToken = tokenResponse.ParseIdTokenAsJwtSecurityToken();

      Assert.That(jwtSecurityToken.Subject, Is.EqualTo("default-subject"));
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

      Assert.That(tokenResponse.Scope, Is.EqualTo("any_scope openid"));
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
