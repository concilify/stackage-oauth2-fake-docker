namespace Stackage.OAuth2.Fake;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

public static class Result
{
   public static IResult SuccessRedirect(string redirectUri, string code, string? state)
   {
      return Redirect(redirectUri, new Dictionary<string, string?>
      {
         ["code"] = code,
         ["state"] = state,
      });
   }

   public static IResult InvalidRequestRedirect(string redirectUri, string description, string? state)
   {
      return Redirect(redirectUri, new Dictionary<string, string?>
      {
         ["error"] = "invalid_request",
         ["error_description"] = description,
         ["state"] = state,
      });
   }

   public static IResult UnsupportedResponseTypeRedirect(string redirectUri, string description, string? state)
   {
      return Redirect(redirectUri, new Dictionary<string, string?>
      {
         ["error"] = "unsupported_response_type",
         ["error_description"] = description,
         ["state"] = state,
      });
   }

   public static IResult UnsupportedGrantType()
      => BadRequest("unsupported_grant_type", "The given grant_type is not supported");

   public static IResult InvalidRequest(string description) => BadRequest("invalid_request", description);

   public static IResult InvalidGrant(string description) => BadRequest("invalid_grant", description);

   private static IResult Redirect(string redirectUri, IDictionary<string, string?> parameters)
   {
      var sanitisedParameters = parameters
         .Where(kvp => !string.IsNullOrEmpty(kvp.Value))
         .ToDictionary(kvp => kvp.Key, string? (kvp) => Uri.EscapeDataString(kvp.Value!));

      return Results.Redirect(QueryHelpers.AddQueryString(redirectUri, sanitisedParameters));
   }

   private static IResult BadRequest(string error, string description)
   {
      return TypedResults.BadRequest(new
      {
         error,
         error_description = description,
      });
   }
}
