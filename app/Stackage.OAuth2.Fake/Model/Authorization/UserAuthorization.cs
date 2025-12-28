namespace Stackage.OAuth2.Fake.Model.Authorization;

using System;

public record UserAuthorization(
   string Code,
   Scope Scope,
   string ClientId,
   string[]? Audiences) : IAuthorizationWithIdToken, IAuthorizationWithRefreshToken, IAuthorizationWithAudiences
{
   public bool IsAuthenticated => Subject != null;

   public string? Subject { get; private set; }

   public int? TokenExpirySeconds => null;

   public void Authenticate(string subject)
   {
      Subject = subject;
   }

   public static UserAuthorization Create(Scope scope, string? audience)
   {
      return new UserAuthorization(
         Code: Guid.NewGuid().ToString(),
         Scope: scope,
         Audiences: audience != null ? [audience] : null);
   }
}
