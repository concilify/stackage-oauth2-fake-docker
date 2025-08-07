namespace Stackage.OAuth2.Fake.Tests.Stubs;

using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

public static class ConfigurationStub
{
   public static IConfiguration Empty() => With(new Dictionary<string, string?>());

   public static IConfiguration With(IDictionary<string, string?> dictionary)
   {
      return new ConfigurationBuilder()
         .AddInMemoryCollection(dictionary)
         .Build();
   }
}
