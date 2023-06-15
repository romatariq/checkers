namespace DAL;

public static class Constants
{
    private static readonly char Separator = Path.DirectorySeparatorChar;
    public static readonly string SavingLocation = $"C:{Separator}common{Separator}checkers{Separator}";
    public const int GamesPerPage = 4;
    public const int GameSettingsPerPage = 8;
}