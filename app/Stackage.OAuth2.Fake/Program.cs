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
builder.Services.AddSingleton<AuthorizationCodeCache>();
builder.Services.AddSingleton<DeviceCodeCache>();
builder.Services.AddSingleton<JsonWebKeyCache>();

builder.Services.AddTransient<IGrantTypeHandler, AuthorizationCodeGrantTypeHandler>();
builder.Services.AddTransient<IGrantTypeHandler, DeviceCodeGrantTypeHandler>();
builder.Services.AddTransient<ITokenGenerator, TokenGenerator>();
builder.Services.AddTransient<IClaimsParser, ClaimsParser>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok());

app.MapWellKnownEndpoints();
app.MapAuthorizationEndpoint();
app.MapOAuth2DeviceEndpoints();
app.MapTokenEndpoint();
app.MapInternalEndpoints();

app.Run();
