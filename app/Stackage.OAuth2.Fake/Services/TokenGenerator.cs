namespace Stackage.OAuth2.Fake.Services;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Model.Authorization;

public class TokenGenerator : ITokenGenerator
{
   private readonly JsonWebKeyCache _jsonWebKeyCache;
   private readonly IUserStore _userStore;
   private readonly Settings _settings;
   private readonly AuthorizationCache<RefreshAuthorization> _authorizationCache;

   public TokenGenerator(
      JsonWebKeyCache jsonWebKeyCache,
      IUserStore userStore,
      Settings settings,
      AuthorizationCache<RefreshAuthorization> authorizationCache)
   {
      _jsonWebKeyCache = jsonWebKeyCache;
      _userStore = userStore;
      _settings = settings;
      _authorizationCache = authorizationCache;
   }

   public TokenResponse Generate(IAuthorization authorization)
   {
      if (!authorization.IsAuthenticated)
      {
         throw new InvalidOperationException($"{authorization.GetType().Name} has not been authenticated.");
      }

      var accessTokenClaims = new List<Claim>
      {
         new(JwtRegisteredClaimNames.Sub, authorization.Subject)
      };

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
         ExpiresInSeconds = expirySeconds
      };

      if (!authorization.Scope.IsEmpty)
      {
         if (authorization.Scope.Contains("openid"))
         {
            var idTokenClaims = new List<Claim>
            {
               new(JwtRegisteredClaimNames.Sub, authorization.Subject)
            };

            if (authorization.Scope.Contains("profile") && _userStore.TryGet(authorization.Subject, out var user))
            {
               string[] profileClaims =
               [
                  JwtRegisteredClaimNames.Name,
                  JwtRegisteredClaimNames.Nickname,
                  JwtRegisteredClaimNames.Picture,
               ];

               idTokenClaims.AddRange(user.GetClaims(profileClaims));
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

         response = response with { Scope = authorization.Scope };
      }

      return response;
   }

   private string Generate(IList<Claim> claims, int expirySeconds)
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
         SigningCredentials = signingCredentials
      };

      if (expirySeconds <= 0)
      {
         tokenDescriptor.NotBefore = utcNow.AddSeconds(expirySeconds).AddMilliseconds(-1);
      }

      var tokenHandler = new JwtSecurityTokenHandler();

      var token = tokenHandler.CreateToken(tokenDescriptor);

      return tokenHandler.WriteToken(token);
   }
}
