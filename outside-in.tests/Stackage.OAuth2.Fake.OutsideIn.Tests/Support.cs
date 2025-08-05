namespace Stackage.OAuth2.Fake.OutsideIn.Tests;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using Stackage.OAuth2.Fake.OutsideIn.Tests.Model;

public static class Support
{
   public static async Task<TResponse> ParseAsync<TResponse>(this HttpResponseMessage httpResponseMessage)
   {
      var response = JsonSerializer.Deserialize<TResponse>(await httpResponseMessage.Content.ReadAsStringAsync());

      if (response == null)
      {
         throw new JsonException("Failed to deserialize the response content.");
      }

      return response;
   }

   public static async Task<OpenIdConfigurationResponse> GetWellKnownOpenIdConfigurationAsync(this HttpClient httpClient)
   {
      var httpResponse = await httpClient.GetAsync(".well-known/openid-configuration");

      return await httpResponse.ParseAsync<OpenIdConfigurationResponse>();
   }

   public static async Task<JsonWebKeySet> GetJsonWebKeySetAsync()
   {
      using var httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      var httpResponse = await httpClient.GetAsync(".well-known/jwks.json");

      return await httpResponse.ParseAsync<JsonWebKeySet>();
   }

   public static async Task<AuthorizationResponse> StartAuthorizationAsync(
      this HttpClient httpClient,
      OpenIdConfigurationResponse openIdConfigurationResponse,
      string[]? scopes = null)
   {
      scopes ??= [];

      var requestQuery = new Dictionary<string, string?>
      {
         ["response_type"] = "code",
         ["state"] = "AnyState",
         ["redirect_uri"] = "http://any-host/callback"
      };

      if (scopes.Length != 0)
      {
         requestQuery["scope"] = string.Join(" ", scopes);
      }

      var httpResponse = await httpClient.GetAsync(
         QueryHelpers.AddQueryString(openIdConfigurationResponse.AuthorizationEndpoint, requestQuery));

      var queryString = QueryHelpers.ParseNullableQuery(httpResponse.Headers.Location?.Query);

      Assert.That(queryString?.Keys, Contains.Item("code"));

      return new AuthorizationResponse(Code: queryString!["code"].ToString());
   }

   public static async Task<DeviceAuthorizationResponse> StartDeviceAuthorizationAsync(
      this HttpClient httpClient,
      OpenIdConfigurationResponse openIdConfigurationResponse,
      string[]? scopes = null)
   {
      scopes ??= [];

      var requestQuery = new Dictionary<string, string?>
      {
         ["client_id"] = "AnyClientId",
      };

      if (scopes.Length != 0)
      {
         requestQuery["scope"] = string.Join(" ", scopes);
      }

      var content = new FormUrlEncodedContent(requestQuery);

      var httpResponse = await httpClient.PostAsync(
         openIdConfigurationResponse.DeviceAuthorizationEndpoint,
         content);

      return await httpResponse.ParseAsync<DeviceAuthorizationResponse>();
   }

