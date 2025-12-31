namespace Stackage.OAuth2.Fake.Model.Authorization;

using System.Diagnostics.CodeAnalysis;

public interface IAuthorization
{
   string ClientId { get; }

   Scope Scope { get; }

   [MemberNotNullWhen(true, nameof(Subject))]
   bool IsAuthenticated { get; }

   string? Subject { get; }

   int? TokenExpirySeconds { get; }
}
