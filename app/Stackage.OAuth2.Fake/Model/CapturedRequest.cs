namespace Stackage.OAuth2.Fake.Model;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

public record CapturedRequest(
   string Path,
   string Method,
   IDictionary<string, StringValues> Headers,
   string BodyBase64)
{
   public static async Task<CapturedRequest> FromHttpRequestAsync(HttpRequest request)
   {
      request.EnableBuffering();

      var bodyBytes = await ReadAsBytesAsync(request.Body);

      request.Body.Position = 0;

      var capturedRequest = new CapturedRequest(
         request.Path,
         request.Method,
         request.Headers,
         Convert.ToBase64String(bodyBytes));

      return capturedRequest;
   }

   private static async Task<byte[]> ReadAsBytesAsync(Stream stream)
   {
      if (stream is not MemoryStream memoryStream)
      {
         memoryStream = new MemoryStream();

         await stream.CopyToAsync(memoryStream);
      }

      return memoryStream.ToArray();
   }
};
