namespace Stackage.OAuth2.Fake.Services.ProviderClaimsPolicies;

using System.Collections.Immutable;
using System.Security.Claims;
using Stackage.OAuth2.Fake.Model;

public class EntraProviderClaimsPolicy : DefaultProviderClaimsPolicy
{
   private const string TidClaimName = "tid";
   private const string OidClaimName = "oid";
   private const string XmsEdovClaimName = "xms_edov";

   public override string ProviderMode => ProviderModes.Entra;

   public override ImmutableArray<Claim> GetAccessTokenClaims(User user, Scope scope) =>
   [
      .. base.GetAccessTokenClaims(user, scope),
      .. Optional(TidClaimName, user.TenantId),
      .. Optional(OidClaimName, user.ObjectId),
   ];

   public override ImmutableArray<Claim> GetIdTokenClaims(User user, Scope scope) =>
   [
      .. base.GetIdTokenClaims(user, scope),
      .. Optional(TidClaimName, user.TenantId),
      .. scope.Contains("profile") ? Optional(OidClaimName, user.ObjectId) : [],
      .. Optional(XmsEdovClaimName, user.Verified),
   ];
}
