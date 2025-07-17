namespace Stackage.OAuth2.Fake.OutsideIn.Tests;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

[SetUpFixture]
[NonParallelizable]
public class WaitForHealthyApp
{
   [OneTimeSetUp]
   public async Task setup_once_before_all_tests()
   {
      using var httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri(Configuration.AppUri);

      for (var i = 0; i < 30; i++)
      {
         try
         {
            var response = await httpClient.GetAsync("health");

            if (response.StatusCode == HttpStatusCode.OK) break;

            await Task.Delay(1000);
         }
         catch
         {
            await Task.Delay(5000);
         }
      }
   }
}
