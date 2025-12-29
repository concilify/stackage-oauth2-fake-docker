namespace Stackage.OAuth2.Fake.Model.Authorization;

using System;

public record UserAuthorization(string Code, Scope Scope, string ClientId) : IAuthorizationWithCode, IAuthorizationWithClientId
{
   public bool IsAuthenticated => Subject != null;

   public string? Subject { get; private set; }

   public int? TokenExpirySeconds => null;

   public void Authenticate(string subject)
   {
      Subject = subject;
   }

   public static UserAuthorization Create(Scope scope, string clientId)
   {
      return new UserAuthorization(
         Code: Guid.NewGuid().ToString(),
         Scope: scope,
         ClientId: clientId);
   }
}
