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
    public class IndexModel : PageModel
    {
        private readonly IGameSettingsRepository _gameSettingsRepository;

        public int PageNr { get; set; } = 1;
        public int? LastPageNr { get; set; }
        public int? NextPageNr { get; set; }
        
        public string? Filter { get; set; }


        public IndexModel(IGameSettingsRepository gameSettingsRepository)
        {
            _gameSettingsRepository = gameSettingsRepository;
        }

        public IList<GameSetting> GameSetting { get;set; } = default!;

        public async Task OnGetAsync(int? pageNr, string? filter)
        {
            Filter = filter == "" ? null : filter;
            var pageCount = _gameSettingsRepository.GetPageCount(Filter);
            
            if (pageNr == null || pageNr < 1 || pageNr > pageCount)
            {
                pageNr = 1;
            }
            PageNr = pageNr.Value;
            if (PageNr > 1)
            {
                LastPageNr = PageNr - 1;
            }

            if (PageNr < pageCount)
            {
                NextPageNr = PageNr + 1;
            }
            GameSetting = await _gameSettingsRepository.GetGameSettingsListAsync(PageNr, Filter);
        }
    }
}
