namespace Stackage.OAuth2.Fake.Model;

public interface IAuthorization
{
   Scope Scope { get; }

   string Subject { get; }
}
