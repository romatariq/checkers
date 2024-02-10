using System.Text.Json;
using Domain;

namespace DAL.FileSystem;

public class GameSettingsRepositoryFileSystem : IGameSettingsRepository
{
    private const string FileExtension = "json";
    private JsonSerializerOptions JsonOptions { get; set; } = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true
    };

    public List<GameSetting> GetGameSettingsList(int page = 1, string? filter = null)
    {
        return GetAllGameSettings(filter)
            .Skip(Constants.GameSettingsPerPage * (page - 1))
            .Take(Constants.GameSettingsPerPage)
            .ToList();
    }

    public Task<List<GameSetting>> GetGameSettingsListAsync(int page = 1, string? filter = null)
    {
        return Task.FromResult(GetGameSettingsList(page, filter));
    }

    public int GetPageCount(string? filter = null)
    {
        var itemCount = GetAllGameSettings(filter).Count();
        return itemCount / Constants.GameSettingsPerPage + (itemCount % Constants.GameSettingsPerPage == 0 ? 0 : 1);
    }

    private IEnumerable<GameSetting> GetAllGameSettings(string? filter)
    {
        var items = Directory
            .GetFileSystemEntries(Constants.GameSettingsPath, $"*.{FileExtension}")
            .Select(file => Path.GetFileNameWithoutExtension(file))
            .Select(x => GetGameSettings(int.Parse(x))!);

        if (filter != null)
        {
            items = items
                .Where(s =>
                    s.Name.ToLower().Contains(filter.ToLower())
                    );
        }

        return items
            .OrderBy(s => s.Name);
    }


    public GameSetting? GetGameSettings(int id)
    {
        var fileName = GetFileNameWithPathAndExtension(id.ToString());
        
        if (!File.Exists(fileName)) return null;
        
        var fileContent = File.ReadAllText(GetFileNameWithPathAndExtension(id.ToString()));
        var settings = JsonSerializer.Deserialize<GameSetting>(fileContent, JsonOptions);
        
        if (settings == null)
        {
            throw new NullReferenceException($"Could not deserialize: {fileContent}");
        }
        return settings;
    }
    
    public Task<GameSetting?> GetGameSettingsAsync(int id)
    {
        return Task.FromResult(GetGameSettings(id));
    }

    
    public void SaveGameSettings(GameSetting setting)
    {
        if (setting.Id == 0)
        {
            setting.Id = setting.GetHashCode();
        }

        var fileContent = JsonSerializer.Serialize(setting, JsonOptions);
        File.WriteAllText(GetFileNameWithPathAndExtension(setting.Id.ToString()),fileContent);
    }

    public Task SaveGameSettingsAsync(GameSetting setting)
    {
        if (setting.Id == 0)
        {
            setting.Id = setting.GetHashCode();
        }

        var fileContent = JsonSerializer.Serialize(setting, JsonOptions);
        return File.WriteAllTextAsync(GetFileNameWithPathAndExtension(setting.Id.ToString()),fileContent);
    }
    
    
    public void DeleteGameSettings(int id)
    {
        if (CheckIfSettingIsUsed(id))
        {
            return;
        }

        File.Delete(GetFileNameWithPathAndExtension(id.ToString()));
    }
    
    public Task DeleteGameSettingsAsync(int id)
    {
        DeleteGameSettings(id);
        return Task.CompletedTask;
    }
    
    

    private static bool CheckIfSettingIsUsed(int id)
    {
        return id is 1 or 2 || 
               new GameRepositoryFileSystem()
            .GetSavedGamesList()
            .Select(x => x.GameSetting!.Id)
            .Contains(id);
    }
    
    private string GetFileNameWithPathAndExtension(string id)
    {
        return $"{Constants.GameSettingsPath}{Path.DirectorySeparatorChar}{id}.{FileExtension}";
    }

}