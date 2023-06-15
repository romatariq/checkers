using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages_Games;

public class GameStateAPI : PageModel
{
    private readonly IGameRepository _gameRepository;

    public int StateId { get; set; }

    public GameStateAPI(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }
    
    public async Task<JsonResult> OnGetAsync(int? gameId)
    {
        var game = await _gameRepository.GetSavedGameAsync(gameId);

        StateId = game?.GetTheLastState().Id ?? 0;

        return new JsonResult(new
        {
            stateId = StateId
        });
    }
}