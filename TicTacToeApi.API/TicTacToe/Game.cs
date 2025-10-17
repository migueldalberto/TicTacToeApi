namespace TicTacToeApi.API.TicTacToe;

public class Game
{
    private string _board = "         ";
    private char _playingAs;
    private char _opponent;

    public static readonly int[] diagonalA = { 0, 4, 8 };
    public static readonly int[] diagonalB = { 2, 4, 6 };

    public string Board
    {
        get => _board;
        set
        {
            if (TicTacToeBoard.IsBoardValid(value))
                _board = value;
        }
    }

    public Game(string board, char playingAs)
    {
        Board = board;
        _playingAs = playingAs;
        _opponent = _playingAs == 'o' ? 'x' : 'o';
    }

    public string OptimalPlay()
    {
        int i = -1;
        if ((i = TryWin()) != -1)
            return MakeMove(i);
        if ((i = TryBlock()) != -1)
            return MakeMove(i);
        if ((i = TryFork()) != -1)
            return MakeMove(i);
        if ((i = TryBlockFork()) != -1)
            return MakeMove(i);
        if ((i = TryCenter()) != -1)
            return MakeMove(i);
        if ((i = TryOppositeCorner()) != -1)
            return MakeMove(i);
        if ((i = TryEmptyCorner()) != -1)
            return MakeMove(i);

        i = TryEmptySide();
        return MakeMove(i);
    }

    public int CompleteThreeInARow(char player)
    {
        for (int i = 0; i < 9; ++i)
        {
            BoardPosition position = new BoardPosition(i);
            if (BoardAt(position) != ' ')
                continue;

            if (
                ArePositionsOf(position.Up().Up(), position.Up(), player) ||
                ArePositionsOf(position.Left().Left(), position.Left(), player)
            )
                return i;

            if (Game.diagonalA.Contains(position.ToArrayIndex()))
            {
                if (ArePositionsOf(position.Down().Right(), position.Down().Right().Down().Right(), player))
                    return i;
            }
            else if (Game.diagonalB.Contains(position.ToArrayIndex()))
            {
                if (ArePositionsOf(position.Down().Left(), position.Down().Left().Down().Left(), player))
                    return i;
            }
        }
        return -1;
    }

    // 1. Win: If the player has two in a row, they can place a third to get three in a row.
    // returns true and out index to play if win is possible, returns false and index -1 if otherwise
    public int TryWin()
    {
        return CompleteThreeInARow(_playingAs);
    }
    // 2. Block: If the opponent has two in a row, the player must play the third themselves to block the opponent.
    // returns true and out index to play if block is possible, returns false and index -1 if otherwise
    public int TryBlock()
    {
        return CompleteThreeInARow(_opponent);
    }
    // 3. Fork: Create an opportunity where the player has two ways to win (two non-blocked lines of 2).
    public int TryFork()
    {
        if (BoardAt(0) == _playingAs && BoardAt(8) == _playingAs)
        {
            if (BoardAt(6) == ' ')
                return 6;
            else if (BoardAt(2) == ' ')
                return 2;
        }

        if (BoardAt(2) == _playingAs && BoardAt(6) == _playingAs)
        {
            if (BoardAt(0) == ' ')
                return 0;
            else if (BoardAt(8) == ' ')
                return 8;

        }

        return -1;
    }

