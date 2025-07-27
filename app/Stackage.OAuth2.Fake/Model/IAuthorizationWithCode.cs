namespace Stackage.OAuth2.Fake.Model;

public interface IAuthorizationWithCode : IAuthorization
{
   string Code { get; }
}
