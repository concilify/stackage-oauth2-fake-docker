namespace Stackage.OAuth2.Fake.Services;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Model.Authorization;
using Stackage.OAuth2.Fake.Services.ProviderClaimsPolicies;

public class TokenGenerator : ITokenGenerator
{
   private readonly JsonWebKeyCache _jsonWebKeyCache;
   private readonly IUserStore _userStore;
   private readonly Settings _settings;
   private readonly AuthorizationCache<RefreshAuthorization> _authorizationCache;
   private readonly IEnumerable<IProviderClaimsPolicy> _providerClaimsPolicies;

   public TokenGenerator(
      JsonWebKeyCache jsonWebKeyCache,
      IUserStore userStore,
      Settings settings,
      AuthorizationCache<RefreshAuthorization> authorizationCache,
      IEnumerable<IProviderClaimsPolicy> providerClaimsPolicies)
   {
      _jsonWebKeyCache = jsonWebKeyCache;
      _userStore = userStore;
      _settings = settings;
      _authorizationCache = authorizationCache;
      _providerClaimsPolicies = providerClaimsPolicies;
   }

   public TokenResponse Generate(IAuthorization authorization)
   {
      if (!authorization.IsAuthenticated)
      {
         throw new InvalidOperationException($"{authorization.GetType().Name} has not been authenticated.");
      }

      var providerClaimsPolicy = _providerClaimsPolicies
         .FirstOrDefault(strategy => string.Equals(strategy.ProviderMode, _settings.ProviderMode, StringComparison.OrdinalIgnoreCase));

      if (providerClaimsPolicy == null)
      {
         throw new InvalidOperationException($"ProviderMode {_settings.ProviderMode} is not supported.");
      }

      var user = GetUser(authorization.Subject);

      var accessTokenClaims = new List<Claim>
      {
         new(JwtRegisteredClaimNames.Sub, authorization.Subject),
         new("client_id", authorization.ClientId),
      };

      if (user != null)
      {
         accessTokenClaims.AddRange(providerClaimsPolicy.GetAccessTokenClaims(user, authorization.Scope));
      }

      if (authorization is IAuthorizationWithAudiences { Audiences: not null } authorizationWithAudiences)
      {
         foreach (var audience in authorizationWithAudiences.Audiences)
         {
            accessTokenClaims.Add(new Claim(JwtRegisteredClaimNames.Aud, audience));
         }
      }

      if (authorization is IAuthorizationWithClaims authorizationWithClaims)
      {
         accessTokenClaims.AddRange(authorizationWithClaims.Claims);
      }

      if (!authorization.Scope.IsEmpty)
      {
         accessTokenClaims.Add(new Claim("scope", authorization.Scope));
      }

      var expirySeconds = authorization.TokenExpirySeconds ?? _settings.DefaultTokenExpirySeconds;

      var response = new TokenResponse
      {
         AccessToken = Generate(accessTokenClaims, expirySeconds),
         ExpiresInSeconds = expirySeconds,
      };

      if (!authorization.Scope.IsEmpty)
      {
         response = response with { Scope = authorization.Scope };
      }

      if (authorization.Scope.Contains("openid"))
      {
         var idTokenClaims = new List<Claim>
         {
            new(JwtRegisteredClaimNames.Sub, authorization.Subject),
            new(JwtRegisteredClaimNames.Aud, authorization.ClientId),
         };

         if (user != null)
         {
            idTokenClaims.AddRange(providerClaimsPolicy.GetIdTokenClaims(user, authorization.Scope));
         }

         if (authorization is IAuthorizationWithNonce { Nonce: not null } authorizationWithNonce)
         {
            idTokenClaims.Add(new Claim("nonce", authorizationWithNonce.Nonce));
         }

         var idToken = Generate(idTokenClaims, expirySeconds);

         response = response with { IdToken = idToken };
      }

      if (authorization.Scope.Contains("offline_access"))
      {
         var refreshToken = Guid.NewGuid().ToString();

         response = response with { RefreshToken = refreshToken };

         _authorizationCache.Add(() => RefreshAuthorization.Create(refreshToken, authorization));
      }

      return response;
   }

   private string Generate(IEnumerable<Claim> claims, int expirySeconds)
   {
      var jwk = _jsonWebKeyCache.JsonWebKeys.First();

      var identity = new ClaimsIdentity(claims);
      var signingCredentials = new SigningCredentials(jwk, SecurityAlgorithms.RsaSha256);
      var utcNow = DateTime.UtcNow;

      var tokenDescriptor = new SecurityTokenDescriptor
      {
         Issuer = _settings.IssuerUrl,
         Subject = identity,
         Expires = utcNow.AddSeconds(expirySeconds),
         SigningCredentials = signingCredentials,
      };

      if (expirySeconds <= 0)
      {
         tokenDescriptor.NotBefore = utcNow.AddSeconds(expirySeconds).AddMilliseconds(-1);
      }

      var tokenHandler = new JwtSecurityTokenHandler();

      var token = tokenHandler.CreateToken(tokenDescriptor);

      return tokenHandler.WriteToken(token);
   }

   private User? GetUser(string subject)
   {
      if (_userStore.TryGet(subject, out var user))
      {
         return user;
      }

      return null;
   }
}
