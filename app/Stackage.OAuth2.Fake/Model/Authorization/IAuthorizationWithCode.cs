namespace Stackage.OAuth2.Fake.Model.Authorization;

public interface IAuthorizationWithCode : IAuthorization
{
   string Code { get; }
}
