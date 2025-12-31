namespace Stackage.OAuth2.Fake.Model.Authorization;

using System;

public record UserAuthorization(string Code, string ClientId, Scope Scope, string[]? Audiences) : IAuthorizationWithCode
{
   public bool IsAuthenticated => Subject != null;

   public string? Subject { get; private set; }

   public int? TokenExpirySeconds => null;

   public void Authenticate(string subject)
   {
      Subject = subject;
   }

   public static UserAuthorization Create(string clientId, Scope scope, string? audience)
   {
      return new UserAuthorization(
         Code: Guid.NewGuid().ToString(),
         ClientId: clientId,
         Scope: scope,
         Audiences: audience != null ? [audience] : null);
   }
}
