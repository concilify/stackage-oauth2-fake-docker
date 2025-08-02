namespace Stackage.OAuth2.Fake.Model.Authorization;

public interface IAuthorization
{
   Scope Scope { get; }

   string Subject { get; }

   int? TokenExpirySeconds { get; }
}
