using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Game
{
    
    public int Id { get; set; }

    [MaxLength(128)]
    public string Name { get; set; } = default!;
    
    public DateTime StartedAt { get; set; } = DateTime.Now;
    
    [MaxLength(128)]
    public string Player1Name { get; set; } = default!;
    
    public EPlayerType Player1Type { get; set; }
    
    [MaxLength(128)]
    public string Player2Name { get; set; } = default!;
    
    public EPlayerType Player2Type { get; set; }

    public int GameSettingId { get; set; }
    public GameSetting? GameSetting { get; set; }
    
    public ICollection<GameState>? GameStates { get; set; }

    public GameState GetTheLastState()
    {
        return GameStates!.MaxBy(x => x.CreatedAt)!;
    }
    
    public GameState GetTheFirstState()
    {
        return GameStates!.MinBy(x => x.CreatedAt)!;
    }
}