using DAL;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages_Games;

public class Play : PageModel
{
    private readonly IGameStateRepository _gameStateRepository;
    private readonly IGameRepository _gameRepository;
    
    public GameRunner GameRunner { get; set; } = default!;
    public Game Game { get; set; } = default!;
    public string Player { get; set; } = default!;
    public string Color { get; set; } = default!;
    public int StateId { get; set; }
    public int CurrentMostRecentStateId { get; set; }

    public Play(IGameRepository gameRepository, IGameStateRepository gameStateRepository)
    {
        _gameStateRepository = gameStateRepository;
        _gameRepository = gameRepository;
    }
    
    public async Task<IActionResult> OnGet(int? id, string? player, string? color, int? stateId, int? fromX, int? fromY, int? toX, int? toY)
    {
        if (id == null || player == null || player is not ("1" or "2"))
        {
            return NotFound();
        }
        
        var game = await _gameRepository.GetSavedGameAsync(id);

        if (game == null) return NotFound();

        Player = player;
        Game = game;
        
        if (stateId == null)
        {
            GameRunner = new GameRunner(Game, _gameRepository);
            StateId = Game.GetTheLastState().Id;
            CurrentMostRecentStateId = StateId;
        }
        else
        {
            StateId = stateId.Value;
            if (Game.GameStates!.FirstOrDefault(s => s.Id == StateId) == null) return NotFound();
            
            CurrentMostRecentStateId = Game.GetTheLastState().Id;
            GameRunner = new GameRunner(Game, _gameRepository, StateId);
        }

        
        if (GameRunner.CurrentPlayer.Type != EPlayerType.Human && !GameRunner.GameIsOver)
        {
            var moveWasSuccessful = GameRunner.StartAi();
            if (moveWasSuccessful)
            {
                StateId = Game.GetTheLastState().Id;
                CurrentMostRecentStateId = StateId;
            }
        }

        else if (fromX != null && fromY != null && toX != null && toY != null 
            && (GameRunner.CurrentPlayer.Color == ETeamColor.White && Player == "1" 
                || GameRunner.CurrentPlayer.Color == ETeamColor.Black && Player == "2"))
        {
            var moveWasSuccessful = GameRunner.MoveButton(new Tuple<int, int>(fromX.Value, fromY.Value), new Tuple<int, int>(toX.Value, toY.Value));
            if (moveWasSuccessful)
            {
                StateId = Game.GetTheLastState().Id;
                CurrentMostRecentStateId = StateId;
            }
        }

        Color = color switch
        {
            "purple" => "purple",
            "brown" => "brown",
            "blue" => "blue",
            "black" => "black",
            _ => "green"
        };

        return Page();
    }
}