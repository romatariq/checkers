using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL.Db;

public class AppDbContext : DbContext
{
    public DbSet<Game> Games { get; set; } = default!;
    public DbSet<GameState> GameStates { get; set; } = default!;
    public DbSet<GameSetting> GameSettings { get; set; } = default!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}