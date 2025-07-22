namespace Stackage.OAuth2.Fake.OutsideIn.Tests;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
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

   public static async Task<DeviceAuthorizeResponse> StartDeviceAuthorizationAsync(
      this HttpClient httpClient,
      OpenIdConfigurationResponse openIdConfigurationResponse)
   {
      var content = new FormUrlEncodedContent(new Dictionary<string, string>
      {
         ["client_id"] = "AnyClientId",
      });

      var httpResponse = await httpClient.PostAsync(
         openIdConfigurationResponse.DeviceAuthorizationEndpoint,
         content);

      return await httpResponse.ParseAsync<DeviceAuthorizeResponse>();
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
}
