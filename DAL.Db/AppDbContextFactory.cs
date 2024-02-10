using DAL.FileSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DAL.Db;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{

    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite($"Data Source={Constants.DatabasePath}");

        return new AppDbContext(optionsBuilder.Options);
    }
}