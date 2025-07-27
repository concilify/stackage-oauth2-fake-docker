namespace Stackage.OAuth2.Fake.Model;

using System;

public record UserAuthorization(string Code, Scope Scope) : IAuthorization
{
   public bool IncludeScope => !Scope.IsEmpty;

   public bool IncludeRefreshToken => Scope.Contains("offline_access");

   public static UserAuthorization Create(Scope scope)
   {
      return new UserAuthorization(
         Code: Guid.NewGuid().ToString(),
         Scope: scope);
   }
}
