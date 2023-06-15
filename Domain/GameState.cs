namespace Domain;

using System.Text.Json;


public class GameState
{
    public int Id { get; set; }

    public int GameId { get; set; }
    public Game? Game { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public DateTime? EndedAt { get; set; }
    
    public ETeamColor CurrentPlayer { get; set; }

    public int? StartFromX { get; set; }
    public int? StartFromY { get; set; }

    public string SerializedBoard { get; set; } = default!;

    public static string SerializeBoard(BoardSquare[][] board)
    {
        return JsonSerializer.Serialize(board);
    }
    
    public static BoardSquare[][] DeserializeBoard(string board)
    {
        return JsonSerializer.Deserialize<BoardSquare[][]>(board)!;
    }
}