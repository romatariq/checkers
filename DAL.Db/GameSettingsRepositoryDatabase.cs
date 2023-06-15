using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL.Db;

public class GameSettingsRepositoryDatabase : IGameSettingsRepository
{
    private readonly AppDbContext _dbContext;
    
    public GameSettingsRepositoryDatabase(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public List<GameSetting> GetGameSettingsList(int page = 1, string? filter = null)
    {
        return GetQuery(filter)
            .Skip(Constants.GameSettingsPerPage * (page - 1))
            .Take(Constants.GameSettingsPerPage)
            .ToList();
    }
    
    public Task<List<GameSetting>> GetGameSettingsListAsync(int page = 1, string? filter = null)
    {
        return GetQuery(filter)
            .Skip(Constants.GameSettingsPerPage * (page - 1))
            .Take(Constants.GameSettingsPerPage)
            .ToListAsync();
    }

    private IQueryable<GameSetting> GetQuery(string? filter)
    {
        var query = _dbContext
            .GameSettings
            .OrderBy(s => s.Name)
            .AsQueryable();

        if (filter != null)
        {
            query = query
                .Where(s => 
                    s.Name.ToLower().Contains(filter.ToLower())
                );
        }

        return query;
    }

    public int GetPageCount(string? filter = null)
    {
        var itemCount = GetQuery(filter).Count();
        return itemCount / Constants.GameSettingsPerPage + (itemCount % Constants.GameSettingsPerPage == 0 ? 0 : 1);
    }

    public GameSetting? GetGameSettings(int id)
    {
        return _dbContext
            .GameSettings
            .FirstOrDefault(s => s.Id == id);
    }
    
    public Task<GameSetting?> GetGameSettingsAsync(int id)
    {
        return _dbContext
            .GameSettings
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public void SaveGameSettings(GameSetting setting)
    {
        var settingsFromDb = _dbContext.GameSettings
            .FirstOrDefault(s => s.Id == setting.Id);

        if (settingsFromDb == null)
        {
            _dbContext.GameSettings.Add(setting);
            _dbContext.SaveChanges();
            return;
        }

        settingsFromDb.Name = setting.Name;
        settingsFromDb.BoardHeight = setting.BoardHeight;
        settingsFromDb.BoardWidth = setting.BoardWidth;
        settingsFromDb.WhiteStarts = setting.WhiteStarts;
        settingsFromDb.CanEatBackwards = setting.CanEatBackwards;
        settingsFromDb.HasToCapture = setting.HasToCapture;
        settingsFromDb.AllButtonCanEatKing = setting.AllButtonCanEatKing;
        settingsFromDb.KingCanMoveOnlyOneStep = setting.KingCanMoveOnlyOneStep;
        _dbContext.SaveChanges();
    }
    
    public Task SaveGameSettingsAsync(GameSetting setting)
    {
        var settingsFromDb = _dbContext.GameSettings
            .FirstOrDefaultAsync(s => s.Id == setting.Id).Result;

        if (settingsFromDb == null)
        {
            _dbContext.GameSettings.Add(setting);
            return _dbContext.SaveChangesAsync();
        }

        settingsFromDb.Name = setting.Name;
        settingsFromDb.BoardHeight = setting.BoardHeight;
        settingsFromDb.BoardWidth = setting.BoardWidth;
        settingsFromDb.WhiteStarts = setting.WhiteStarts;
        settingsFromDb.CanEatBackwards = setting.CanEatBackwards;
        settingsFromDb.HasToCapture = setting.HasToCapture;
        settingsFromDb.AllButtonCanEatKing = setting.AllButtonCanEatKing;
        settingsFromDb.KingCanMoveOnlyOneStep = setting.KingCanMoveOnlyOneStep;
        return _dbContext.SaveChangesAsync();
    }

    public void DeleteGameSettings(int id)
    {
        if (CheckIfSettingIsUsed(id))
        {
            return;
        }
        var settingsFromDb = GetGameSettings(id);
        if (settingsFromDb == null) return;
        
        _dbContext.GameSettings.Remove(settingsFromDb);
        _dbContext.SaveChanges();
    }
    
    public Task DeleteGameSettingsAsync(int id)
    {
        if (CheckIfSettingIsUsed(id))
        {
            return Task.CompletedTask;
        }
        var settingsFromDb = GetGameSettingsAsync(id).Result;
        if (settingsFromDb == null) return Task.CompletedTask;
        
        _dbContext.GameSettings.Remove(settingsFromDb);
        return _dbContext.SaveChangesAsync();
    }

    private bool CheckIfSettingIsUsed(int id)
    {
        return id is 1 or 2 ||
               _dbContext.Games
            .Select(x => x.GameSetting!.Id)
            .Contains(id);
    }
}