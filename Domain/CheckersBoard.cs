namespace Domain;

public class CheckersBoard
{
    public int Row { get; }
    public int Column { get; }
    public BoardSquare[][] Board { get; set; }
    public const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public CheckersBoard(BoardSquare[][] board)
    {
        Board = board;
        Row = board.Length;
        Column = board[0].Length;
    }

    public CheckersBoard(int width, int height)
    {
        if (width > Alphabet.Length)
        {
            throw new Exception($"Board width cannot be larger than {Alphabet.Length}");
        }

        if (height < 3 || width < 3)
        {
            throw new Exception("Board has to be bigger");
        }
        Row = height;
        Column = width;
        Board = new BoardSquare[Row][];
        for (int i = 0; i < Row; i++)
        {
            Board[i] = new BoardSquare[Column];
        }
        FillBoard();
    }

    private void FillBoard()
    {

        var previousColor = ETeamColor.Black;
        var howManyRowsOfButtons = (Row - 2) / 2;
        for (int i = 0; i < Row; i++)
        {
            if (i != 0 && Column % 2 == 0)
            {
                previousColor = ToggleColor(previousColor);
            }
            
            for (int j = 0; j < Column; j++)
            {
                previousColor = ToggleColor(previousColor);
                if (previousColor == ETeamColor.Black)
                {
                    Board[i][j] = new BoardSquare(ETeamColor.Black);
                    if (i < howManyRowsOfButtons)
                    {
                        Board[i][j].Button = new Button(ETeamColor.Black);
                    }
                    else if (i >= Row  - howManyRowsOfButtons)
                    {
                        Board[i][j].Button = new Button(ETeamColor.White);
                    }
                }
                else
                {
                    Board[i][j] = new BoardSquare(ETeamColor.White);
                }


            }
        }
    }

    private ETeamColor ToggleColor(ETeamColor color)
    {
        return color == ETeamColor.White ? ETeamColor.Black : ETeamColor.White;
    }

    public Button? GetButton(Tuple<int, int> coordinates, BoardSquare[][]? board = null)
    {
        try
        {
            board ??= Board;
            return board[coordinates.Item1][coordinates.Item2].Button;
        }
        catch (Exception)
        {
            return null;
        }
    }


    public void RemoveButton(Tuple<int, int> coordinates, BoardSquare[][]? board = null)
    {
        board ??= Board;
        board[coordinates.Item1][coordinates.Item2].Button = null;
    }

    public void PlaceButton(Tuple<int, int> coordinates, Button? button, BoardSquare[][]? board = null)
    {
        board ??= Board;
        board[coordinates.Item1][coordinates.Item2].Button = button;
    }
    
}