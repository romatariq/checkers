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

namespace WebApp.Pages_GameStates
{
    public class IndexModel : PageModel
    {
        private readonly IGameStateRepository _gameStateRepository;

        public IndexModel(IGameStateRepository gameStateRepository)
        {
            _gameStateRepository = gameStateRepository;
        }

        public IList<GameState> GameState { get;set; } = new List<GameState>();

        public async Task OnGetAsync()
        {
            GameState = await _gameStateRepository.GetGameStatesAsync();
        }
    }
}
