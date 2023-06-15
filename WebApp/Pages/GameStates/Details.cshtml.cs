using System;
using System.Collections;
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
    public class DetailsModel : PageModel
    {
        private readonly IGameStateRepository _gameStateRepository;

        public DetailsModel(IGameStateRepository gameStateRepository)
        {
            _gameStateRepository = gameStateRepository;
        }

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
    }
}
