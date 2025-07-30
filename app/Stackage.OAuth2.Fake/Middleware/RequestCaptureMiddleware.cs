namespace Stackage.OAuth2.Fake.Middleware;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Services;

public class RequestCaptureMiddleware : IMiddleware
{
   private readonly CapturedRequestCache _capturedRequestCache;

   public RequestCaptureMiddleware(CapturedRequestCache capturedRequestCache)
   {
      _capturedRequestCache = capturedRequestCache;
   }

   public async Task InvokeAsync(HttpContext context, RequestDelegate next)
   {
      if (!context.Request.Path.StartsWithSegments("/.internal"))
      {
         var capturedRequest = await CapturedRequest.FromHttpRequestAsync(context.Request);

         await _capturedRequestCache.AddAsync(capturedRequest);
      }

      await next(context);
   }
}
