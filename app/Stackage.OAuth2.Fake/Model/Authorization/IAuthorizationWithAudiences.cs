namespace Stackage.OAuth2.Fake.Model.Authorization;

public interface IAuthorizationWithAudiences : IAuthorization
{
   string[]? Audiences { get; }
}
