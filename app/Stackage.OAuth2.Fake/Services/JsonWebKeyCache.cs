namespace Stackage.OAuth2.Fake.Services;

using System;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

public class JsonWebKeyCache
{
   public JsonWebKeyCache()
   {
      var rsa = new RSACryptoServiceProvider(2048);
      var keyId = Guid.NewGuid().ToString();
      var rsaKey = new RsaSecurityKey(rsa) { KeyId = keyId };

      var jsonWebKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(rsaKey);

      jsonWebKey.Alg = "RS256";
      jsonWebKey.Use = "sig";

      JsonWebKeys = [jsonWebKey];
   }

   public JsonWebKey[] JsonWebKeys { get; }
}
