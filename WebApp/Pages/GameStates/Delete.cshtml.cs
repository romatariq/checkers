using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL.Db;
using Domain;

namespace WebApp.Pages_GameStates
{
    public class DeleteModel : PageModel
    {
        private readonly IGameRepository _gameRepository;
        private readonly IGameStateRepository _gameStateRepository;

        public DeleteModel(IGameRepository gameRepository, IGameStateRepository gameStateRepository)
        {
            _gameRepository = gameRepository;
            _gameStateRepository = gameStateRepository;
        }

        [BindProperty]
      public GameState GameState { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameState = await _gameStateRepository.GetSavedGameStateAsync(id);
            
            if (gameState == null)
            {
                return NotFound();
            }
            GameState = gameState;
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var gameState = await _gameStateRepository.GetSavedGameStateAsync(id);
            var game = await _gameRepository.GetSavedGameAsync(gameState?.GameId);

            if (gameState == null || game?.GameStates == null)
            {
                return NotFound();
            }

            var gameStateToBeRemoved = game.GameStates.FirstOrDefault(x => x.Id == gameState.Id);

            if (gameStateToBeRemoved != null)
            {
                game.GameStates.Remove(gameStateToBeRemoved);
                await _gameRepository.SaveGameAsync(game);
            }

            return RedirectToPage("./Index");
        }
    }
}
