using DAL;
using DAL.Db;
using DAL.FileSystem;
using Domain;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var connectionString = $"DataSource={Constants.DatabasePath};Cache=Shared";
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<IGameRepository, GameRepositoryDatabase>();
builder.Services.AddScoped<IGameSettingsRepository, GameSettingsRepositoryDatabase>();
builder.Services.AddScoped<IGameStateRepository, GameStateRepositoryDatabase>();

// builder.Services.AddScoped<IGameRepository, GameRepositoryFileSystem>();
// builder.Services.AddScoped<IGameSettingsRepository, GameSettingsRepositoryFileSystem>();
// builder.Services.AddScoped<IGameStateRepository, GameStateRepositoryFileSystem>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// adds 2 default settings to db and fs
InitializeGameModeSettings();

app.Run();

void InitializeGameModeSettings()
{
    var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlite($"Data Source={Constants.DatabasePath}")
        .Options;
    var ctx = new AppDbContext(dbOptions);

    IGameSettingsRepository gameSettingsRepositoryFs = new GameSettingsRepositoryFileSystem();
    IGameSettingsRepository gameSettingsRepositoryDb = new GameSettingsRepositoryDatabase(ctx);
    
    var gameModeSettings = new GameSetting
    {
        Id = 1,
        Name = "classic"
    };
    gameSettingsRepositoryFs.SaveGameSettings(gameModeSettings);
    gameSettingsRepositoryDb.SaveGameSettings(gameModeSettings);

    
    gameModeSettings = new GameSetting
    {
        Id = 2,
        Name = "russian",
        BoardHeight = 10,
        BoardWidth = 10,
        KingCanMoveOnlyOneStep = false,
    };
    gameSettingsRepositoryFs.SaveGameSettings(gameModeSettings);
    gameSettingsRepositoryDb.SaveGameSettings(gameModeSettings);
}