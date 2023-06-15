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
    public class DeleteModel : PageModel
    {
        private readonly IGameRepository _gameRepository;

        public DeleteModel(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _gameRepository.DeleteSavedGameAsync(id.Value);

            return RedirectToPage("./Index");
        }
    }
}
