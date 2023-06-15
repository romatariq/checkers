using Domain;

namespace DAL;

public interface IGameStateRepository
{
    List<GameState> GetGameStates();
    
    GameState? GetSavedGameState(int? id);
    
    
    Task<List<GameState>> GetGameStatesAsync();
    
    Task<GameState?> GetSavedGameStateAsync(int? id);
}