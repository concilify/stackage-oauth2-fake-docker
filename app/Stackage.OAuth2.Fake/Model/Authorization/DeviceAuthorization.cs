namespace Stackage.OAuth2.Fake.Model.Authorization;

using System;

public record DeviceAuthorization(
   string DeviceCode,
   string UserCode,
   Scope Scope,
   string ClientId,
   string[]? Audiences) : IAuthorizationWithCode, IAuthorizationWithIdToken, IAuthorizationWithRefreshToken, IAuthorizationWithAudiences
{
   public string Code => DeviceCode;

   public bool IsAuthenticated => Subject != null;

   public string? Subject { get; private set; }

   public int? TokenExpirySeconds => null;

   public void Authenticate(string subject)
   {
      Subject = subject;
   }

   public static DeviceAuthorization Create(Scope scope, string? audience)
   {
      return new DeviceAuthorization(
         DeviceCode: Guid.NewGuid().ToString(),
         UserCode: Guid.NewGuid().ToString()[..4].ToUpper(),
         Scope: scope,
         Audiences: audience != null ? [audience] : null);
   }
}
