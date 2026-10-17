namespace Stackage.OAuth2.Fake.Services.ProviderClaimsPolicies;

using System.Collections.Immutable;
using System.Security.Claims;
using Stackage.OAuth2.Fake.Model;

public class GoogleProviderClaimsPolicy : DefaultProviderClaimsPolicy
{
   private const string HdClaimName = "hd";
   private const string EmailVerifiedClaimName = "email_verified";

   public override string ProviderMode => ProviderModes.Google;

   public override ImmutableArray<Claim> GetIdTokenClaims(User user, Scope scope) =>
   [
      .. base.GetIdTokenClaims(user, scope),
      .. Optional(HdClaimName, user.TenantId),
      .. Optional(EmailVerifiedClaimName, user.Verified),
   ];
}
