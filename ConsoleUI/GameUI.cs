using Domain;
using GameBrain;

namespace ConsoleUI;

public class GameUI
{
    private GameRunner GameRunner { get; set; }
    
    public GameUI(GameRunner game)
    {
        GameRunner = game;
    }

    public void Run()
    {
        if (GameRunner.TurnCount == 0)
        {
            InitializeGame();
        }

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Game commands");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine();
        Console.WriteLine("Q - quit");
        Console.WriteLine("ENTER - call AI");
        Console.WriteLine("BACK - get previous state");
        Console.WriteLine("NEXT - get next state");
        Console.WriteLine("START - get the first state");
        Console.WriteLine("END - get the last state");
        Console.ReadLine();
        while (true)
        {
            Console.Clear();
            PrintBoard();
            Console.WriteLine();
            
            if (GameRunner.GameIsOver)
            {
                var winner = GameRunner.GetGameWinner();
                var winnerColor = winner.Color == ETeamColor.White ? "White" : "Black";
                Console.WriteLine($"Game over! {winnerColor} won!");
                Console.ReadLine();
                break;
            }

            Console.WriteLine($"{(GameRunner.CurrentPlayer.Color == ETeamColor.White ? GameRunner.Game.Player1Name : GameRunner.Game.Player2Name) }'s turn - {(GameRunner.CurrentPlayer.Color == ETeamColor.White ? "White" : "Black (yellow)")}");
            if (GameRunner.CurrentPlayer.Type != EPlayerType.Human)
            {
                Console.WriteLine("Press enter to call AI!");
            }
            
            if (GameRunner.CurrentPlayer.Type != EPlayerType.Human)
            {
                var userInput = (Console.ReadLine() ?? "").ToLower();
                
                
                var error = GetError(userInput);

                if (error == null)
                {
                    GameRunner.StartAi();
                }
                else if (error.Message == "quit")
                {
                    return;
                }
                continue;
            }

            Tuple<int, int> fromCoordinates;
            try
            {
                fromCoordinates = GetUserMove(GameRunner.GetAllPossibleFromCoordinates());
            }
            catch (Exception e)
            {
                switch (e.Message)
                {
                    case "quit":
                        return;
                    default:
                        continue;
                }
            }


            Tuple<int, int> toCoordinates;
            try
            {
                toCoordinates = GetUserMove(GameRunner.GetAllPossibleToCoordinates(fromCoordinates));
            }
            catch (Exception e)
            {
                switch (e.Message)
                {
                    case "quit":
                        return;
                    default:
                        continue;
                }
            }

            GameRunner.MoveButton(fromCoordinates, toCoordinates);
        }
    }


    private Tuple<int, int> GetUserMove(List<Tuple<int, int>> givenCoordinates)
    {
        Console.WriteLine("Enter coordinates of the button you want to move! Valid coordinates are: ");
        PrintCoordinates(givenCoordinates);
        var userInput = (Console.ReadLine() ?? "").ToLower();
        Tuple<int, int> userCoordinates;

        while (true)
        {
            var error = GetError(userInput!);
            if (error != null)
            {
                throw new Exception(error.Message);
            }
            
            try
            {
                userCoordinates = ConvertLetterCoordinatesToArrayCoordinates(userInput);
                
                if (givenCoordinates.Contains(userCoordinates)) break;
                
                Console.WriteLine("Invalid input! Choose from given coordinates!");
                userInput = (Console.ReadLine() ?? "").ToLower();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                userInput = (Console.ReadLine() ?? "").ToLower();
            }
        }

        return userCoordinates;
    }

    private Exception? GetError(string userInput)
    {
        switch (userInput.ToLower())
        {
            case "q":
                return new Exception("quit");
            case "quit":
                return new Exception("quit");
            case "back":
                var previousStateId = GameRunner.GetPreviousGameStateId(null, GameRunner.LastState.Id);
                GameRunner = new GameRunner(GameRunner.Game, GameRunner.GameRepository, previousStateId);
                return new Exception("back");
            case "next":
                var nextStateId = GameRunner.GetNextGameStateId(null, GameRunner.LastState.Id);
                GameRunner = new GameRunner(GameRunner.Game, GameRunner.GameRepository, nextStateId);
                return new Exception("next");
            case "start":
                GameRunner = new GameRunner(GameRunner.Game, GameRunner.GameRepository, GameRunner.Game.GetTheFirstState().Id);
                return new Exception("start");
            case "end":
                GameRunner = new GameRunner(GameRunner.Game, GameRunner.GameRepository, GameRunner.Game.GetTheLastState().Id);
                return new Exception("end");
            default:
                return null;
        }
    }

