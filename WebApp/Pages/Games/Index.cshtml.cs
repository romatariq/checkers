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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages_Games
{
    public class IndexModel : PageModel
    {
        private readonly IGameRepository _gameRepository;

        public IndexModel(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        [BindProperty]
        public int SelectedGameStateId { get; set; }
        
        [BindProperty]
        public int SelectedPlayer { get; set; }
        
        [BindProperty]
        public int SelectedGameId { get; set; }

        public IList<Game> Game { get;set; } = default!;
        
        
        public int PageNr { get; set; } = 1;
        public int? LastPageNr { get; set; }
        public int? NextPageNr { get; set; }

        public string? Filter { get; set; }
        
        
        public async Task OnGetAsync(int? pageNr, string? filter)
        {
            Filter = filter == "" ? null : filter;
            var pageCount = _gameRepository.GetPageCount(Filter);
            
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
            Game = await _gameRepository.GetSavedGamesListAsync(PageNr, Filter);
        }

        public Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Task.FromResult<IActionResult>(Page());
            }
            

            return Task.FromResult<IActionResult>(RedirectToPage("./Play", new {
                id = SelectedGameId,
                player = SelectedPlayer,
                stateId = SelectedGameStateId
            }));
        }
    }
}
