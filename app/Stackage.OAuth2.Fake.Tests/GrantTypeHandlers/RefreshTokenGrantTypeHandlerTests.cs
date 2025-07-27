namespace Stackage.OAuth2.Fake.Tests.GrantTypeHandlers;

using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using Stackage.OAuth2.Fake.GrantTypeHandlers;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Services;
using Stackage.OAuth2.Fake.Tests.Stubs;

public class RefreshTokenGrantTypeHandlerTests
{
   [Test]
   public void handle_returns_status_code_400_when_refresh_token_missing()
   {
      var testSubject = CreateHandler(
         authorizationCache: new AuthorizationCache<RefreshAuthorization>());

      var httpRequest = CreateRequest(new Dictionary<string, StringValues>());

      var result = testSubject.Handle(httpRequest);

      Assert.That(result, Is.InstanceOf<IStatusCodeHttpResult>());
      Assert.That(((IStatusCodeHttpResult)result).StatusCode, Is.EqualTo(400));
   }

   [Test]
   public void handle_returns_status_code_400_when_refresh_token_not_found()
   {
      var testSubject = CreateHandler(
         authorizationCache: new AuthorizationCache<RefreshAuthorization>());

      var httpRequest = CreateRequest(new Dictionary<string, StringValues>
      {
         ["refresh_token"] = "UnknownRefreshToken"
      });

      var result = testSubject.Handle(httpRequest);

      Assert.That(result, Is.InstanceOf<IStatusCodeHttpResult>());
      Assert.That(((IStatusCodeHttpResult)result).StatusCode, Is.EqualTo(400));
   }

   [Test]
   public void handle_returns_status_code_200_when_refresh_token_found()
   {
      var authorizationCache = new AuthorizationCache<RefreshAuthorization>();

      authorizationCache.Add(
         () => RefreshAuthorization.Create("AnyRefreshToken", AuthorizationStub.Valid()));

      var testSubject = CreateHandler(
         authorizationCache: authorizationCache);

      var httpRequest = CreateRequest(new Dictionary<string, StringValues>
      {
         ["refresh_token"] = "AnyRefreshToken"
      });

      var result = testSubject.Handle(httpRequest);

      Assert.That(result, Is.InstanceOf<IStatusCodeHttpResult>());
      Assert.That(((IStatusCodeHttpResult)result).StatusCode, Is.EqualTo(200));
   }

   [Test]
   public void second_call_to_handle_returns_status_code_400_when_refresh_token_found()
   {
      var authorizationCache = new AuthorizationCache<RefreshAuthorization>();

      authorizationCache.Add(
         () => RefreshAuthorization.Create("AnyRefreshToken", AuthorizationStub.Valid()));

      var testSubject = CreateHandler(
         authorizationCache: authorizationCache);

      var httpRequest = CreateRequest(new Dictionary<string, StringValues>
      {
         ["refresh_token"] = "AnyRefreshToken"
      });

      testSubject.Handle(httpRequest);

      var result = testSubject.Handle(httpRequest);

      Assert.That(result, Is.InstanceOf<IStatusCodeHttpResult>());
      Assert.That(((IStatusCodeHttpResult)result).StatusCode, Is.EqualTo(400));
   }

   private static HttpRequest CreateRequest(Dictionary<string, StringValues> formValues)
   {
      var httpContext = new DefaultHttpContext
      {
         Request =
         {
            Form = new FormCollection(formValues)
         }
      };

      return httpContext.Request;
   }

   private static RefreshTokenGrantTypeHandler CreateHandler(
      AuthorizationCache<RefreshAuthorization>? authorizationCache = null,
      ITokenGenerator? tokenGenerator = null)
   {
      authorizationCache ??= new AuthorizationCache<RefreshAuthorization>();
      tokenGenerator ??= TokenGeneratorStub.Valid();

      return new RefreshTokenGrantTypeHandler(
         authorizationCache,
         tokenGenerator);
   }
}
