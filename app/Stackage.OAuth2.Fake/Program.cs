using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stackage.OAuth2.Fake;
using Stackage.OAuth2.Fake.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var settings = builder.Configuration.Get<Settings>()!;
builder.Services.AddSingleton(settings);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok());

app.MapWellKnownEndpoints();
app.MapOAuth2DeviceEndpoints();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
   var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
           Random.Shared.Next(-20, 55),
           summaries[Random.Shared.Next(summaries.Length)]
       ))
       .ToArray();
   return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
   public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
