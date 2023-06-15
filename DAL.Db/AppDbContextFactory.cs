using DAL.FileSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DAL.Db;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public static readonly string SqliteLocation = $"{Constants.SavingLocation}app.db";

    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite($"Data Source={SqliteLocation}");

        return new AppDbContext(optionsBuilder.Options);
    }
}