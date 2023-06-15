namespace Domain;

public class Button
{
    public ETeamColor Color { get; set; }
    public EButtonType Type { get; set; }
    
    public Button(ETeamColor color, EButtonType type = EButtonType.Normal)
    {
        Color = color;
        Type = type;
    }

    // only necessary for deserialization, should not be used otherwise!
    public Button()
    {
        Color = ETeamColor.Black;
        Type = EButtonType.Normal;
    }

    public override string ToString()
    {
        return Type.Equals(EButtonType.Normal) ? "O" : "X";
    }
}