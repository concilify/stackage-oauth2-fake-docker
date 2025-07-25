namespace Stackage.OAuth2.Fake.Services;

using System;
using System.Collections.Concurrent;

public class AuthorizationCodeCache
{
   private readonly ConcurrentDictionary<string, string> _codes = new();

   public string Create()
   {
      var code = Guid.NewGuid().ToString();

      _codes.TryAdd(code, string.Empty);

      return code;
   }

   public bool CodeIsValid(string code)
   {
      return _codes.ContainsKey(code);
   }

   public void Remove(string code)
   {
      _codes.TryRemove(code, out _);
   }
}