    private void InitializeGame()
    {
        Console.WriteLine("Game name: ");
        var gameName = Console.ReadLine() ?? "New game";
        
        Console.WriteLine("Player 1 name: ");
        var player1Name = Console.ReadLine() ?? "Player1";
        
        Console.WriteLine("Player 1 type [human or ai]: ");
        var player1TypeString = Console.ReadLine()?.ToLower() ?? "ai";
        var player1Type = player1TypeString == "ai" ? GetAiDifficultyType() : EPlayerType.Human;
        
        Console.WriteLine("Player 2 name: ");
        var player2Name = Console.ReadLine() ?? "Player2";
        
        Console.WriteLine("Player 2 type [human or ai]: ");
        var player2TypeString = Console.ReadLine()?.ToLower() ?? "ai";
        var player2Type = player2TypeString == "ai" ? GetAiDifficultyType() : EPlayerType.Human;
        
        GameRunner.Game.Name = gameName;
        GameRunner.Game.Player1Name = player1Name;
        GameRunner.Game.Player1Type = player1Type;
        GameRunner.Game.Player2Name = player2Name;
        GameRunner.Game.Player2Type = player2Type;
        GameRunner.GameRepository.SaveGame(GameRunner.Game);
        GameRunner.Player1.Type = player1Type;
        GameRunner.Player2.Type = player2Type;
    }

    private EPlayerType GetAiDifficultyType()
    {
        Console.WriteLine("Choose AI difficulty: ");
        Console.WriteLine("Very easy - 4");
        Console.WriteLine("Easy - 3");
        Console.WriteLine("Medium - 2");
        Console.WriteLine("Hard - 1");
        var userInput = Console.ReadLine() ?? "";

        return userInput.ToLower() switch
        {
            "4" => EPlayerType.VeryEasyAI,
            "3" => EPlayerType.EasyAI,
            "2" => EPlayerType.MediumAI,
            "very easy" => EPlayerType.VeryEasyAI,
            "easy" => EPlayerType.EasyAI,
            "medium" => EPlayerType.MediumAI,
            _ => EPlayerType.HardAI
        };
    }

    private void PrintCoordinates(List<Tuple<int, int>> coordinates)
    {
        for (int i = 0; i < coordinates.Count; i++)
        {
            Console.Write(ConvertCoordinateToString(coordinates[i]));
            if (i != coordinates.Count - 1)
            {
                Console.Write(", ");
            }
            
        }

        Console.WriteLine();
    } 

    private string ConvertCoordinateToString(Tuple<int, int> coordinate)
    {
        return CheckersBoard.Alphabet[coordinate.Item2] + (GameRunner.Board.Row - coordinate.Item1).ToString();
    }
    
    private Tuple<int, int> ConvertLetterCoordinatesToArrayCoordinates(string? pos)
    {
        pos = pos?.ToUpper();
        if (pos == null || pos.Length < 2 || pos.Length > 3)
        {
            throw new Exception("Invalid input");
        }
        
        var letter = pos[0];
        var number = pos[1..];

        if (!CheckersBoard.Alphabet.Contains(letter) || !int.TryParse(number, out _))
        {
            throw new Exception("Invalid letter or number");
        }

        var columnIndex = CheckersBoard.Alphabet.IndexOf(letter);
        var rowIndex = GameRunner.Board.Row - int.Parse(number);

        if (rowIndex >= GameRunner.Board.Row || rowIndex < 0 || columnIndex < 0 || columnIndex >= GameRunner.Board.Column)
        {
            throw new Exception("Coordinate out of board area");
        }

        return new Tuple<int, int>(rowIndex, columnIndex);
    }
    
    private void PrintBoard()
    {
        
        Console.Write("   ");
        for (int i = 0; i < GameRunner.Board.Column; i++)
        {
            Console.Write($" {CheckersBoard.Alphabet[i]} ");
        }
        Console.WriteLine();

        for (int i = 0; i < GameRunner.Board.Row; i++)
        {

            Console.Write($"{GameRunner.Board.Row - i} {(GameRunner.Board.Row - i > 9 ? "" : " ")}");
            
            for (int j = 0; j < GameRunner.Board.Column; j++)
            {

                var square = GameRunner.Board.Board[i][j];
                if (square.Color == ETeamColor.White)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                }
                if (square.Color == ETeamColor.Black)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                }

                var button = square.Button;
                Console.ForegroundColor = ConsoleColor.White;
                if (button?.Color == ETeamColor.Black)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }

                Console.Write(button != null ? $" {button} " : "   ");
            }
            Console.ResetColor();
            Console.WriteLine();
        }
        Console.ResetColor();
    }

}