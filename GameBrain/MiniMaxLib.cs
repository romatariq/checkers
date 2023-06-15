namespace GameBrain;

public class MiniMaxLib
{
    public static (Tuple<int, int> from, Tuple<int, int> to) MiniMaxDecision<TBoard, TPlayer>(
        int desiredDepth,
        TBoard board,
        TPlayer player,
        Tuple<int, int>? startFrom,
        Func<TPlayer, TPlayer> switchPlayer,
        Func<TBoard, TPlayer, Tuple<int, int>?, List<Tuple<int, int>>> possibleFromMoves,
        Func<Tuple<int, int>, TBoard, TPlayer, Tuple<int, int>?, List<Tuple<int, int>>> possibleToMoves,
        Func<Tuple<int, int>, Tuple<int, int>, TBoard, TPlayer, bool> moveButtonOnBoard,
        Func<TBoard, TPlayer, int, int> getBoardValue,
        Func<TBoard, TBoard> cloneBoard,
        Func<TBoard, TPlayer, Tuple<int, int>?, bool> isGameOver)
    {
        Tuple<int, int> bestFromCoordinate = null!;
        Tuple<int, int> bestToCoordinate = null!;
        var bestBoardAfterMoveValue = int.MinValue;

        foreach (var fromCoordinate in possibleFromMoves(board, player, startFrom))
        {
            foreach (var toCoordinates in possibleToMoves(fromCoordinate, board, player, startFrom))
            {
                var boardClone = cloneBoard(board);
                var isTurnOver = moveButtonOnBoard(fromCoordinate, toCoordinates, boardClone, player);

                var boardAfterMoveValue = isTurnOver
                    ? MiniMaxMin(
                        desiredDepth,
                        player,
                        boardClone,
                        switchPlayer(player),
                        0,
                        null,
                        switchPlayer,
                        possibleFromMoves,
                        possibleToMoves,
                        moveButtonOnBoard,
                        getBoardValue,
                        cloneBoard,
                        isGameOver)
                    : MiniMaxMax(
                        desiredDepth,
                        player,
                        boardClone,
                        player,
                        0,
                        toCoordinates,
                        switchPlayer,
                        possibleFromMoves,
                        possibleToMoves,
                        moveButtonOnBoard,
                        getBoardValue,
                        cloneBoard,
                        isGameOver);

                if (boardAfterMoveValue > bestBoardAfterMoveValue)
                {
                    bestBoardAfterMoveValue = boardAfterMoveValue;
                    bestFromCoordinate = fromCoordinate;
                    bestToCoordinate = toCoordinates;
                }
            }
        }

        return (bestFromCoordinate, bestToCoordinate);
    }

    private static int MiniMaxMin<TBoard, TPlayer>(
        int desiredDepth,
        TPlayer initialPlayer,
        TBoard board,
        TPlayer player,
        int depth,
        Tuple<int, int>? startFrom,
        Func<TPlayer, TPlayer> switchPlayer,
        Func<TBoard, TPlayer, Tuple<int, int>?, List<Tuple<int, int>>> possibleFromMoves,
        Func<Tuple<int, int>, TBoard, TPlayer, Tuple<int, int>?, List<Tuple<int, int>>> possibleToMoves,
        Func<Tuple<int, int>, Tuple<int, int>, TBoard, TPlayer, bool> moveButtonOnBoard,
        Func<TBoard, TPlayer, int, int> getBoardValue,
        Func<TBoard, TBoard> cloneBoard,
        Func<TBoard, TPlayer, Tuple<int, int>?, bool> isGameOver)
    {
        if (depth == desiredDepth || isGameOver(board, player, startFrom)) return getBoardValue(board, initialPlayer, depth);
        var bestBoardAfterMoveValue = int.MaxValue;

        foreach (var fromCoordinate in possibleFromMoves(board, player, startFrom))
        {
            foreach (var toCoordinates in possibleToMoves(fromCoordinate, board, player, startFrom))
            {
                var boardClone = cloneBoard(board);
                var isTurnOver = moveButtonOnBoard(fromCoordinate, toCoordinates, boardClone, player);
                var boardAfterMoveValue = isTurnOver
                    ? MiniMaxMax(
                        desiredDepth,
                        initialPlayer,
                        boardClone,
                        switchPlayer(player),
                        depth + 1,
                        null,
                        switchPlayer,
                        possibleFromMoves,
                        possibleToMoves,
                        moveButtonOnBoard,
                        getBoardValue,
                        cloneBoard,
                        isGameOver
                    )
                    : MiniMaxMin(
                        desiredDepth,
                        initialPlayer,
                        boardClone,
                        player,
                        depth,
                        toCoordinates,
                        switchPlayer,
                        possibleFromMoves,
                        possibleToMoves,
                        moveButtonOnBoard,
                        getBoardValue,
                        cloneBoard,
                        isGameOver
                    );
                if (boardAfterMoveValue < bestBoardAfterMoveValue)
                {
                    bestBoardAfterMoveValue = boardAfterMoveValue;
                }
            }
        }
        return bestBoardAfterMoveValue;
    }

    private static int MiniMaxMax<TBoard, TPlayer>(
        int desiredDepth,
        TPlayer initialPlayer,
        TBoard board,
        TPlayer player,
        int depth,
        Tuple<int, int>? startFrom,
        Func<TPlayer, TPlayer> switchPlayer,
        Func<TBoard, TPlayer, Tuple<int, int>?, List<Tuple<int, int>>> possibleFromMoves,
        Func<Tuple<int, int>, TBoard, TPlayer, Tuple<int, int>?, List<Tuple<int, int>>> possibleToMoves,
        Func<Tuple<int, int>, Tuple<int, int>, TBoard, TPlayer, bool> moveButtonOnBoard,
        Func<TBoard, TPlayer, int, int> getBoardValue,
        Func<TBoard, TBoard> cloneBoard,
        Func<TBoard, TPlayer, Tuple<int, int>?, bool> isGameOver)
    {
        if (depth == desiredDepth || isGameOver(board, player, startFrom)) return getBoardValue(board, initialPlayer, depth);
        var bestBoardAfterMoveValue = int.MinValue;

        foreach (var fromCoordinate in possibleFromMoves(board, player, startFrom))
        {
            foreach (var toCoordinates in possibleToMoves(fromCoordinate, board, player, startFrom))
            {
                var boardClone = cloneBoard(board);
                var isTurnOver = moveButtonOnBoard(fromCoordinate, toCoordinates, boardClone, player);

                var boardAfterMoveValue = isTurnOver
                    ? MiniMaxMin(
                        desiredDepth,
                        initialPlayer,
                        boardClone,
                        switchPlayer(player),
                        depth + 1,
                        null,
                        switchPlayer,
                        possibleFromMoves,
                        possibleToMoves,
                        moveButtonOnBoard,
                        getBoardValue,
                        cloneBoard,
                        isGameOver
                    )
                    : MiniMaxMax(
                        desiredDepth,
                        initialPlayer,
                        boardClone,
                        player,
                        depth,
                        toCoordinates,
                        switchPlayer,
                        possibleFromMoves,
                        possibleToMoves,
                        moveButtonOnBoard,
                        getBoardValue,
                        cloneBoard,
                        isGameOver
                    );

                if (boardAfterMoveValue > bestBoardAfterMoveValue)
                {
                    bestBoardAfterMoveValue = boardAfterMoveValue;
                }
            }
        }
        return bestBoardAfterMoveValue;

    }
}