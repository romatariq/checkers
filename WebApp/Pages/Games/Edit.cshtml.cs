using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.Db;
using Domain;

namespace WebApp.Pages_Games
{
    public class EditModel : PageModel
    {
        private readonly IGameRepository _gameRepository;
        private readonly IGameSettingsRepository _gameSettingsRepository;

        public EditModel(IGameRepository gameRepository, IGameSettingsRepository gameSettingsRepository)
        {
            _gameRepository = gameRepository;
            _gameSettingsRepository = gameSettingsRepository;
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
            ViewData["GameSettingId"] = new SelectList(_gameSettingsRepository.GetGameSettingsList(), "Id", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            

            if (!GameExists(Game.Id))
            {
                return NotFound();
            }
            await _gameRepository.SaveGameAsync(Game);

            return RedirectToPage("./Index");
        }

        private bool GameExists(int id)
        {
            return _gameRepository.GetSavedGame(id) != null;
        }
    }
}
