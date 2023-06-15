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

namespace WebApp.Pages_GameSettings
{
    public class EditModel : PageModel
    {
        private readonly IGameSettingsRepository _gameSettingsRepository;

        public EditModel(IGameSettingsRepository gameSettingsRepository)
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            

            if (!GameSettingExists(GameSetting.Id))
            {
                return NotFound();
            }
            await _gameSettingsRepository.SaveGameSettingsAsync(GameSetting);

            return RedirectToPage("./Index");
        }

        private bool GameSettingExists(int id)
        {
            return _gameSettingsRepository.GetGameSettings(id) != null;
        }
    }
}
