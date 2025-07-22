using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stackage.OAuth2.Fake;
using Stackage.OAuth2.Fake.Endpoints;
using Stackage.OAuth2.Fake.GrantTypeHandlers;
using Stackage.OAuth2.Fake.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(builder.Configuration.Get<Settings>()!);
builder.Services.AddSingleton<DeviceCodeCache>();
builder.Services.AddSingleton<JsonWebKeyCache>();

builder.Services.AddTransient<IGrantTypeHandler, DeviceCodeGrantTypeHandler>();
builder.Services.AddTransient<ITokenGenerator, TokenGenerator>();
builder.Services.AddTransient<IClaimsParser, ClaimsParser>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok());

app.MapWellKnownEndpoints();
app.MapOAuth2DeviceEndpoints();
app.MapTokenEndpoint();
app.MapInternalEndpoints();

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
