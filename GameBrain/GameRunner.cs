using System.Text.Json;
using DAL;
using Domain;

namespace GameBrain;

public class GameRunner
{
    public Game Game { get; set; }

    public GameState LastState { get; set; }
    
    public CheckersBoard Board { get; set; }
    public Player Player1 { get; set; }
    public Player Player2 { get; set; }
    public Player CurrentPlayer { get; set; }
    private GameSetting Setting { get; set; }

    public bool GameIsOver { get; set; }
    public DateTime? GameEndedAt { get; set; }

    public int TurnCount { get; set; } = 1;
    private Tuple<int, int>? StartFrom { get; set; }
    public IGameRepository GameRepository { get; set; }
    
    public GameRunner(Game game, IGameRepository gameRepository, int? stateId = null)
    {
        Game = game;
        GameRepository = gameRepository;
        
        if (game.GameStates == null || game.GameStates.Count == 0)
        {
            LastState = new GameState()
            {
                CurrentPlayer = game.GameSetting!.WhiteStarts ? ETeamColor.White : ETeamColor.Black,
                SerializedBoard = GameState.SerializeBoard(new CheckersBoard(game.GameSetting.BoardWidth, game.GameSetting.BoardHeight).Board)
            };
            Game.GameStates = new List<GameState>(){LastState};
            TurnCount = 0;
            GameRepository.SaveGame(Game);
        }
        else
        {
            LastState = stateId == null ? 
                game.GameStates.OrderByDescending(x => x.CreatedAt).First() 
                : game.GameStates.First(x => x.Id == stateId);
        }

        StartFrom = LastState.StartFromX != null && LastState.StartFromY != null 
            ? Tuple(LastState.StartFromX.Value, LastState.StartFromY.Value) 
            : null;
        Board = new CheckersBoard(GameState.DeserializeBoard(LastState.SerializedBoard));
        Player1 = new Player(ETeamColor.White, Game.Player1Type, Game.Player1Name);
        Player2 = new Player(ETeamColor.Black, Game.Player2Type, Game.Player2Name);
        CurrentPlayer = LastState.CurrentPlayer.Equals(Player1.Color) ? Player1 : Player2;
        Setting = game.GameSetting!;
        GameIsOver = LastState.EndedAt != null;
    }

    
    public bool MoveButton(Tuple<int, int> from, Tuple<int, int> to)
    {
        if (GameIsOver)
        {
            return false;
        }
        if (CheckIfValidMove(from, to))
        {
            var turnOver = MoveButtonOnBoard(from, to);

            if (turnOver)
            {
                StartFrom = null;
                ChangeCurrentPlayer();
            } else
            {
                StartFrom = to;
            }

            if (IsGameOver())
            {
                GameIsOver = true;
                GameEndedAt = DateTime.Now;
            }
            AddState();
            return true;
        }

        return false;
    }

    //also returns if turn is over
    private bool MoveButtonOnBoard(Tuple<int, int> from, Tuple<int, int> to, BoardSquare[][]? board = null, Player? currentPlayer = null)
    {
        var captureMoves = GetAllCaptureMoves(from, board, currentPlayer);
        var button = Board.GetButton(from, board);
        Board.RemoveButton(from, board);
        if (to.Item1 == 0 && CurrentPlayer == Player1)
        {
            button!.Type = EButtonType.King;
        }
        if (to.Item1 == (Setting.BoardHeight - 1) && CurrentPlayer == Player2)
        {
            button!.Type = EButtonType.King;
        }
        Board.PlaceButton(to, button, board);
        
        if (captureMoves.Contains(to))
        {
            Board.RemoveButton(CapturedButtonCoordinates(from, to), board);
            if (GetAllCaptureMoves(to, board, currentPlayer).Count > 0)
            {
                return false;
            }
        }

        return true;
    }
    

    private void AddState()
    {
        var newState = new GameState()
        {
            CurrentPlayer = CurrentPlayer.Color,
            GameId = Game.Id,
            SerializedBoard = GameState.SerializeBoard(Board.Board),
            StartFromX = StartFrom?.Item1,
            StartFromY = StartFrom?.Item2,
            EndedAt = GameEndedAt
        };
        DeleteAllStatesMadeAfterCurrentState();
        LastState = newState;
        Game.GameStates!.Add(newState);
        GameRepository.SaveGame(Game);
        
    }

