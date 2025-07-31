namespace Stackage.OAuth2.Fake.Services;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Stackage.OAuth2.Fake.Model;

public class UserStore : IUserStore
{
   private readonly IDictionary<string, User> _users;

   public UserStore(IConfiguration configuration)
   {
      var usersSection = configuration.GetSection("Users");

      _users = ParseUsers(usersSection).ToDictionary(u => u.Subject);
   }

   public void Add(User user)
   {
      _users.Add(user.Subject, user);
   }

   public bool TryGet(
      string subject,
      [MaybeNullWhen(false)] out User user)
   {
      return _users.TryGetValue(subject, out user);
   }

   private static IEnumerable<User> ParseUsers(IConfigurationSection usersSection)
   {
      var configurationUsers = usersSection.Get<List<ConfigurationUser>>() ?? [];

      return configurationUsers.Select(u => new User(u.Subject, [..ParseClaims(u.ClaimsSection)]));
   }

   private static IEnumerable<Claim> ParseClaims(IConfigurationSection claimsSection)
   {
      return claimsSection.GetChildren()
         .Where(c => c.Value != null)
         .Select(c => new Claim(c.Key, c.Value!));
   }

   private record ConfigurationUser(
      string Subject,
      [property: ConfigurationKeyName("Claims")] IConfigurationSection ClaimsSection);
}
