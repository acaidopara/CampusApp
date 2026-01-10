using Destructurama;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Persistence.Triggers;
using Rise.Server.Identity;
using Rise.Server.Processors;
using Rise.Services;
using Rise.Services.Identity;
using Rise.Shared.Departments;
using Serilog.Events;
using DotNetEnv;
using Prometheus;
using Rise.Server.Middleware;
using System.Text.Json;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger(); // Initial log setup, will be overwritten by Serilog, but we need a logger before Dependency Injection is activated.

try
{
    Env.Load();
    Log.Information("Starting web application");
    var builder = WebApplication.CreateBuilder(args);

    builder.Services
        .AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .WithOrigins(builder.Configuration["FrontendUrl"] ?? "https://localhost:5001") // je Nginx frontend URL
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        })
        .AddSerilog((_, lc) => lc.ReadFrom.Configuration(builder.Configuration) // Configuration in AppSettings.json
            .Destructure.UsingAttributes()) // Sensitive data logging
        .AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .Services.AddDbContext<ApplicationDbContext>(o =>
        {
            // var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection") ??
            //                     throw new InvalidOperationException("Connection string 'DatabaseConnection' not found.");

            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
            Log.Information($"URL:  {connectionString}");
            o.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 35))); // Swap Sqlite for your database provider (e.g. Sql Server, MySQL, PostgreSQL, etc.).
            o.EnableDetailedErrors();
            if (builder.Environment.IsDevelopment())
            {
                o.EnableSensitiveDataLogging(); // only enabled in development.
            }
            o.UseTriggers(options => options.AddTrigger<EntityBeforeSaveTrigger>()); // Handles all UpdatedAt, CreatedAt stuff.
        })
        .ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = ctx =>
                {
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    var result = Result.Unauthorized();
                    var json = JsonSerializer.Serialize(result);

                    ctx.Response.ContentType = "application/json";
                    return ctx.Response.WriteAsync(json);
                };
            })
        .AddHttpContextAccessor()
        .AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, CustomClaimsPrincipalFactory>()
        .AddScoped<ISessionContextProvider, HttpContextSessionProvider>() // Provides the current user from the HttpContext to the session provider.
        .AddApplicationServices() // You'll need to add your own services in this function call.
        .AddAuthorization()
        .AddFastEndpoints(o =>
        {
            o.IncludeAbstractValidators = true; // Include validators from abstract classes (see https://docs.fluentvalidation.net/en/latest/).
            o.Assemblies = [typeof(DepartmentRequest).Assembly]; // Adds the validators from other assemblies
        })
        .SwaggerDocument(o =>
            {
                o.DocumentSettings = s =>
                    {
                        s.Title = "RISE API";
                    };
            });

    var app = builder.Build();
    app.UseHttpMetrics();

    app.UseCors("AllowFrontend");
    app.UseRequestLocalization();
    // apply Database migraticons on startup, not so wise in production (Use Generated SQL Scripts) 
    // See: https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli
    if (app.Environment.IsDevelopment()) 
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var dbSeeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
            dbContext.Database.EnsureDeleted(); // Delete the database if it exists to clean it up if needed.

            Log.Information("About to migrate DB...");
            dbContext.Database.Migrate();
            Log.Information("Migration completed."); // Creates the database if it doesn't exist and applies all migrations. See Readme.md for more info.
            await dbSeeder.SeedAsync(); // Seeds the database with some test data.
        }
    }
    // Theses middlewares are strict in order of calling!
    app.UseHttpsRedirection()
        .UseBlazorFrameworkFiles() // Blazor is also served from the API. 
        .UseStaticFiles()
        .UseMiddleware<GlobalExceptionMiddleware>()
        .UseAuthentication()
        .UseAuthorization()
        .UseFastEndpoints(o =>
        {
            o.Endpoints.Configurator = ep =>
            {
                ep.DontAutoSendResponse();
                ep.PreProcessor<GlobalRequestLogger>(Order.Before);
                ep.PostProcessor<GlobalResponseSender>(Order.Before);
                ep.PostProcessor<GlobalResponseLogger>(Order.Before);
            };
        })
        .UseSwaggerGen();

    // Prometheus metrics endpoint
    app.MapMetrics();  //https://appserver/metrics

    app.MapFallbackToFile("index.html"); // Serves the Blazor app from the API, when no routes match.


   app.Run();
    
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program
{
    
}