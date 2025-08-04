namespace Stackage.OAuth2.Fake.Model.Authorization;

using System;

public record RefreshAuthorization(string RefreshToken, Scope Scope, string Subject) : IAuthorizationWithCode
{
   public string Code => RefreshToken;

   public bool IsAuthenticated => true;

   public int? TokenExpirySeconds => null;

   public static RefreshAuthorization Create(string refreshToken, IAuthorization authorization)
   {
      if (!authorization.IsAuthenticated)
      {
         throw new InvalidOperationException($"{authorization.GetType().Name} has not been authenticated.");
      }

      return new RefreshAuthorization(refreshToken, authorization.Scope, authorization.Subject);
   }
}