    public bool StartAi()
    {
        if (CurrentPlayer.Type == EPlayerType.Human) return false;

        var bestMove = CurrentPlayer.Type == EPlayerType.VeryEasyAI ? 
            GetRandomAIMove() 
            : GetMiniMaxAIMove();

        return MoveButton(bestMove.from, bestMove.to);
    }

    private (Tuple<int, int> from, Tuple<int, int> to) GetRandomAIMove()
    {
        var random = new Random();
        
        var fromMoves = GetAllPossibleFromCoordinates();
        var randomFromIndex = random.Next(0, fromMoves.Count);
        var randomFromMove = fromMoves[randomFromIndex];
        
        var toMoves = GetAllPossibleToCoordinates(randomFromMove);
        var randomToIndex = random.Next(0, toMoves.Count);
        var randomToMove = toMoves[randomToIndex];

        return (randomFromMove, randomToMove);
    }
    
    private (Tuple<int, int> from, Tuple<int, int> to) GetMiniMaxAIMove()
    {
        var depth = 4 - (int) CurrentPlayer.Type;
        var cellCount = Board.Row * Board.Column;
        if (cellCount > 200 && depth > 2) depth = 2;
        if (cellCount > 600) depth = 1;

        var bestMove = MiniMaxLib.MiniMaxDecision(
            depth,
            Board.Board,
            CurrentPlayer,
            StartFrom,
            SwitchPlayer,
            GetAllPossibleFromCoordinates,
            GetAllPossibleToCoordinates,
            MoveButtonOnBoard,
            GetBoardValue,
            GetBoardClone,
            IsGameOver);

        return (bestMove.from, bestMove.to);
    }
    
    private Player SwitchPlayer(Player player)
    {
        return player == Player1 ? Player2 : Player1;
    }

    private BoardSquare[][] GetBoardClone(BoardSquare[][] board)
    {
        var serializedBoard = GameState.SerializeBoard(board);
        return GameState.DeserializeBoard(serializedBoard);
    }

    private int GetBoardValue(BoardSquare[][] board, Player player, int depth)
    {
        var playerPoints = 0;
        var opponentPoints = 0;

        for (int x = 0; x < board.Length; x++)
        {
            for (int y = 0; y < board[0].Length; y++)
            {
                var currentCoordinates = Tuple(x, y);
                var button = Board.GetButton(currentCoordinates, board);
                if (button?.Color == player.Color)
                {
                    playerPoints += button.Type == EButtonType.Normal ? 1 : 2;
                }
                if (button != null && button.Color != player.Color)
                {
                    opponentPoints += button.Type == EButtonType.Normal ? 1 : 2;
                }
            }
        }

        var points = playerPoints - opponentPoints;
        if (opponentPoints == 0)
        {
            points += 10;
            points -= depth;
        }

        return points;
    }


    //deletes all states where createdAt date is after current state so there couldn't be multiple threads
    //for example original move B3->A4 *goes back to last state and makes move B3->C4 then should delete the state cause by B3->A4 
    private void DeleteAllStatesMadeAfterCurrentState()
    {
        var statesToBeRemoved = Game.GameStates!.Where(x => x.CreatedAt > LastState.CreatedAt);
        foreach (var state in statesToBeRemoved.ToList())
        {
            Game.GameStates!.Remove(state);
        }

    }
    
    
    public List<Tuple<int, int>> GetAllPossibleFromCoordinates(BoardSquare[][]? board = null, Player? currentPlayer = null, Tuple<int, int>? startFrom = null)
    {
        var captureFromCoordinates = new List<Tuple<int, int>>();
        var allPossibleFromCoordinates = new List<Tuple<int, int>>();

        if (startFrom == null && board == null)
        {
            startFrom = StartFrom;
        }
        currentPlayer ??= CurrentPlayer;
        
        if (startFrom != null)
        {
            captureFromCoordinates.Add(startFrom);
            return captureFromCoordinates;
        }
        
        for (int x = 0; x < Board.Row; x++)
        {
            for (int y = 0; y < Board.Column; y++)
            {
                var currentCoordinates = Tuple(x, y);
                var button = Board.GetButton(currentCoordinates, board);
                if (button?.Color == currentPlayer.Color)
                {
                    if (GetAllCaptureMoves(currentCoordinates, board, currentPlayer).Count > 0)
                    {
                        captureFromCoordinates.Add(currentCoordinates);
                    }
                    if (GetAllPossibleToCoordinates(currentCoordinates, board, currentPlayer, startFrom).Count > 0)
                    {
                        allPossibleFromCoordinates.Add(currentCoordinates);
                    }
                }
            }
        }

        if (captureFromCoordinates.Count == 0 || !Setting.HasToCapture)
        {
            return allPossibleFromCoordinates;
        }
        return captureFromCoordinates;
    }

