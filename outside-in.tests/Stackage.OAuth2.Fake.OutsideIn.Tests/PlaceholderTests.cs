namespace Stackage.OAuth2.Fake.OutsideIn.Tests;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

public class PlaceholderTests
{
    [Test]
    public async Task weather_forecast_endpoint_returns_okay()
    {
       using var httpClient = new HttpClient();
       httpClient.BaseAddress = new Uri(Configuration.AppUri);

       var response = await httpClient.GetAsync("weatherforecast");

       Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
