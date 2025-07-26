namespace Stackage.OAuth2.Fake.Model;

public record RefreshAuthorization(string RefreshToken) : IAuthorization
{
   public string Code => RefreshToken;

   public static RefreshAuthorization Create(string refreshToken)
   {
      return new RefreshAuthorization(refreshToken);
   }
}
