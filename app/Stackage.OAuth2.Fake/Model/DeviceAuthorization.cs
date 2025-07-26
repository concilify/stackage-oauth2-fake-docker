namespace Stackage.OAuth2.Fake.Model;

using System;

public record DeviceAuthorization(string DeviceCode, string UserCode) : IAuthorization
{
   public string Code => DeviceCode;

   public static DeviceAuthorization Create()
   {
      return new DeviceAuthorization(
         DeviceCode: Guid.NewGuid().ToString(),
         UserCode: Guid.NewGuid().ToString()[..4].ToUpper());
   }
}