    public List<Tuple<int, int>> GetAllPossibleToCoordinates(Tuple<int, int> from, BoardSquare[][]? board = null, Player? currentPlayer = null, Tuple<int, int>? startFrom = null)
    {
        var button = Board.GetButton(from, board);
        var list = new List<Tuple<int, int>>();
        if (startFrom == null && board == null)
        {
            startFrom = StartFrom;
        }

        if (button == null || !CheckIfPlayerMovesOwnButton(button, currentPlayer))
        {
            return list;
        }

        var isButtonFlyingKing = button.Type == EButtonType.King && !Setting.KingCanMoveOnlyOneStep;


        //get all king capturing moves or normal moves
        list = isButtonFlyingKing ? GetAllKingCaptureMoves(from, list, button, board) : GetAllNormalCapturingMoves(from, list, button, board, currentPlayer);
        
        // Moving moves only when there are no capturing moves or does not have to capture by rules and it's first move
        if (list.Count == 0 || !Setting.HasToCapture && startFrom == null)
        {
            list = isButtonFlyingKing ? GetAllKingMovingMoves(from, list, board) : GetAllNormalMovingMoves(from, list, button, board, currentPlayer);
        }

        // Can move to current location if does not have to capture and it is ongoing move
        if (!Setting.HasToCapture && startFrom != null)
        {
            list.Add(from);
        }

        return list;
    }

    
    private void ChangeCurrentPlayer()
        => CurrentPlayer = CurrentPlayer.Equals(Player1) ? Player2 : Player1;


    private static Tuple<int, int> Tuple(int x, int y) 
        => new(x, y);


    private bool CheckIfValidMove(Tuple<int, int> from, Tuple<int, int> to)
        => GetAllPossibleFromCoordinates().Contains(from)
           && GetAllPossibleToCoordinates(from).Contains(to);


    private int GetPlayerAttackingDirection(Player? currentPlayer)
    {
        currentPlayer ??= CurrentPlayer;
        return currentPlayer.Color == ETeamColor.White ? -1 : 1;
    }


