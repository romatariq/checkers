using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL.Db;
using Domain;

namespace WebApp.Pages_GameSettings
{
    public class CreateModel : PageModel
    {
        private readonly IGameSettingsRepository _gameSettingsRepository;

        public CreateModel(IGameSettingsRepository gameSettingsRepository)
        {
            _gameSettingsRepository = gameSettingsRepository;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public GameSetting GameSetting { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || GameSetting == null)
            {
                return Page();
            }

            await _gameSettingsRepository.SaveGameSettingsAsync(GameSetting);

            return RedirectToPage("./Index");
        }
    }
}
