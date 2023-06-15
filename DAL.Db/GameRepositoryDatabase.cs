using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL.Db;

public class GameRepositoryDatabase : IGameRepository
{
    private readonly AppDbContext _dbContext;

    public GameRepositoryDatabase(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Game> GetSavedGamesList(int page = 1, string? filter = null)
    {
        return GetQuery(filter)
            .Skip(Constants.GamesPerPage * (page - 1))
            .Take(Constants.GamesPerPage)
            .Include(g => g.GameSetting)
            .Include(g => g.GameStates)
            .ToList();
    }
    
    public Task<List<Game>> GetSavedGamesListAsync(int page = 1, string? filter = null)
    {
        return GetQuery(filter)
            .Skip(Constants.GamesPerPage * (page - 1))
            .Take(Constants.GamesPerPage)
            .Include(g => g.GameSetting)
            .Include(g => g.GameStates)
            .ToListAsync();
    }
    
    public int GetPageCount(string? filter = null)
    {
        var itemCount = GetQuery(filter).Count();   
        return itemCount / Constants.GamesPerPage + (itemCount % Constants.GamesPerPage == 0 ? 0 : 1);
    }

    private IQueryable<Game> GetQuery(string? filter)
    {
        var query = _dbContext
            .Games
            .Include(g => g.GameSetting)
            .OrderBy(g => g.Name)
            .AsQueryable();

        if (filter != null)
        {
            var filterLower = filter.ToLower();
            query = query
                .Where(g =>
                    g.Name.ToLower().Contains(filterLower) ||
                    g.Player1Name.ToLower().Contains(filterLower) ||
                    g.Player2Name.ToLower().Contains(filterLower) ||
                    g.GameSetting!.Name.ToLower().Contains(filterLower)
                );
        }

        return query;
    }

    public Game? GetSavedGame(int? id)
    {
        return _dbContext.Games
            .Include(g => g.GameSetting)
            .Include(g => g.GameStates)
            .FirstOrDefault(x => x.Id == id);
    }

    public Task<Game?> GetSavedGameAsync(int? id)
    {
        return _dbContext.Games
            .Include(g => g.GameSetting)
            .Include(g => g.GameStates)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public void SaveGame(Game game)
    {
        var gameFromDbId = _dbContext.Games.Find(game.Id);

        if (gameFromDbId == null)
        {
            _dbContext.Games.Add(game);
            _dbContext.SaveChanges();
            return;
        }

        gameFromDbId.Name = game.Name;
        gameFromDbId.Player1Name = game.Player1Name;
        gameFromDbId.Player2Name = game.Player2Name;
        gameFromDbId.Player1Type = game.Player1Type;
        gameFromDbId.Player2Type = game.Player2Type;
        _dbContext.SaveChanges();
    }
    
    public Task SaveGameAsync(Game game)
    {
        var gameFromDbId = _dbContext.Games.Find(game.Id);

        if (gameFromDbId == null)
        {
            _dbContext.Games.Add(game);
            return _dbContext.SaveChangesAsync();
        }

        gameFromDbId.Name = game.Name;
        gameFromDbId.Player1Name = game.Player1Name;
        gameFromDbId.Player2Name = game.Player2Name;
        gameFromDbId.Player1Type = game.Player1Type;
        gameFromDbId.Player2Type = game.Player2Type;
        return _dbContext.SaveChangesAsync();
    }

    public void DeleteSavedGame(int id)
    {
        var gameFromDb = GetSavedGame(id);

        if (gameFromDb == null)
        {
            return;
        }

        _dbContext.Remove(gameFromDb);
        _dbContext.SaveChanges();
    }
    
    public Task DeleteSavedGameAsync(int id)
    {
        var gameFromDb = GetSavedGameAsync(id).Result;

        if (gameFromDb == null)
        {
            return Task.CompletedTask;
        }

        _dbContext.Remove(gameFromDb);
        return _dbContext.SaveChangesAsync();
    }
}