using GameBrain;

namespace Domain;

public class BoardSquare
{
    public ETeamColor Color { get; set; }
    public Button? Button { get; set; }
    
    public BoardSquare(ETeamColor color, Button? button = null)
    {
        Color = color;
        Button = button;
    }
    
    // only necessary for deserialization, should not be used otherwise!
    public BoardSquare()
    {
        Color = ETeamColor.Black;
        Button = null;
    }
}