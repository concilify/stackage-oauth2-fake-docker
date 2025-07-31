namespace Stackage.OAuth2.Fake.Services;

using System.Diagnostics.CodeAnalysis;
using Stackage.OAuth2.Fake.Model;

public interface IUserStore
{
   void Add(User user);

   public bool TryGet(
      string subject,
      [MaybeNullWhen(false)] out User user);
}
