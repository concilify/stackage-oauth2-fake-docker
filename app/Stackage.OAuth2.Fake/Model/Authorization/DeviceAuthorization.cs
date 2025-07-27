namespace Stackage.OAuth2.Fake.Model.Authorization;

using System;

public record DeviceAuthorization(string DeviceCode, string UserCode) : IAuthorizationWithCode
{
   private string? _subject;

   public string Code => DeviceCode;

   // TODO: Future PR - scope for device_code grants will be implemented later
   public Scope Scope => Scope.Empty;

   public string Subject => _subject ?? throw new InvalidOperationException($"{nameof(DeviceAuthorization)} has not been authorized.");

   public bool IsAuthorized => _subject != null;

   public void Authorize(string subject)
   {
      _subject = subject;
   }

   public static DeviceAuthorization Create()
   {
      return new DeviceAuthorization(
         DeviceCode: Guid.NewGuid().ToString(),
         UserCode: Guid.NewGuid().ToString()[..4].ToUpper());
   }
}
