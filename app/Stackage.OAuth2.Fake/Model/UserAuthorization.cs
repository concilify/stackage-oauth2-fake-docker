namespace Stackage.OAuth2.Fake.Model;

using System;

public record UserAuthorization(string Code) : IAuthorization
{
   public static UserAuthorization Create()
   {
      return new UserAuthorization(Code: Guid.NewGuid().ToString());
   }
}
