namespace Stackage.OAuth2.Fake.Services;

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

public class AuthorizationCache
{
   private readonly ConcurrentDictionary<string, string[]> _authorizations = new();

   public string Create(string[] scopes)
   {
      var code = Guid.NewGuid().ToString();

      _authorizations.TryAdd(code, scopes);

      return code;
   }

   public bool TryGet(
      string code,
      [MaybeNullWhen(false)] out string[] scope)
   {
      return _authorizations.TryGetValue(code, out scope);
   }

   public void Remove(string code)
   {
      _authorizations.TryRemove(code, out _);
   }
}
