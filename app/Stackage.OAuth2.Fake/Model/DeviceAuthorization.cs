namespace Stackage.OAuth2.Fake.Model;

using System;
using System.Collections.Generic;
using System.Security.Claims;

public record DeviceAuthorization(string DeviceCode, string UserCode, Scope Scope) : IFutureAuthorization
{
   private string? _subject;

   public string Code => DeviceCode;

   public bool IncludeScope => !Scope.IsEmpty;

   public bool IncludeRefreshToken => Scope.Contains("offline_access");

   public string Subject => _subject ?? throw new InvalidOperationException($"{nameof(DeviceAuthorization)} has not been authorized.");

   public IEnumerable<Claim> Claims => [];

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
