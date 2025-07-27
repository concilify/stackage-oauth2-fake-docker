namespace Stackage.OAuth2.Fake.Tests.GrantTypeHandlers;

using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using Stackage.OAuth2.Fake.GrantTypeHandlers;
using Stackage.OAuth2.Fake.Model;
using Stackage.OAuth2.Fake.Services;
using Stackage.OAuth2.Fake.Tests.Stubs;

public class DeviceCodeGrantTypeHandlerTests
{
   [Test]
   public void handle_returns_status_code_400_when_device_code_missing()
   {
      var testSubject = CreateHandler(
         authorizationCache: new AuthorizationCache<DeviceAuthorization>());

      var httpRequest = CreateRequest(new Dictionary<string, StringValues>());

      var result = testSubject.Handle(httpRequest);

      Assert.That(result, Is.InstanceOf<IStatusCodeHttpResult>());
      Assert.That(((IStatusCodeHttpResult)result).StatusCode, Is.EqualTo(400));
   }

   [Test]
   public void handle_returns_status_code_400_when_device_code_not_found()
   {
      var testSubject = CreateHandler(
         authorizationCache: new AuthorizationCache<DeviceAuthorization>());

      var httpRequest = CreateRequest(new Dictionary<string, StringValues>
      {
         ["device_code"] = "UnknownDeviceCode"
      });

      var result = testSubject.Handle(httpRequest);

      Assert.That(result, Is.InstanceOf<IStatusCodeHttpResult>());
      Assert.That(((IStatusCodeHttpResult)result).StatusCode, Is.EqualTo(400));
   }

   [Test]
   public void handle_returns_status_code_200_when_device_code_found()
   {
      var authorizationCache = new AuthorizationCache<DeviceAuthorization>();

      var authorization = authorizationCache.Add(() => DeviceAuthorization.Create(scope: string.Empty));

      var testSubject = CreateHandler(
         authorizationCache: authorizationCache);

      var httpRequest = CreateRequest(new Dictionary<string, StringValues>
      {
         ["device_code"] = authorization.DeviceCode
      });

      var result = testSubject.Handle(httpRequest);

      Assert.That(result, Is.InstanceOf<IStatusCodeHttpResult>());
      Assert.That(((IStatusCodeHttpResult)result).StatusCode, Is.EqualTo(200));
   }

   [Test]
   public void second_call_to_handle_returns_status_code_400_when_device_code_found()
   {
      var authorizationCache = new AuthorizationCache<DeviceAuthorization>();

      var authorization = authorizationCache.Add(() => DeviceAuthorization.Create(scope: string.Empty));

      var testSubject = CreateHandler(
         authorizationCache: authorizationCache);

      var httpRequest = CreateRequest(new Dictionary<string, StringValues>
      {
         ["device_code"] = authorization.DeviceCode
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

   private static DeviceCodeGrantTypeHandler CreateHandler(
      AuthorizationCache<DeviceAuthorization>? authorizationCache = null,
      Settings? settings = null,
      ITokenGenerator? tokenGenerator = null)
   {
      authorizationCache ??= new AuthorizationCache<DeviceAuthorization>();
      settings ??= new Settings();
      tokenGenerator ??= TokenGeneratorStub.Valid();

      return new DeviceCodeGrantTypeHandler(
         authorizationCache,
         settings,
         tokenGenerator);
   }
}