    // 4. Blocking an opponent's fork: If there is only one possible fork for the opponent, the player should block it. Otherwise, the player should block all forks in any way that simultaneously allows them to create two in a row. Otherwise, the player should create a two in a row to force the opponent into defending, as long as it doesn't result in them creating a fork. For example, if "X" has two opposite corners and "O" has the center, "O" must not play a corner move in order to win. (Playing a corner move in this scenario creates a fork for "X" to win.)
    public int TryBlockFork()
    {
        if (BoardAt(0) == _opponent && BoardAt(8) == _opponent)
        {
            if (BoardAt(6) == ' ')
                return 6;
            else if (BoardAt(2) == ' ')
                return 2;
        }

        if (BoardAt(2) == _opponent && BoardAt(6) == _opponent)
        {
            if (BoardAt(0) == ' ')
                return 0;
            else if (BoardAt(8) == ' ')
                return 8;

        }

        return -1;
    }
    // 5. Center: A player marks the center. (If it is the first move of the game, playing a corner move gives the second player more opportunities to make a mistake and may therefore be the better choice; however, it makes no difference between perfect players.)
    public int TryCenter()
    {
        int i = 4;
        if (_board[i] == ' ')
            return i;
        return -1;
    }
    // 6. Opposite corner: If the opponent is in the corner, the player plays the opposite corner.
    public int TryOppositeCorner()
    {
        if (_board[0] == _opponent && _board[8] == ' ')
        {
            return 8;
        }
        else if (_board[8] == _opponent && _board[0] == ' ')
        {
            return 0;
        }
        else if (_board[2] == _opponent && _board[6] == ' ')
        {
            return 6;
        }
        else if (_board[6] == _opponent && _board[2] == ' ')
        {
            return 2;
        }

        return -1;
    }
    // 7. Empty corner: The player plays in a corner square.
    public int TryEmptyCorner()
    {
        for (int i = 0; i < 9; ++i)
        {
            BoardPosition position = new BoardPosition(i);
            if (BoardAt(position) != ' ') continue;
            if (position.IsCorner())
                return i;
        }
        return -1;
    }
    // 8. Empty side: The player plays in a middle square on any of the 4 sides 
    public int TryEmptySide()
    {
        for (int i = 1; i < 9; i += 2)
        {
            if (BoardAt(i) == ' ')
                return i;
        }
        return -1;
    }

    public int CountAligned(BoardPosition position, char player)
    {
        int i = position.ToArrayIndex();
        if (BoardAt(position) != ' ' || (player != 'x' && player != 'o'))
            return -1;

        int count = 0;
        if (BoardAt(position.Up()) == player)
            ++count;
        if (BoardAt(position.Down()) == player)
            ++count;
        if (BoardAt(position.Left()) == player)
            ++count;
        if (BoardAt(position.Right()) == player)
            ++count;

        if (Game.diagonalA.Contains(i))
        {
            if (BoardAt(position.Down().Right()) == player)
                ++count;
            if (BoardAt(position.Down().Right().Down().Right()) == player)
                ++count;
        }

        if (Game.diagonalB.Contains(i))
        {
            if (BoardAt(position.Down().Left()) == player)
                ++count;
            if (BoardAt(position.Down().Left().Down().Left()) == player)
                ++count;
        }
        return count;
    }
    public int CountAligned(int i)
    {
        BoardPosition position = new BoardPosition(i);
        return CountAligned(position, _playingAs);
    }

    public int CountAligned(int i, char player)
    {
        BoardPosition position = new BoardPosition(i);
        return CountAligned(position, player);
    }

    public bool ArePositionsOf(BoardPosition position1, BoardPosition position2, char player)
    {
        return player == _playingAs ?
            BoardAt(position1) == BoardAt(position2) && BoardAt(position1) == _playingAs
            :
            BoardAt(position1) == BoardAt(position2) && BoardAt(position1) == _opponent;
    }

    public char BoardAt(BoardPosition position) => _board[position.ToArrayIndex()];
    public char BoardAt(int position) => _board[position];

    public string MakeMove(int index)
    {
        if (Board[index] != ' ')
            return Board;
        char[] newBoard = _board.ToArray();
        newBoard[index] = _playingAs;
        Board = new string(newBoard);
        return Board;
    }

    // verifies that board is with equal number of player marks and opponent marks
    // this garantees the possibility of being player's turn
    // it also verifies that the difference between opponent marks and player marks is not greater than 2, which would invalidate the board
    public bool PlayersTurn()
    {
        int OCount = 0;
        int XCount = 0;
        for (int i = 0; i < 9; ++i)
        {
            if (_board[i] == 'o')
                ++OCount;
            else if (_board[i] == 'x')
                ++XCount;
        }
        return _playingAs == 'o' ?
            OCount <= XCount && XCount - OCount < 2
            :
            XCount <= OCount && OCount - XCount < 2;
    }

    public bool IsGameOver()
    {
        for (int i = 0; i < 9; i += 2)
        {
            char current = _board[i];
            if (current == ' ') continue;
            BoardPosition position = new BoardPosition(i);

            if (
                (current == BoardAt(position.Up()) && current == BoardAt(position.Up().Up())) ||
                (current == BoardAt(position.Left()) && current == BoardAt(position.Left().Left())) ||
                (position.IsAlignedToDiagonal() &&
                current == BoardAt(position.NextOnDiagonal()) &&
                current == BoardAt(position.NextOnDiagonal().NextOnDiagonal()))
            )
                return true;
            else continue;
        }
        return false;
    }

}
