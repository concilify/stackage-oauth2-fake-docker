namespace Stackage.OAuth2.Fake.Model.Authorization;

public interface IAuthorizationWithClientId : IAuthorization
{
   string ClientId { get; }
}
