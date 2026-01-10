using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rise.Persistence;
using Rise.Persistence.Triggers;
using Rise.Server;
using Rise.Shared.Identity.Accounts;
using Identity = Rise.Server.Endpoints.Identity;

namespace Rise.Integration.Tests;

public class App : AppFixture<Program>
{
    public ApplicationDbContext Db { get; private set; } = null!;
    public HttpClient Student { get; private set; } = null!;
    public HttpClient TestClient { get; set; } = null!;

    public string STUDENT_EMAIL = "jane.doe@student.hogent.be";
    public string TEST_EMAIL = "student1@student.hogent.be";
    public string STUDENT_PWD = "A1b2C3!";

    private string _testConnectionString = null!;

    protected override void ConfigureApp(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d =>
            d.ServiceType == typeof(ApplicationDbContext));

        if (descriptor != null)
            services.Remove(descriptor);

        _testConnectionString =
            $"Server=localhost;Port=3306;Database=RiseTestDb;User=root;Password=root;";

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseMySql(_testConnectionString, ServerVersion.AutoDetect(_testConnectionString));
            options.UseTriggers(o => o.AddTrigger<EntityBeforeSaveTrigger>());
        });
    }

    protected override IHost ConfigureAppHost(IHostBuilder builder)
    {
        var host = base.ConfigureAppHost(builder);

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();

        db.Database.EnsureDeleted();
        db.Database.Migrate();
        seeder.SeedAsync().GetAwaiter().GetResult();

        Db = db;

        return host;
    }

    protected override async ValueTask SetupAsync()
    {
        Student = await Login(STUDENT_EMAIL, STUDENT_PWD);
        TestClient = CreateClient();
    }

    public async Task<HttpClient> Login(string email, string pwd)
    {
        var client = CreateClient();
        var req = new AccountRequest.Login { Email = email, Password = pwd };
        await client.POSTAsync<Identity.Accounts.Login, AccountRequest.Login>(req);
        return client;
    }

    protected override ValueTask TearDownAsync()
    {
        return ValueTask.CompletedTask;
    }
}

public class TestCollection : TestCollection<App>;
