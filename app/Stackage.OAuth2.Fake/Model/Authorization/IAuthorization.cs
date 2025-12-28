namespace Stackage.OAuth2.Fake.Model.Authorization;

using System.Diagnostics.CodeAnalysis;

public interface IAuthorization
{
   Scope Scope { get; }

   [MemberNotNullWhen(true, nameof(Subject))]
   bool IsAuthenticated { get; }

   string? Subject { get; }

   int? TokenExpirySeconds { get; }
}