    private bool CheckIfPlayerMovesOwnButton(Button? button, Player? currentPlayer)
    {
        currentPlayer ??= CurrentPlayer;
        return button != null && currentPlayer.Color.Equals(button.Color);
    }

    
    private bool CheckIfNewPlaceIsEmpty(Tuple<int, int> to, BoardSquare[][]? board)
        => Board.GetButton(to, board) == null;

    
    private bool CheckIfNewPlaceIsOnBoard(Tuple<int, int> to)
        => to.Item1 >= 0 && to.Item1 < Board.Row &&
            to.Item2 >= 0 && to.Item2 < Board.Column;
    
    
    private bool CheckIfMovesOnePositionDiagonalAndForward(Tuple<int, int> from, Tuple<int, int> to, Button button, Player? currentPlayer)
    {
        var directionMultiplier = GetPlayerAttackingDirection(currentPlayer);

        var isKing = button.Type == EButtonType.King;
        var additionalKingMoves = Tuple(from.Item1 + 1 * directionMultiplier * -1, from.Item2 - 1).Equals(to)
                                    || Tuple(from.Item1 + 1 * directionMultiplier * -1, from.Item2 + 1).Equals(to);
        

        return Tuple(from.Item1 + 1 * directionMultiplier, from.Item2 - 1).Equals(to)
               || Tuple(from.Item1 + 1 * directionMultiplier, from.Item2 + 1).Equals(to) 
               || isKing && additionalKingMoves;
    }

    
    // in case cannot eat backwards
    private bool CheckIfJumpAndMovesCorrectDirection(Tuple<int, int> from, Tuple<int, int> to, Player? currentPlayer)
        => to.Item1 - from.Item1 == GetPlayerAttackingDirection(currentPlayer) * 2;
    
    
    private bool CheckIfJumpsAndMovesDiagonal(Tuple<int, int> from, Tuple<int, int> to, Button button, BoardSquare[][]? board, Player? currentPlayer)
    {
        var capturedButton = Board.GetButton(CapturedButtonCoordinates(from, to), board);
        if (button.Type == EButtonType.Normal 
            && !Setting.AllButtonCanEatKing 
            && capturedButton?.Type == EButtonType.King)
        {
            return false;
        }

        currentPlayer ??= CurrentPlayer;
        
        return Math.Abs(from.Item1 - to.Item1) == 2 
               && Math.Abs(from.Item2 - to.Item2) == 2 
               && capturedButton != null 
               && capturedButton.Color != currentPlayer.Color;
    }

    
    private Tuple<int, int> CapturedButtonCoordinates(Tuple<int, int> from, Tuple<int, int> to)
    {
        var xDirectionStep = from.Item1 > to.Item1 ? 1 : -1;
        var yDirectionStep = from.Item2 > to.Item2 ? 1 : -1;
        return Tuple(to.Item1 + xDirectionStep, to.Item2 + yDirectionStep);
    }

    private List<Tuple<int, int>> GetAllCaptureMoves(Tuple<int, int> from, BoardSquare[][]? board = null, Player? currentPlayer = null)
    {
        var list = new List<Tuple<int, int>>();
        var button = Board.GetButton(from, board);
        if (button == null)
        {
            return list;
        }
        var isButtonFlyingKing = button.Type == EButtonType.King && !Setting.KingCanMoveOnlyOneStep;

        return isButtonFlyingKing ? GetAllKingCaptureMoves(from, list, button, board) : GetAllNormalCapturingMoves(from, list, button, board, currentPlayer);
    }


    private List<Tuple<int, int>> GetAllNormalCapturingMoves(Tuple<int, int> from, List<Tuple<int, int>> list, Button button, BoardSquare[][]? board, Player? currentPlayer)
    {
        for (int x = -2; x <= 2; x+=4)
        {
            for (int y = -2; y <= 2; y+=4)
            {
                var to = Tuple(from.Item1 + x, from.Item2 + y);

                if (CheckIfNewPlaceIsOnBoard(to) 
                    && CheckIfNewPlaceIsEmpty(to, board)
                    && CheckIfJumpsAndMovesDiagonal(from, to, button, board, currentPlayer)
                    && (button.Type == EButtonType.King 
                        || button.Type == EButtonType.Normal && Setting.CanEatBackwards 
                        || CheckIfJumpAndMovesCorrectDirection(from, to, currentPlayer)))
                {
                    list.Add(to);
                }
                
            }
        }

        return list;
    }
    
    
    private List<Tuple<int, int>> GetAllNormalMovingMoves(Tuple<int, int> from, List<Tuple<int, int>> list, Button button, BoardSquare[][]? board, Player? currentPlayer)
    {
        for (int x = -1; x <= 1; x+=2)
        {
            for (int y = -1; y <= 1; y+=2)
            {
                var to = Tuple(from.Item1 + x, from.Item2 + y);

                if (CheckIfNewPlaceIsOnBoard(to) 
                    && CheckIfNewPlaceIsEmpty(to, board)
                    && CheckIfMovesOnePositionDiagonalAndForward(from, to, button, currentPlayer))
                {
                    list.Add(to);
                }
                
            }
        }

        return list;
    }

    
    private List<Tuple<int, int>> GetAllKingCaptureMoves(Tuple<int, int> from, List<Tuple<int, int>> list, Button button, BoardSquare[][]? board)
    {
        for (int xDirectionStep = -1; xDirectionStep <= 1; xDirectionStep+=2)
        {
            for (int yDirectionStep = -1; yDirectionStep <= 1; yDirectionStep+=2)
            {

                var x = xDirectionStep;
                var y = yDirectionStep;

                while (true)
                {
                    var to = Tuple(from.Item1 + x, from.Item2 + y);

                    if (!CheckIfNewPlaceIsOnBoard(to)) break;
                    
                    if (CheckIfNewPlaceIsEmpty(to, board))
                    {
                        x += xDirectionStep;
                        y += yDirectionStep;
                        continue;
                    }

                    // if own button, break
                    if (Board.GetButton(to, board)!.Color == button.Color) break;
                    
                    // if after that is also empty add to list and break
                    to = Tuple(to.Item1 + xDirectionStep, to.Item2 + yDirectionStep);
                    
                    if (CheckIfNewPlaceIsOnBoard(to) && CheckIfNewPlaceIsEmpty(to, board))
                    {
                        list.Add(to);
                    }
                    break;
                    
                }
            }
        }

        return list;
    }
    
