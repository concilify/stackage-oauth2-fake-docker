namespace Stackage.OAuth2.Fake.Model;

using System;
using System.Collections.Generic;
using System.Security.Claims;

public record UserAuthorization(string Code, Scope Scope) : IFutureAuthorization
{
   private string? _subject;

   public bool IncludeScope => !Scope.IsEmpty;

   public bool IncludeRefreshToken => Scope.Contains("offline_access");

   public string Subject => _subject ?? throw new InvalidOperationException($"{nameof(UserAuthorization)} has not been authorized.");

   public IEnumerable<Claim> Claims => [];

   public bool IsAuthorized => _subject != null;

   public void Authorize(string subject)
   {
      _subject = subject;
   }

   public static UserAuthorization Create(Scope scope)
   {
      return new UserAuthorization(
         Code: Guid.NewGuid().ToString(),
         Scope: scope);
   }
}
