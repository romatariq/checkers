using Domain;

namespace GameBrain;

public class Player
{

    public ETeamColor Color { get; set; }
    public EPlayerType Type { get; set; }
    public string Name { get; set; }
    
    public Player(ETeamColor color, EPlayerType type, string name)
    {
        Color = color;
        Type = type;
        Name = name;
    }
    
}