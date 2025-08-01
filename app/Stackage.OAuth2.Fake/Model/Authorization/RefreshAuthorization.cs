namespace Stackage.OAuth2.Fake.Model.Authorization;

public record RefreshAuthorization(string RefreshToken, Scope Scope, string Subject) : IAuthorizationWithCode
{
   public string Code => RefreshToken;

   public int? TokenExpirySeconds => null;

   public static RefreshAuthorization Create(string refreshToken, IAuthorization authorization)
   {
      return new RefreshAuthorization(refreshToken, authorization.Scope, authorization.Subject);
   }
}
