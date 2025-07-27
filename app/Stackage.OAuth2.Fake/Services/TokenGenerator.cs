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
   private const int TokenExpirySecs = 20 * 60;

   private readonly JsonWebKeyCache _jsonWebKeyCache;
   private readonly Settings _settings;

   public TokenGenerator(
      JsonWebKeyCache jsonWebKeyCache,
      Settings settings)
   {
      _jsonWebKeyCache = jsonWebKeyCache;
      _settings = settings;
   }

   public TokenResponse Generate(IAuthorization authorization)
   {
      var claims = new List<Claim>
      {
         new(JwtRegisteredClaimNames.Sub, authorization.Subject)
      };

      if (authorization is IAuthorizationWithClaims authorizationWithClaims)
      {
         claims.AddRange(authorizationWithClaims.Claims);
      }

      if (!authorization.Scope.IsEmpty)
      {
         claims.Add(new Claim("scope", authorization.Scope));
      }

      var response = new TokenResponse
      {
         AccessToken = Generate(claims, TokenExpirySecs),
         ExpiresInSeconds = TokenExpirySecs
      };

      if (!authorization.Scope.IsEmpty)
      {
         if (authorization.Scope.Contains("offline_access"))
         {
            var refreshToken = Guid.NewGuid().ToString();

            response = response with { RefreshToken = refreshToken };
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

      var tokenHandler = new JwtSecurityTokenHandler();

      var token = tokenHandler.CreateToken(tokenDescriptor);

      return tokenHandler.WriteToken(token);
   }
}
