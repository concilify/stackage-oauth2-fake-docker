namespace Stackage.OAuth2.Fake.Model;

using System;
using System.Linq;

public record UserAuthorization(string Code, string[] Scopes) : IAuthorization
{
   public bool IncludeScope => Scopes.Length != 0;

   public bool IncludeRefreshToken => Scopes.Contains("offline_access");

   public string Scope => string.Join(" ", Scopes);

   public static UserAuthorization Create(string scope)
   {
      return new UserAuthorization(
         Code: Guid.NewGuid().ToString(),
         Scopes: scope.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray());
   }
}
