/**
 * Docs:
 * - https://learn.microsoft.com/aspnet/core/fundamentals/static-files
 * - https://learn.microsoft.com/aspnet/core/client-side/spa/react
 * - https://learn.microsoft.com/aspnet/core/test/http-files
 * - https://www.typescriptlang.org/docs/handbook/react.html
 */

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace MyApp;

public static class Program
{
    public static void Main(string[] args)
    {
        bool authEnabled = false;
        bool httpsEnabled = true;

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // ...

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        if (authEnabled)
        {
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));
            builder.Services.AddAuthorization();
        }

        var scopeRequiredByApi = builder.Configuration["AzureAd:Scopes"] ?? "";

        var app = builder.Build();

        // var scopeRequiredByApi = app.Configuration["AzureAd:Scopes"] ?? "";

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // UseDefaultFiles must be called before UseStaticFiles to serve the default file
        app.UseDefaultFiles();

        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = (StaticFileResponseContext ctx) =>
            {
                var cacheMaxAgeSecs = app.Environment.IsDevelopment() ? 0 : 3600;
                ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cacheMaxAgeSecs}");
            },
        });

        if (authEnabled) { app.UseAuthorization(); }

        if (httpsEnabled) { app.UseHttpsRedirection(); }

        var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
        var get = app.MapGet("/api/weatherforecast", (HttpContext httpContext) =>
            {
                if (authEnabled) { httpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi); }

                var forecast = Enumerable.Range(1, 5).Select(index =>
                        new WeatherForecast
                        {
                            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            TemperatureC = Random.Shared.Next(-20, 55),
                            Summary = summaries[Random.Shared.Next(summaries.Length)]
                        })
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi();

        if (authEnabled)
        {
            get.RequireAuthorization();
        }

        app.Run();
    }
}
