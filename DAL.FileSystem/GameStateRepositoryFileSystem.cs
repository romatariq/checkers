using Domain;

namespace DAL.FileSystem;

public class GameStateRepositoryFileSystem: IGameStateRepository
{
    private readonly GameRepositoryFileSystem _gameRepository = new();
    
    public List<GameState> GetGameStates()
    {
        var gameStates = new List<GameState>();

        var gamesList = _gameRepository.GetSavedGamesList();
        var gamePageCount = _gameRepository.GetPageCount();
        var gamePage = 2;

        while (gamePageCount >= gamePage)
        {
            gamesList.AddRange(_gameRepository.GetSavedGamesList(gamePage));
            gamePage++;
        }
        
        foreach (var game in gamesList)
        {
            if (game.GameStates == null) continue;
                
            foreach (var state in game.GameStates)
            {
                state.Game = game;
                state.GameId = game.Id;
                gameStates.Add(state);
            }
        }

        return gameStates.OrderByDescending(x => x.CreatedAt).ToList();
    }
    
    public Task<List<GameState>> GetGameStatesAsync()
    {
        return Task.FromResult(GetGameStates());
    }


    public GameState? GetSavedGameState(int? id)
    {
        return GetGameStates()
            .FirstOrDefault(x => x.Id == id);

    }
    
    public Task<GameState?> GetSavedGameStateAsync(int? id)
    {
        return Task.FromResult(GetSavedGameState(id));    
    }
}