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

namespace WebApp.Pages_Games
{
    public class DetailsModel : PageModel
    {
        private readonly IGameRepository _gameRepository;

        public DetailsModel(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

      public Game Game { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _gameRepository.GetSavedGameAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            Game = game;
            return Page();
        }
    }
}
