namespace Stackage.OAuth2.Fake.Services;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;
using Stackage.OAuth2.Fake.Model;

public class UserStore : IUserStore
{
   private readonly Dictionary<string, User> _users;

   public UserStore(
      IConfiguration configuration,
      IClaimsParser claimsParser)
   {
      var usersSection = configuration.GetSection("Users");

      _users = ParseUsers(usersSection, claimsParser).ToDictionary(u => u.Subject);
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
      IConfigurationSection usersSection,
      IClaimsParser claimsParser)
   {
      var configurationUsers = usersSection.Get<List<ConfigurationUser>>() ?? [];

      return configurationUsers.Select(u => new User(u.Subject, ParseClaims(u.ClaimsSection, claimsParser)));
   }

   private static ImmutableArray<Claim> ParseClaims(
         IConfigurationSection claimsSection,
         IClaimsParser claimsParser)
   {
      var claimsObject = new JsonObject();

      foreach (var claim in claimsSection.GetChildren())
      {
         claimsObject.Add(claim.Key, claim.Value);
      }

      if (claimsParser.TryParse(claimsObject, out var claims))
      {
         return claims;
      }

      return ImmutableArray<Claim>.Empty;
   }

   private record ConfigurationUser(
      string Subject,
      [property: ConfigurationKeyName("Claims")] IConfigurationSection ClaimsSection);
}
