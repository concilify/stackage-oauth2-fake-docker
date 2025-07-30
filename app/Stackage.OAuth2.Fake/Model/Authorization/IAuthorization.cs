namespace Stackage.OAuth2.Fake.Model.Authorization;

// TODO: subject and expiry should be optional and use settings for defaults in token generator
public interface IAuthorization
{
   Scope Scope { get; }

   string Subject { get; }

   int TokenExpirySeconds { get; }
}
