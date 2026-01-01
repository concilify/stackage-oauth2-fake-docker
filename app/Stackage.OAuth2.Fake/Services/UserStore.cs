namespace Stackage.OAuth2.Fake.Services;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using Stackage.OAuth2.Fake.Model;

public class UserStore : IUserStore
{
   private const string UsersFilePath = "users.json";

   private readonly Dictionary<string, User> _users;

   public UserStore(
      IFileSystem fileSystem,
      IClaimsParser claimsParser)
   {
      _users = ParseUsers(fileSystem, claimsParser).ToDictionary(u => u.Subject);
   }

   public bool TryAdd(User user)
   {
      return _users.TryAdd(user.Subject, user);
   }

   public IReadOnlyList<User> GetAll()
   {
      return _users.Values.ToList();
   }

   public bool TryGet(
      string subject,
      [MaybeNullWhen(false)] out User user)
   {
      return _users.TryGetValue(subject, out user);
   }

   private static IEnumerable<User> ParseUsers(
      IFileSystem fileSystem,
      IClaimsParser claimsParser)
   {
      if (!fileSystem.File.Exists(UsersFilePath))
      {
         return [];
      }

      try
      {
         var fileContent = fileSystem.File.ReadAllText(UsersFilePath);
         var options = new JsonSerializerOptions
         {
            PropertyNameCaseInsensitive = true,
         };
         var jsonUsers = JsonSerializer.Deserialize<List<ConfigurationUser>>(fileContent, options);

         if (jsonUsers == null)
         {
            return [];
         }

         return jsonUsers
            .Where(u => u.Subject != null && u.Claims != null)
            .Select(u => new User(u.Subject!, ParseClaims(u.Claims!, claimsParser)));
      }
      catch
      {
         // If file reading or JSON deserialization fails, return empty collection
         return [];
      }
   }

   private static ImmutableArray<Claim> ParseClaims(
      JsonObject claimsObject,
      IClaimsParser claimsParser)
   {
      if (claimsParser.TryParse(claimsObject, out var claims))
      {
         return claims;
      }

      return ImmutableArray<Claim>.Empty;
   }

   private record ConfigurationUser
   {
      public string? Subject { get; init; }

      public JsonObject? Claims { get; init; }
   }
}
