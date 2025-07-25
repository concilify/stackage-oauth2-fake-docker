namespace Stackage.OAuth2.Fake.Tests.GrantTypeHandlers;

using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using Stackage.OAuth2.Fake.GrantTypeHandlers;
using Stackage.OAuth2.Fake.Services;
using Stackage.OAuth2.Fake.Tests.Stubs;

public class AuthorizationCodeGrantTypeHandlerTests
{
   [Test]
   public void handle_returns_status_code_400_when_authorization_code_missing()
   {
      var testSubject = CreateHandler(
         authorizationCodeCache: new AuthorizationCodeCache());

      var httpRequest = CreateRequest(new Dictionary<string, StringValues>());

      var result = testSubject.Handle(httpRequest);

      Assert.That(result, Is.InstanceOf<IStatusCodeHttpResult>());
      Assert.That(((IStatusCodeHttpResult)result).StatusCode, Is.EqualTo(400));
   }

   [Test]
   public void handle_returns_status_code_400_when_authorization_code_not_found()
   {
      var testSubject = CreateHandler(
         authorizationCodeCache: new AuthorizationCodeCache());

      var httpRequest = CreateRequest(new Dictionary<string, StringValues>
      {
         ["code"] = "UnknownAuthorizationCode"
      });

      var result = testSubject.Handle(httpRequest);

      Assert.That(result, Is.InstanceOf<IStatusCodeHttpResult>());
      Assert.That(((IStatusCodeHttpResult)result).StatusCode, Is.EqualTo(400));
   }

   [Test]
   public void handle_returns_status_code_200_when_authorization_code_found()
   {
      var authorizationCodeCache = new AuthorizationCodeCache();

      var authorizationCode = authorizationCodeCache.Create();

      var testSubject = CreateHandler(
         authorizationCodeCache: authorizationCodeCache);

      var httpRequest = CreateRequest(new Dictionary<string, StringValues>
      {
         ["code"] = authorizationCode
      });

      var result = testSubject.Handle(httpRequest);

      Assert.That(result, Is.InstanceOf<IStatusCodeHttpResult>());
      Assert.That(((IStatusCodeHttpResult)result).StatusCode, Is.EqualTo(200));
   }

   [Test]
   public void second_call_to_handle_returns_status_code_400_when_authorization_code_found()
   {
      var authorizationCodeCache = new AuthorizationCodeCache();

      var authorizationCode = authorizationCodeCache.Create();

      var testSubject = CreateHandler(
         authorizationCodeCache: authorizationCodeCache);

      var httpRequest = CreateRequest(new Dictionary<string, StringValues>
      {
         ["code"] = authorizationCode
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

   private static AuthorizationCodeGrantTypeHandler CreateHandler(
      AuthorizationCodeCache? authorizationCodeCache = null,
      Settings? settings = null,
      ITokenGenerator? tokenGenerator = null)
   {
      authorizationCodeCache ??= new AuthorizationCodeCache();
      settings ??= new Settings();
      tokenGenerator ??= TokenGeneratorStub.Valid();

      return new AuthorizationCodeGrantTypeHandler(
         authorizationCodeCache,
         settings,
         tokenGenerator);
   }
}
