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
    public class DetailsModel : PageModel
    {
        private readonly IGameSettingsRepository _gameSettingsRepository;

        public DetailsModel(IGameSettingsRepository gameSettingsRepository)
        {
            _gameSettingsRepository = gameSettingsRepository;
        }

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
    }
}
