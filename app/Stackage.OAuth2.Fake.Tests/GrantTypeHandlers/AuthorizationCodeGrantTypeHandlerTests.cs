namespace Stackage.OAuth2.Fake.Tests.GrantTypeHandlers;

using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using Stackage.OAuth2.Fake.GrantTypeHandlers;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Services;
using Stackage.OAuth2.Fake.Tests.Stubs;

public class AuthorizationCodeGrantTypeHandlerTests
{
   [Test]
   public void handle_returns_status_code_400_when_authorization_code_missing()
   {
      var testSubject = CreateHandler(
         authorizationCache: new AuthorizationCache<UserAuthorization>());

      var httpRequest = CreateRequest(new Dictionary<string, StringValues>());

      var result = testSubject.Handle(httpRequest);

      Assert.That(result, Is.InstanceOf<IStatusCodeHttpResult>());
      Assert.That(((IStatusCodeHttpResult)result).StatusCode, Is.EqualTo(400));
   }

   [Test]
   public void handle_returns_status_code_400_when_authorization_code_not_found()
   {
      var testSubject = CreateHandler(
         authorizationCache: new AuthorizationCache<UserAuthorization>());

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
      var authorizationCache = new AuthorizationCache<UserAuthorization>();

      var authorization = authorizationCache.Add(() => UserAuthorization.Create(scopes: []));

      var testSubject = CreateHandler(
         authorizationCache: authorizationCache);

      var httpRequest = CreateRequest(new Dictionary<string, StringValues>
      {
         ["code"] = authorization.Code
      });

      var result = testSubject.Handle(httpRequest);

      Assert.That(result, Is.InstanceOf<IStatusCodeHttpResult>());
      Assert.That(((IStatusCodeHttpResult)result).StatusCode, Is.EqualTo(200));
   }

   [Test]
   public void second_call_to_handle_returns_status_code_400_when_authorization_code_found()
   {
      var authorizationCache = new AuthorizationCache<UserAuthorization>();

      var authorization = authorizationCache.Add(() => UserAuthorization.Create(scopes: []));

      var testSubject = CreateHandler(
         authorizationCache: authorizationCache);

      var httpRequest = CreateRequest(new Dictionary<string, StringValues>
      {
         ["code"] = authorization.Code
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
      AuthorizationCache<UserAuthorization>? authorizationCache = null,
      Settings? settings = null,
      ITokenGenerator? tokenGenerator = null)
   {
      authorizationCache ??= new AuthorizationCache<UserAuthorization>();
      settings ??= new Settings();
      tokenGenerator ??= TokenGeneratorStub.Valid();

      return new AuthorizationCodeGrantTypeHandler(
         authorizationCache,
         settings,
         tokenGenerator);
   }
}
