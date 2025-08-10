namespace Stackage.OAuth2.Fake.Services;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stackage.OAuth2.Fake.Model;

public class CapturedRequestCache
{
   private readonly List<CapturedRequest> _capturedRequests = [];
   private readonly SemaphoreSlim _semaphore = new(1);

   public async Task AddAsync(CapturedRequest capturedRequest)
   {
      await SynchronizeAsync(() => _capturedRequests.Add(capturedRequest));
   }

   public async Task ClearAsync()
   {
      await SynchronizeAsync(() => _capturedRequests.Clear());
   }

   public IEnumerable<CapturedRequest> GetAll()
   {
      return _capturedRequests.ToArray();
   }

   private async Task SynchronizeAsync(Action action)
   {
      await _semaphore.WaitAsync();

      try
      {
         action();
      }
      finally
      {
         _semaphore.Release();
      }
   }
}
