using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace MyApp;

public static class Program
{
    public static void Main(string[] args)
    {
        bool authEnabled = false;
        bool httpsEnabled = false;

        var builder = WebApplication.CreateBuilder(args);

        // CORS used during development to allow React on port 3000
        // to call the API on port 9000/9001
        const string CORSPolicyName = "KM-CORS";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: CORSPolicyName, policy =>
            {
                policy
                    .AllowCredentials()
                    .WithMethods("HEAD", "GET", "POST", "PUT", "DELETE", "OPTIONS")
                    .WithOrigins(
                        "http://127.0.0.1:3000",
                        "https://127.0.0.1:3000",
                        "http://localhost:3000",
                        "https://localhost:3000"
                    )
                    .AllowAnyHeader();
            });
        });

        // Set upload limit to 12 GB (note: multipart form data is encoded, so max file size is less)
        var maxSize = 12L * 1024 * 1024 * 1024;
        builder.Services.Configure<IISServerOptions>(options =>
        {
            // Default is 28.6MB
            options.MaxRequestBodySize = maxSize;
        });
        builder.Services.Configure<KestrelServerOptions>(options =>
        {
            // Default is 28.6MB
            options.Limits.MaxRequestBodySize = maxSize;
        });
        builder.Services.Configure<FormOptions>(x =>
        {
            // Default is 128 MB
            x.MultipartBodyLengthLimit = maxSize;
            // Default is 4MB
            x.ValueLengthLimit = int.MaxValue;
            // Default is 16KB
            x.MultipartHeadersLengthLimit = int.MaxValue;
        });

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
            app.UseCors(CORSPolicyName);
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

        var get = app.MapPost("/api/upload", async Task<IResult> (HttpContext httpContext) =>
            {
                if (authEnabled) { httpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi); }

                IFormCollection form = await httpContext.Request.ReadFormAsync().ConfigureAwait(false);

                if (form.Files.Count == 0)
                {
                    return Results.Problem(detail: "Missing file", statusCode: 400);
                }

                IFormFile file = form.Files.First();
                return Results.Ok($"File received: {file.FileName}, {file.ContentType}, {file.Length} bytes");
            })
            .WithOpenApi();

        if (authEnabled)
        {
            get.RequireAuthorization();
        }

        app.Run();
    }
}
