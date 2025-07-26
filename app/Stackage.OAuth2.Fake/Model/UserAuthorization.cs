namespace Stackage.OAuth2.Fake.Model;

using System;

public record UserAuthorization(string Code, string[] Scopes) : IAuthorization
{
   public static UserAuthorization Create(string[] scopes)
   {
      return new UserAuthorization(
         Code: Guid.NewGuid().ToString(),
         Scopes: scopes);
   }
}
