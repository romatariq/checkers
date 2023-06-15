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

namespace WebApp.Pages_GameSettings
{
    public class DeleteModel : PageModel
    {
        private readonly IGameSettingsRepository _gameSettingsRepository;

        public DeleteModel(IGameSettingsRepository gameSettingsRepository)
        {
            _gameSettingsRepository = gameSettingsRepository;
        }

        [BindProperty]
      public GameSetting GameSetting { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameSetting = await _gameSettingsRepository.GetGameSettingsAsync(id.Value);

            if (gameSetting == null)
            {
                return NotFound();
            }
            GameSetting = gameSetting;
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            await _gameSettingsRepository.DeleteGameSettingsAsync(id.Value);


            return RedirectToPage("./Index");
        }
    }
}
