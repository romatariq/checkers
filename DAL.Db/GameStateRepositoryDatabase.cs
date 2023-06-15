using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL.Db;

public class GameStateRepositoryDatabase: IGameStateRepository
{
    private readonly AppDbContext _dbContext;

    public GameStateRepositoryDatabase(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public List<GameState> GetGameStates()
    {
        return _dbContext
            .GameStates
            .Include(x => x.Game)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }
    
    public Task<List<GameState>> GetGameStatesAsync()
    {
        return _dbContext
            .GameStates
            .Include(x => x.Game)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public GameState? GetSavedGameState(int? id)
    {
        return _dbContext
            .GameStates
            .Include(x => x.Game)
            .FirstOrDefault(x => x.Id == id);
    }
    
    public Task<GameState?> GetSavedGameStateAsync(int? id)
    {
        return _dbContext
            .GameStates
            .Include(x => x.Game)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}