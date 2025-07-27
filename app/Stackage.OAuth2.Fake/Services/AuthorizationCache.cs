namespace Stackage.OAuth2.Fake.Services;

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Stackage.OAuth2.Fake.Model.Authorization;

public class AuthorizationCache<TAuthorization>
   where TAuthorization : IAuthorizationWithCode
{
   private readonly ConcurrentDictionary<string, TAuthorization> _authorizations = new();

   public TAuthorization Add(Func<TAuthorization> authorizationFactory)
   {
      var authorization = authorizationFactory();

      _authorizations.TryAdd(authorization.Code, authorization);

      return authorization;
   }

   public bool TryGet(
      string code,
      [MaybeNullWhen(false)] out TAuthorization authorization)
   {
      return _authorizations.TryGetValue(code, out authorization);
   }

   public void Remove(TAuthorization authorization)
   {
      _authorizations.TryRemove(authorization.Code, out _);
   }
}
