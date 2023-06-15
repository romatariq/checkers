using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL.Db;
using DAL.FileSystem;
using Domain;
using NuGet.Packaging;

namespace WebApp.Pages_Games
{
    public class CreateModel : PageModel
    {
        private readonly IGameRepository _gameRepository;
        private readonly IGameSettingsRepository _gameSettingsRepository;
        
        [BindProperty]
        public Game Game { get; set; } = default!;
        
        [BindProperty]
        public GameSetting GameSetting { get; set; } = default!;
        
        public SelectList SettingsSelectList { get; set; } = default!;
        
        [BindProperty]
        public bool SelectedSettingMethod { get; set; }
        
        private List<GameSetting> GameSettingList { get; set; } = default!;

        public CreateModel(IGameRepository gameRepository, IGameSettingsRepository gameSettingsRepository)
        {
            _gameRepository = gameRepository;
            _gameSettingsRepository = gameSettingsRepository;
        }

        public async Task<IActionResult> OnGet()
        {
            GameSettingList = await GetSettingsSelectList();
            SettingsSelectList = new SelectList(GameSettingList, nameof(GameSetting.Id), "Name");
            return Page();
        }
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || Game == null)
          {
              GameSettingList = await GetSettingsSelectList();
              SettingsSelectList = new SelectList(GameSettingList, nameof(GameSetting.Id), "Name");
              return Page();
          }

          if (SelectedSettingMethod)
          {
              await _gameSettingsRepository.SaveGameSettingsAsync(GameSetting);
              Game.GameSettingId = GameSetting.Id;
              Game.GameSetting = GameSetting;
          }
          else if (_gameSettingsRepository.GetType() == typeof(GameSettingsRepositoryFileSystem))
          {
              Game.GameSetting = await _gameSettingsRepository.GetGameSettingsAsync(Game.GameSettingId);
          }

          await _gameRepository.SaveGameAsync(Game);

          if (Game.Player1Type == EPlayerType.Human && Game.Player2Type == EPlayerType.Human)
          {
              return RedirectToPage("./CreateGameLaunch", new {id = Game.Id});
          }
          if (Game.Player1Type == EPlayerType.Human)
          {
              return RedirectToPage("./CreateGameLaunch", new {id = Game.Id, player = "1"});
          }
          return RedirectToPage("./CreateGameLaunch", new {id = Game.Id, player = "2"});
        }
        
        
        private async Task<List<GameSetting>> GetSettingsSelectList()
        {
            var gameSettingsList = new List<GameSetting>();
            var pageCount = _gameSettingsRepository.GetPageCount();
            var page = 1;

            while (pageCount >= page)
            {
                gameSettingsList.AddRange(await _gameSettingsRepository.GetGameSettingsListAsync(page));
                page++;
            }
            
            return gameSettingsList;
        }
    }
}
