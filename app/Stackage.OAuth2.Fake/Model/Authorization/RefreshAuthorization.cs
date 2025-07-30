namespace Stackage.OAuth2.Fake.Model.Authorization;

public record RefreshAuthorization(string RefreshToken, Scope Scope, string Subject) : IAuthorizationWithCode
{
   private const int DefaultTokenExpirySeconds = 20 * 60;

   public string Code => RefreshToken;

   public int TokenExpirySeconds => DefaultTokenExpirySeconds;

   public static RefreshAuthorization Create(string refreshToken, IAuthorization authorization)
   {
      return new RefreshAuthorization(refreshToken, authorization.Scope, authorization.Subject);
   }
}
