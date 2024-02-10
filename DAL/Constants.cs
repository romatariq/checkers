namespace DAL;

public static class Constants
{
    public const int GamesPerPage = 4;
    public const int GameSettingsPerPage = 8;

    private static readonly string SavingLocation = Path.Combine(GetSolutionDirectoryPath(), "data");
    public static readonly string DatabasePath = Path.Combine(SavingLocation, "app.db");
    public static readonly string GamesPath = GetOrCreatePath("games");
    public static readonly string GameSettingsPath = GetOrCreatePath("settings");
    
    private static string GetOrCreatePath(string subPath)
    {
        var path = Path.Combine(SavingLocation, subPath);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }
    
    private static string GetSolutionDirectoryPath()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (directory != null && !directory.GetFiles("*.sln").Any())
        {
            directory = directory.Parent;
        }
        
        return directory?.FullName ?? throw new Exception("Solution directory not found");
    }
}