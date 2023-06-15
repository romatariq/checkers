using System.ComponentModel.DataAnnotations;

namespace Domain;

public record GameSetting
{
    public int Id { get; set; }

    [MaxLength(128)]
    public string Name { get; set; } = default!;
    
    [Range(4, 26)]
    public int BoardWidth { get; set; } = 8;
    [Range(4, 99)]
    public int BoardHeight { get; set; } = 8;
    
    public bool HasToCapture { get; set; } = true;
    public bool CanEatBackwards { get; set; } = true;
    public bool WhiteStarts { get; set; } = true;
    public bool KingCanMoveOnlyOneStep { get; set; } = true;
    public bool AllButtonCanEatKing { get; set; } = true;
    
    public ICollection<Game>? Games { get; set; }
}