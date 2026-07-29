namespace Stackage.OAuth2.Fake.Model.Authorization;

public interface IAuthorizationWithNonce : IAuthorization
{
   string? Nonce { get; }
}
