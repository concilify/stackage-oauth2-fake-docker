namespace Stackage.OAuth2.Fake.Model;

using System;

public record UserAuthorization(string Code, Scope Scope) : IAuthorizationWithCode
{
   private string? _subject;

   public string Subject => _subject ?? throw new InvalidOperationException($"{nameof(UserAuthorization)} has not been authorized.");

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
