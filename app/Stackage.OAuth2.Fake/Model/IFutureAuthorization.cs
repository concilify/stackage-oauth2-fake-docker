namespace Stackage.OAuth2.Fake.Model;

public interface IFutureAuthorization : IAuthorization
{
   string Code { get; }

   bool IsAuthorized { get; }

   void Authorize(string subject);
}
