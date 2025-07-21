namespace Stackage.OAuth2.Fake.Services;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

public class TokenGenerator : ITokenGenerator
{
   private readonly JsonWebKeyCache _jsonWebKeyCache;
   private readonly Settings _settings;

   public TokenGenerator(
      JsonWebKeyCache jsonWebKeyCache,
      Settings settings)
   {
      _jsonWebKeyCache = jsonWebKeyCache;
      _settings = settings;
   }

   public string Generate(IList<Claim> claims, int expirySeconds)
   {
      var jwk = _jsonWebKeyCache.JsonWebKeys.First();

      var identity = new ClaimsIdentity(claims);

      return CreateJwt(identity, expirySeconds, jwk);
   }

   private string CreateJwt(ClaimsIdentity identity, int expirySeconds, JsonWebKey jwk)
   {
      var tokenHandler = new JwtSecurityTokenHandler();
      var signingCredentials = new SigningCredentials(jwk, SecurityAlgorithms.RsaSha256);
      var utcNow = DateTime.UtcNow;

      var tokenDescriptor = new SecurityTokenDescriptor
      {
         Issuer = _settings.IssuerUrl,
         Subject = identity,
         Expires = utcNow.AddSeconds(expirySeconds),
         SigningCredentials = signingCredentials
      };

      var token = tokenHandler.CreateToken(tokenDescriptor);

      return tokenHandler.WriteToken(token);
   }
}
