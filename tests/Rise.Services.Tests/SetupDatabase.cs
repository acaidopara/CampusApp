using Microsoft.EntityFrameworkCore;
using Rise.Persistence;

namespace Rise.Services.Tests;

public static class SetupDatabase
{
    public static async Task<ApplicationDbContext> CreateDbContextAsync()
    {
        var connectionString = $"Server=localhost;port=3306;Database=RiseTestDb;User=root;Password=root;";

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetectAsync(connectionString).Result)
            .Options;

        var db = new ApplicationDbContext(options);

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync(); 

        return db;
    }
}