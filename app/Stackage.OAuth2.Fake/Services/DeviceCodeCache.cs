namespace Stackage.OAuth2.Fake.Services;

using System;
using System.Collections.Concurrent;

public class DeviceCodeCache
{
   private readonly ConcurrentDictionary<string, string> _codes = new();

   public (string DeviceCode, string UserCode) Create()
   {
      var deviceCode = Guid.NewGuid().ToString();
      var userCode = Guid.NewGuid().ToString()[..4].ToUpper();

      _codes.TryAdd(deviceCode, userCode);

      return (deviceCode, userCode);
   }

   public bool DeviceIsVerified(string deviceCode)
   {
      return _codes.ContainsKey(deviceCode);
   }

   public void Remove(string deviceCode)
   {
      _codes.TryRemove(deviceCode, out _);
   }
}
