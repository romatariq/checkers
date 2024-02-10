using System.Text.Json;
using Domain;

namespace DAL.FileSystem;

public class GameRepositoryFileSystem : IGameRepository
{
    private const string FileExtension = "json";
    private JsonSerializerOptions JsonOptions { get; set; } = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true
    };
    
    public List<Game> GetSavedGamesList(int page = 1, string? filter = null)
    {
        return GetAllGames(filter)
            .Skip(Constants.GamesPerPage * (page - 1))
            .Take(Constants.GamesPerPage)
            .ToList();
    }

    public int GetPageCount(string? filter = null)
    {
        var itemCount = GetAllGames(filter).Count();
        return itemCount / Constants.GamesPerPage + (itemCount % Constants.GamesPerPage == 0 ? 0 : 1);
    }
    
    private IEnumerable<Game> GetAllGames(string? filter)
    {
        var items = Directory
            .GetFileSystemEntries(Constants.GamesPath, $"*.{FileExtension}")
            .Select(file => Path.GetFileNameWithoutExtension(file))
            .Select(x => GetSavedGame(int.Parse(x))!);

        if (filter != null)
        {
            var filterLower = filter.ToLower();
            items = items
                .Where(g =>
                    g.Name.ToLower().Contains(filterLower) ||
                    g.Player1Name.ToLower().Contains(filterLower) ||
                    g.Player2Name.ToLower().Contains(filterLower) ||
                    g.GameSetting!.Name.ToLower().Contains(filterLower)
                );
        }
        return items
            .OrderBy(g => g.Name);
    }

    public Task<List<Game>> GetSavedGamesListAsync(int page = 1, string? filter = null)
    {
        return Task.FromResult(GetSavedGamesList(page, filter));
    }

    
    public Game? GetSavedGame(int? id)
    {
        if (id == null)
        {
            return null;
        }

        string fileContent;
        try
        {
            fileContent = File.ReadAllText(GetFileNameWithPathAndExtension(id.Value.ToString()));

        }
        catch (FileNotFoundException)
        {
            return null;
        }
        
        var game = JsonSerializer.Deserialize<Game>(fileContent, JsonOptions);
        
        if (game == null)
        {
            throw new NullReferenceException($"Could not deserialize: {fileContent}");
        }

        return game;
    }

    public Task<Game?> GetSavedGameAsync(int? id)
    {
        return Task.FromResult(GetSavedGame(id));
    }

    
    public void SaveGame(Game game)
    {
        if (game.Id != 0 && game.GameSettingId == 0)
        {
            var gameFromFs = GetSavedGame(game.Id);

            game.GameSettingId = gameFromFs!.GameSettingId;
            game.GameSetting = gameFromFs.GameSetting;
            game.GameStates = gameFromFs.GameStates;
        }

        if (game.GameSettingId == 0 && game.GameSetting != null)
        {
            game.GameSettingId = game.GameSetting.Id;
        }
        
        
        if (game.Id == 0)
        {
            game.Id = game.GetHashCode();
            
        }

        if (game.GameStates != null)
        {
            foreach (var state in game.GameStates)
            {
                state.GameId = game.Id;
                if (state.Id == 0)
                {
                    state.Id = state.GetHashCode();
                }
            }    
        }

        var fileContent = JsonSerializer.Serialize(game, JsonOptions);
        File.WriteAllText(GetFileNameWithPathAndExtension(game.Id.ToString()),fileContent);
    }
        
    public Task SaveGameAsync(Game game)
    {
        if (game.Id != 0 && game.GameSettingId == 0)
        {
            var gameFromFs = GetSavedGame(game.Id);

            game.GameSettingId = gameFromFs!.GameSettingId;
            game.GameSetting = gameFromFs.GameSetting;
            game.GameStates = gameFromFs.GameStates;
        }

        if (game.GameSettingId == 0 && game.GameSetting != null)
        {
            game.GameSettingId = game.GameSetting.Id;
        }
        
        
        if (game.Id == 0)
        {
            game.Id = game.GetHashCode();
            
        }

        if (game.GameStates != null)
        {
            foreach (var state in game.GameStates)
            {
                state.GameId = game.Id;
                if (state.Id == 0)
                {
                    state.Id = state.GetHashCode();
                }
            }    
        }

        var fileContent = JsonSerializer.Serialize(game, JsonOptions);
        return File.WriteAllTextAsync(GetFileNameWithPathAndExtension(game.Id.ToString()),fileContent);
    }

    
    public void DeleteSavedGame(int id)
    {
        File.Delete(GetFileNameWithPathAndExtension(id.ToString()));
    }
    
    public Task DeleteSavedGameAsync(int id)
    {
        DeleteSavedGame(id);
        return Task.CompletedTask;
    }

    
    private string GetFileNameWithPathAndExtension(string name)
    {
        return $"{Constants.GamesPath}{Path.DirectorySeparatorChar}{name}.{FileExtension}";
    }
}