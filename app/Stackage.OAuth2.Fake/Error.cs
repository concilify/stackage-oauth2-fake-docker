namespace Stackage.OAuth2.Fake;

using Microsoft.AspNetCore.Http;

public static class Error
{
   public static IResult UnsupportedGrantType()
      => BadRequest("unsupported_grant_type", "The given grant_type is not supported");

   public static IResult InvalidRequest(string description) => BadRequest("invalid_request", description);

   public static IResult InvalidGrant(string description) => BadRequest("invalid_grant", description);

   private static IResult BadRequest(string error, string description)
   {
      return TypedResults.BadRequest(new
      {
         error,
         error_description = description,
      });
   }
}
