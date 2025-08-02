namespace Stackage.OAuth2.Fake.Model.Authorization;

using System;

public record DeviceAuthorization(string DeviceCode, string UserCode, Scope Scope) : IAuthorizationWithCode
{
   private string? _subject;

   public string Code => DeviceCode;

   public string Subject => _subject ?? throw new InvalidOperationException($"{nameof(DeviceAuthorization)} has not been authorized.");

   public int? TokenExpirySeconds => null;

   public bool IsAuthorized => _subject != null;

   public void Authorize(string subject)
   {
      _subject = subject;
   }

   public static DeviceAuthorization Create(Scope scope)
   {
      return new DeviceAuthorization(
         DeviceCode: Guid.NewGuid().ToString(),
         UserCode: Guid.NewGuid().ToString()[..4].ToUpper(),
         Scope: scope);
   }
}
