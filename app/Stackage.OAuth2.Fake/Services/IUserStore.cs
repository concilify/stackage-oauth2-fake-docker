namespace Stackage.OAuth2.Fake.Services;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Stackage.OAuth2.Fake.Model;

public interface IUserStore
{
   bool TryAdd(User user);

   IReadOnlyList<User> GetAll();

   public bool TryGet(
      string subject,
      [MaybeNullWhen(false)] out User user);
}
