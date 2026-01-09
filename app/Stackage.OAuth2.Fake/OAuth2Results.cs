namespace Stackage.OAuth2.Fake;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.WebUtilities;
using Stackage.OAuth2.Fake.Model;

public static class OAuth2Results
{
   public static RedirectHttpResult SuccessRedirect(string redirectUri, string code, string? state)
   {
      return Redirect(redirectUri, new Dictionary<string, string?>
      {
         ["code"] = code,
         ["state"] = state,
      });
   }

   public static RedirectHttpResult InvalidRequestRedirect(string redirectUri, string description, string? state)
   {
      return Redirect(redirectUri, new Dictionary<string, string?>
      {
         ["error"] = "invalid_request",
         ["error_description"] = description,
         ["state"] = state,
      });
   }

   public static RedirectHttpResult UnsupportedResponseTypeRedirect(string redirectUri, string description, string? state)
   {
      return Redirect(redirectUri, new Dictionary<string, string?>
      {
         ["error"] = "unsupported_response_type",
         ["error_description"] = description,
         ["state"] = state,
      });
   }

   public static BadRequest<ErrorResponse> UnsupportedGrantTypeBadRequest() =>
      BadRequest("unsupported_grant_type", "The given grant_type is not supported");

   public static BadRequest<ErrorResponse> InvalidRequestBadRequest(string description) => BadRequest("invalid_request", description);

   public static BadRequest<ErrorResponse> InvalidGrantBadRequest(string description) => BadRequest("invalid_grant", description);

   private static RedirectHttpResult Redirect(string redirectUri, IDictionary<string, string?> parameters)
   {
      var sanitisedParameters = parameters
         .Where(kvp => !string.IsNullOrEmpty(kvp.Value))
         .ToDictionary(kvp => kvp.Key, string? (kvp) => Uri.EscapeDataString(kvp.Value!));

      return TypedResults.Redirect(QueryHelpers.AddQueryString(redirectUri, sanitisedParameters));
   }

   private static BadRequest<ErrorResponse> BadRequest(string error, string description)
   {
      return TypedResults.BadRequest(new ErrorResponse { Error = error, Description = description });
   }
}
