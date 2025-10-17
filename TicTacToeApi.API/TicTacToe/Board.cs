namespace TicTacToeApi.API.TicTacToe;

// class for dealing with boards positions
public class BoardPosition
{
    private int _boardArrayIndex;

    public BoardPosition(int index)
    {
        if (index >= 0 && index < 9)
            _boardArrayIndex = index;
    }

    public int ToArrayIndex() => _boardArrayIndex;

    public BoardPosition Up() => new BoardPosition(_boardArrayIndex - 3 > 0 ? _boardArrayIndex - 3 : _boardArrayIndex + 6);
    public BoardPosition Down() => new BoardPosition(_boardArrayIndex + 3 < 9 ? _boardArrayIndex + 3 : _boardArrayIndex - 6);
    public BoardPosition Left() => new BoardPosition(_boardArrayIndex % 3 == 0 ? _boardArrayIndex + 2 : _boardArrayIndex - 1);
    public BoardPosition Right() => new BoardPosition((_boardArrayIndex - 2) % 3 == 0 ? _boardArrayIndex - 2 : _boardArrayIndex + 1);

    // returns index of down left or down right square
    // if _boardArrayIndex == 4, returns down right
    public BoardPosition NextOnDiagonal() => Game.diagonalA.Contains(_boardArrayIndex) ? Down().Right() : Down().Left();

    public bool IsAlignedToDiagonal() => Game.diagonalA.Contains(_boardArrayIndex) || Game.diagonalB.Contains(_boardArrayIndex);
    public bool IsCorner() => _boardArrayIndex == 0 || _boardArrayIndex == 2 || _boardArrayIndex == 6 || _boardArrayIndex == 8;
}

public class TicTacToeBoard
{
    // verifies that the board string is 9 length and only contains x's, o's or spaces
    public static bool IsBoardValid(string board)
    {
        if (board.Length != 9)
            return false;

        for (int i = 0; i < 9; ++i)
        {
            if (board[i] != ' ' && board[i] != 'x' && board[i] != 'o')
                return false;
        }

        return true;
    }
}