    private List<Tuple<int, int>> GetAllKingMovingMoves(Tuple<int, int> from, List<Tuple<int, int>> list, BoardSquare[][]? board)
    {
        for (int xDirectionStep = -1; xDirectionStep <= 1; xDirectionStep+=2)
        {
            for (int yDirectionStep = -1; yDirectionStep <= 1; yDirectionStep+=2)
            {

                var x = xDirectionStep;
                var y = yDirectionStep;

                while (true)
                {
                    var to = Tuple(from.Item1 + x, from.Item2 + y);

                    if (CheckIfNewPlaceIsOnBoard(to) 
                        && CheckIfNewPlaceIsEmpty(to, board))
                    {
                        list.Add(to);
                        x += xDirectionStep;
                        y += yDirectionStep;
                    }
                    else
                    {
                        break;
                    }
                }

                
            }
        }

        return list;
    }


    private bool IsGameOver(BoardSquare[][]? board = null, Player? currentPlayer = null, Tuple<int, int>? startFrom = null)
    {
        board ??= Board.Board;

        if (GetAllPossibleFromCoordinates(board, currentPlayer, startFrom).Count == 0) return true;
        
        var whiteButtons = 0;
        var blackButtons = 0;
        
        for (int x = 0; x < Board.Row; x++)
        {
            for (int y = 0; y < Board.Column; y++)
            {
                if (Board.GetButton(Tuple(x, y), board)?.Color == ETeamColor.White)
                {
                    whiteButtons++;
                }
                if (Board.GetButton(Tuple(x, y), board)?.Color == ETeamColor.Black)
                {
                    blackButtons++;
                }

                if (whiteButtons != 0 && blackButtons != 0) return false;
            }
        }
        return true;
    }

    public int GetPreviousGameStateId(Player? player, int stateId)
    {
        return GetNextOrPreviousGameStateId(player, stateId, false);
    }
    
    public int GetNextGameStateId(Player? player, int stateId)
    {
        return GetNextOrPreviousGameStateId(player, stateId, true);
    }

    private int GetNextOrPreviousGameStateId(Player? player, int stateId, bool getNextStateId)
    {
        var state = Game.GameStates!.First(state => state.Id == stateId);

        var orderedPlayerStates = Game.GameStates!.ToList();

        orderedPlayerStates = (getNextStateId ? 
            orderedPlayerStates
                .OrderBy(s => s.CreatedAt)
                .Where(s => s.CreatedAt >= state.CreatedAt)
            : orderedPlayerStates
                .OrderByDescending(s => s.CreatedAt)
                .Where(s => s.CreatedAt <= state.CreatedAt))
            
            .Where(s => player == null || s.CurrentPlayer == player.Color)
            .ToList();
        
        
        if (player == null || state.CurrentPlayer == player.Color)
        {
            return orderedPlayerStates.Count > 1 ? orderedPlayerStates[1].Id : stateId;
        }

        return orderedPlayerStates.Count == 0 ? stateId : orderedPlayerStates[0].Id;
    }


    public Player GetGameWinner()
    {
        if (!GameIsOver) throw new Exception("Game is not over");

        return CurrentPlayer == Player1 ? Player2 : Player1;
    }
    

}