   public static async Task<TokenResponse> ExchangeAuthorizationCodeAsync(
      this HttpClient httpClient,
      OpenIdConfigurationResponse openIdConfigurationResponse,
      string code)
   {
      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
         ["client_id"] = "AnyClientId",
         ["grant_type"] = "authorization_code",
         ["code"] = code
      });

      var httpResponse = await httpClient.PostAsync(
         openIdConfigurationResponse.TokenEndpoint,
         content);

      return await httpResponse.ParseAsync<TokenResponse>();
   }

   public static async Task<TokenResponse> ExchangeDeviceCodeAsync(
      this HttpClient httpClient,
      OpenIdConfigurationResponse openIdConfigurationResponse,
      string deviceCode)
   {
      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
         ["client_id"] = "AnyClientId",
         ["grant_type"] = "urn:ietf:params:oauth:grant-type:device_code",
         ["device_code"] = deviceCode
      });

      var httpResponse = await httpClient.PostAsync(
         openIdConfigurationResponse.TokenEndpoint,
         content);

      return await httpResponse.ParseAsync<TokenResponse>();
   }

   public static async Task<TokenResponse> CreateTokenAsync(
      this HttpClient httpClient,
      string[]? scopes = null)
   {
      scopes ??= [];

      var body = new JsonObject
      {
         ["claims"] = new JsonObject()
      };

      if (scopes.Length != 0)
      {
         body["scopes"] = new JsonArray(scopes.Select(s => (JsonNode)s).ToArray());
      }

      var content = JsonContent.Create(body);

      var httpResponse = await httpClient.PostAsync(".internal/create-token", content);

      return await httpResponse.ParseAsync<TokenResponse>();
   }

   public static async Task SeedAuthorizationAsync(
      this HttpClient httpClient,
      string code,
      string[]? scopes = null,
      string? subject = null)
   {
      scopes ??= [];

      var body = new JsonObject
      {
         ["code"] = code
      };

      if (scopes.Length != 0)
      {
         body["scopes"] = new JsonArray(scopes.Select(s => (JsonNode)s).ToArray());
      }

      if (subject != null)
      {
         body["subject"] = subject;
      }

      var content = JsonContent.Create(body);

      var httpResponse = await httpClient.PostAsync(".internal/authorization", content);

      httpResponse.EnsureSuccessStatusCode();
   }

   public static async Task SeedRefreshTokenAsync(
      this HttpClient httpClient,
      string refreshToken,
      string[]? scopes = null,
      string? subject = null)
   {
      scopes ??= [];

      var body = new JsonObject
      {
         ["refreshToken"] = refreshToken
      };

      if (scopes.Length != 0)
      {
         body["scopes"] = new JsonArray(scopes.Select(s => (JsonNode)s).ToArray());
      }

      if (subject != null)
      {
         body["subject"] = subject;
      }

      var content = JsonContent.Create(body);

      var httpResponse = await httpClient.PostAsync(".internal/refresh-token", content);

      httpResponse.EnsureSuccessStatusCode();
   }

   public static void AssertAccessTokenIsSigned(
      this TokenResponse tokenResponse,
      JsonWebKey jsonWebKey)
   {
      var parameters = new TokenValidationParameters
      {
         IssuerSigningKey = jsonWebKey,
         ValidIssuer = Configuration.IssuerUrl,
         ValidateAudience = false
      };

      new JwtSecurityTokenHandler().ValidateToken(
         tokenResponse.AccessToken,
         parameters,
         out var securityToken);

      Assert.That(securityToken, Is.InstanceOf<JwtSecurityToken>());
   }

   public static JwtSecurityToken ParseJwtSecurityToken(this TokenResponse tokenResponse, Func<TokenResponse, string>? tokenAccessor = null)
   {
      tokenAccessor ??= token => token.AccessToken;

      var securityToken = new JwtSecurityTokenHandler().ReadToken(tokenAccessor(tokenResponse));

      Assert.That(securityToken, Is.InstanceOf<JwtSecurityToken>());

      return (JwtSecurityToken)securityToken;
   }

   public static JwtSecurityToken ParseIdTokenAsJwtSecurityToken(this TokenResponse tokenResponse)
   {
      var securityToken = new JwtSecurityTokenHandler().ReadToken(tokenResponse.IdToken);

      Assert.That(securityToken, Is.InstanceOf<JwtSecurityToken>());

      return (JwtSecurityToken)securityToken;
   }

   public static IReadOnlyList<Claim> ParseClaims(this TokenResponse tokenResponse, string name)
   {
      var jwtSecurityToken = tokenResponse.ParseJwtSecurityToken();

      return jwtSecurityToken.Claims.Where(c => c.Type == name).ToList();
   }

   public static Claim? ParseClaim(this TokenResponse tokenResponse, string name)
   {
      var jwtSecurityToken = tokenResponse.ParseJwtSecurityToken();

      return jwtSecurityToken.Claims.SingleOrDefault(c => c.Type == name);
   }
}
