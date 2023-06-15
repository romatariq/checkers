using Domain;

namespace DAL;

public interface IGameRepository
{
    List<Game> GetSavedGamesList(int page = 1, string? filter = null);

    Game? GetSavedGame(int? id);

    void SaveGame(Game game);

    void DeleteSavedGame(int id);


    Task<List<Game>> GetSavedGamesListAsync(int page = 1, string? filter = null);

    Task<Game?> GetSavedGameAsync(int? id);

    Task SaveGameAsync(Game game);

    Task DeleteSavedGameAsync(int id);
    
    
    int GetPageCount(string? filter = null);
}