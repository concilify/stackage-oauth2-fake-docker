namespace Stackage.OAuth2.Fake.Tests.Services.ProviderClaimsPolicies;

using NUnit.Framework;
using Stackage.OAuth2.Fake.Services.ProviderClaimsPolicies;

public class DefaultProviderClaimsPolicyTests
{
   [Test]
   public void provider_mode_is_default()
   {
      var testSubject = new DefaultProviderClaimsPolicy();

      Assert.That(testSubject.ProviderMode, Is.EqualTo("Default"));
   }
}
