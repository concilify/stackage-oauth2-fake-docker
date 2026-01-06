namespace Stackage.OAuth2.Fake.Services;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using Stackage.OAuth2.Fake.Model;

public class UserStore : IUserStore
{
   private const string UsersFilename = "users.json";

   private static readonly JsonSerializerOptions SerializerOptions = new()
   {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      PropertyNameCaseInsensitive = true,
   };

   private readonly IFileSystem _fileSystem;
   private readonly IClaimsSerializer _claimsSerializer;
   private readonly string _usersPath;

   public UserStore(
      IFileSystem fileSystem,
      IClaimsSerializer claimsSerializer)
   {
      _fileSystem = fileSystem;
      _claimsSerializer = claimsSerializer;
      _usersPath = Path.Combine(AppContext.BaseDirectory, UsersFilename);
   }

   public bool TryAdd(User user)
   {
      var users = Load();

      if (!users.TryAdd(user.Subject, user))
      {
         return false;
      }

      Save(users);

      return true;
   }

   public IReadOnlyList<User> GetAll()
   {
      return Load().Values.ToList();
   }

   public bool TryGet(
      string subject,
      [MaybeNullWhen(false)] out User user)
   {
      var users = Load();

      return users.TryGetValue(subject, out user);
   }

   private Dictionary<string, User> Load()
   {
      if (!_fileSystem.File.Exists(_usersPath))
      {
         return new Dictionary<string, User>();
      }

      var usersJson = _fileSystem.File.ReadAllText(_usersPath);
      var seededUsers = JsonSerializer.Deserialize<List<SeededUser>>(usersJson, SerializerOptions);

      if (seededUsers == null)
      {
         throw new JsonException("Failed to deserialize users.");
      }

      return seededUsers
         .Where(u => u is { Subject: not null, Claims: not null })
         .Select(u => new User(u.Subject!, ParseClaims(u.Claims!)))
         .ToDictionary(u => u.Subject);
   }

   private void Save(IDictionary<string, User> users)
   {
      var seededUsers = users
         .Select(u => new SeededUser { Subject = u.Value.Subject, Claims = _claimsSerializer.Serialize(u.Value.Claims) })
         .ToList();

      var usersJson = JsonSerializer.Serialize(seededUsers, SerializerOptions);

      _fileSystem.File.WriteAllText(_usersPath, usersJson);
   }

   private ImmutableArray<Claim> ParseClaims(JsonObject claimsObject)
   {
      if (!_claimsSerializer.TryDeserialize(claimsObject, out var claims))
      {
         throw new JsonException("Failed to deserialize claims.");
      }

      return claims;
   }

   private record SeededUser
   {
      public string? Subject { get; init; }

      public JsonObject? Claims { get; init; }
   }
}
