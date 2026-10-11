namespace Stackage.OAuth2.Fake.Services.ProviderClaimsPolicies;

using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Stackage.OAuth2.Fake.Model;

public class DefaultProviderClaimsPolicy : IProviderClaimsPolicy
{
   private static readonly string[] ProfileClaims =
   [
      JwtRegisteredClaimNames.Name,
      JwtRegisteredClaimNames.Nickname,
      JwtRegisteredClaimNames.Picture,
      JwtRegisteredClaimNames.Email,
   ];

   public virtual string ProviderMode => "Default";

   public virtual ImmutableArray<Claim> GetAccessTokenClaims(User user, Scope scope) =>
   [
      .. user.GetClaimsWithout(ProfileClaims)
   ];

   public virtual ImmutableArray<Claim> GetIdTokenClaims(User user, Scope scope) =>
   [
      .. scope.Contains("profile") ? user.GetClaims(ProfileClaims) : [],
   ];

   protected static ImmutableArray<Claim> Optional(string type, string? value) =>
      value is null ? [] : [new Claim(type, value)];
}
