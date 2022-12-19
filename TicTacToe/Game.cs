namespace TicTacToeApi.TicTacToe;

class Game
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

    public string OptimalPlay ()
    {
        if (TryWin(out int i))
            return MakeMove(i);
        if (TryBlock(out i))
            return MakeMove(i);
        if (TryFork(out i))
            return MakeMove(i);
        if (TryBlockFork(out i))
            return MakeMove(i);
        if (TryCenter(out i))
            return MakeMove(i);
        if (TryOppositeCorner(out i))
            return MakeMove(i);
        if (TryEmptyCorner(out i))
            return MakeMove(i);
    
        TryEmptySide(out i);
        return MakeMove(i);
    }

    public bool CompleteThreeInARow(out int i, char player)
    {
        for (i = 0; i < 9; ++i)
        {
            BoardPosition position = new BoardPosition(i);
            if (BoardAt(position) != ' ') 
                continue;

            if (
                ArePositionsOf(position.Up().Up(), position.Up(), player) ||
                ArePositionsOf(position.Left().Left(), position.Left(), player)
            )
            {
                if (!(position.IsAlignedToDiagonal())) 
                    return true;
                else
                {
                    if (i != 4)
                    {
                        if (ArePositionsOf(position.NextOnDiagonal(), position.NextOnDiagonal().NextOnDiagonal(), player))
                            return true;
                    }
                    else
                    {
                        if (ArePositionsOf(position.Down().Left(), position.Down().Left().Down().Left(), player))
                            return true;
                        if (ArePositionsOf(position.Down().Right(), position.Down().Right().Down().Right(), player))
                            return true;
                    }
                }
            }
        }
        i = -1;
        return false;
    }

    // 1. Win: If the player has two in a row, they can place a third to get three in a row.
    // returns true and out index to play if win is possible, returns false and index -1 if otherwise
    public bool TryWin (out int i)
    {
        return CompleteThreeInARow(out i, _playingAs);
    }
    // 2. Block: If the opponent has two in a row, the player must play the third themselves to block the opponent.
    // returns true and out index to play if block is possible, returns false and index -1 if otherwise
    public bool TryBlock (out int i)
    {
        return CompleteThreeInARow(out i, _opponent);
    }
    // 3. Fork: Create an opportunity where the player has two ways to win (two non-blocked lines of 2).
    public bool TryFork(out int i)
    {
        for (i = 0; i < 9; ++i)
        {
            if (i != ' ') continue;

            if (CountAligned(i) >= 2)
                return true;
        }
        i = -1;
        return false;
    }
    // 4. Blocking an opponent's fork: If there is only one possible fork for the opponent, the player should block it. Otherwise, the player should block all forks in any way that simultaneously allows them to create two in a row. Otherwise, the player should create a two in a row to force the opponent into defending, as long as it doesn't result in them creating a fork. For example, if "X" has two opposite corners and "O" has the center, "O" must not play a corner move in order to win. (Playing a corner move in this scenario creates a fork for "X" to win.)
    public bool TryBlockFork(out int i)
    {
        for (i = 0; i < 9; ++i)
        {
            if (i != ' ') continue;

            if (CountAligned(i, _opponent) >= 2)
                return true;
        }
        i = -1;
        return false;
    }
    // 5. Center: A player marks the center. (If it is the first move of the game, playing a corner move gives the second player more opportunities to make a mistake and may therefore be the better choice; however, it makes no difference between perfect players.)
    public bool TryCenter(out int i)
    {
        i = 4;
        if (_board[4] == ' ')
            return true;
        i = -1;
        return false;
    }
    // 6. Opposite corner: If the opponent is in the corner, the player plays the opposite corner.
    public bool TryOppositeCorner(out int i)
    {
        if (_board[0] == _opponent && _board[8] == ' ')
        {
            i = 8;
            return true;
        }
        else if (_board[8] == _opponent && _board[0] == ' ')
        {
            i = 0;
            return true;
        }
        else if (_board[2] == _opponent && _board[6] == ' ')
        {
            i = 6;
            return true;
        }
        else if (_board[6] == _opponent && _board[2] == ' ')
        {
            i = 2;
            return true;
        }

        i = -1;
        return false;
    }
    // 7. Empty corner: The player plays in a corner square.
    public bool TryEmptyCorner(out int i)
    {
        for (i = 0; i < 9; ++i)
        {
            BoardPosition position = new BoardPosition(i);
            if (BoardAt(position) != ' ') continue;
            if (position.IsCorner())
                return true;
        }
        i = -1;
        return false;
    }
    // 8. Empty side: The player plays in a middle square on any of the 4 sides 
    public bool TryEmptySide(out int i)
    {
        for (i = 0; i < 9; ++i)
        {
            BoardPosition position = new BoardPosition(i);
            if (BoardAt(position) != ' ') continue;

            return true;
        }
        i = -1;
        return false;
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
        if (position.IsAlignedToDiagonal()) 
        {
            if (i != 4)
            {
                if (ArePositionsOf(position.NextOnDiagonal(), position.NextOnDiagonal().NextOnDiagonal(), player))
                    ++count;
            }
            else
            {
                if (ArePositionsOf(position.Down().Left(), position.Down().Left().Down().Left(), player))
                    ++count;
                if (ArePositionsOf(position.Down().Right(), position.Down().Right().Down().Right(), player))
                    ++count;
            }
        }
            ++count;
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

    public bool ArePositionsOf (BoardPosition position1, BoardPosition position2, char player)
    {
        return player == _playingAs ?
            BoardAt(position1) == BoardAt(position2) && BoardAt(position1) == _playingAs
            :
            BoardAt(position1) == BoardAt(position2) && BoardAt(position1) == _opponent;
    }

    public char BoardAt (BoardPosition position) => _board[position.ToArrayIndex()];

    public string MakeMove (int index)
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
    public bool PlayersTurn ()
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

    public bool IsGameOver ()
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