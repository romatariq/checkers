using Domain;

namespace DAL;

public interface IGameSettingsRepository
{
    List<GameSetting> GetGameSettingsList(int page = 1, string? filter = null);

    GameSetting? GetGameSettings(int id);

    void SaveGameSettings(GameSetting setting);

    void DeleteGameSettings(int id);
    
    
    Task<List<GameSetting>> GetGameSettingsListAsync(int page = 1, string? filter = null);

    Task<GameSetting?> GetGameSettingsAsync(int id);
    
    Task SaveGameSettingsAsync(GameSetting setting);
    
    Task DeleteGameSettingsAsync(int id);
    
    
    int GetPageCount(string? filter = null);
}