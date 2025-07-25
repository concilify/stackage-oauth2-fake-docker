namespace Stackage.OAuth2.Fake.Services;

using System;
using System.Collections.Concurrent;

public class AuthorizationCodeCache
{
   private readonly ConcurrentDictionary<string, string> _codes = new();

   public string Create()
   {
      var code = Guid.NewGuid().ToString();

      // TODO: capture optional scope here too

      _codes.TryAdd(code, string.Empty);

      return code;
   }

   // TODO: replace with Try... method and return scope
   public bool CodeIsValid(string code)
   {
      return _codes.ContainsKey(code);
   }

   public void Remove(string code)
   {
      _codes.TryRemove(code, out _);
   }
}
