using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages_Games;

public class CreateGameLaunch : PageModel
{

    public int GameId { get; set; }
    
    
    public IActionResult OnGet(int? id, string? player)
    {
        if (id == null)
        {
            return RedirectToPage("./Index");
        }
        
        // go to human vs human game
        if (player == null)
        {
            GameId = id.Value;
            return Page();
        }
        return RedirectToPage("./Play", new {id = id, player = player});
    }
}