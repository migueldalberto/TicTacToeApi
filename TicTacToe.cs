namespace TicTacToeApi;

// class for dealing with boards positions
class BoardPosition
{
    private int _boardArrayIndex;
    
    public BoardPosition (int index)
    {
        if (index >= 0 && index < 9)
            _boardArrayIndex = index;
    }

    public int ToArrayIndex() => _boardArrayIndex;

    public BoardPosition Up() => new BoardPosition(_boardArrayIndex - 3 > 0 ? _boardArrayIndex -3 : _boardArrayIndex + 6);
    public BoardPosition Down() => new BoardPosition(_boardArrayIndex + 3 < 9 ? _boardArrayIndex + 3 : _boardArrayIndex - 6);
    public BoardPosition Left() => new BoardPosition(_boardArrayIndex % 3 == 0 ? _boardArrayIndex + 2 : _boardArrayIndex - 1);
    public BoardPosition Right() => new BoardPosition(_boardArrayIndex - 2 % 3 == 0 ? _boardArrayIndex - 2 : _boardArrayIndex + 1);

    public BoardPosition NextDiagonalSquare() => TicTacToeGame.diagonalA.Contains(_boardArrayIndex) ? Down().Right() : Down().Left();

    public BoardPosition LeftDownDiagonal() => Left().Down();
    public BoardPosition RightDownDiagonal() => Right().Down();

    public bool IsAlignedToDiagonal() => TicTacToeGame.diagonalA.Contains(_boardArrayIndex) || TicTacToeGame.diagonalB.Contains(_boardArrayIndex);
    public bool IsCorner() => _boardArrayIndex == 0 || _boardArrayIndex == 2 || _boardArrayIndex == 6 || _boardArrayIndex == 8;
}

class TicTacToeGame
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
            if (IsValidBoard(value)) 
                _board = value;
        }
    }

    public TicTacToeGame(string board, char playingAs)
    {
        Board = board;
        _playingAs = playingAs;
        if (_playingAs == 'o')
            _opponent = 'x';
        else
            _opponent = 'o';
    }

    public string OptimalPlay ()
    {
        if (TryWin(out int i))
            return MakeMove(i);
        else if (TryBlock(out i))
            return MakeMove(i);
        else if (TryFork(out i))
            return MakeMove(i);
        else if (TryBlockFork(out i))
            return MakeMove(i);
        else if (TryCenter(out i))
            return MakeMove(i);
        else if (TryOppositeCorner(out i))
            return MakeMove(i);
        else if (TryEmptyCorner(out i))
            return MakeMove(i);
        else 
        {
            TryEmptySide(out i);
            return MakeMove(i);
        }
    }

    // 1. Win: If the player has two in a row, they can place a third to get three in a row.
    // returns true and out index to play if win is possible, returns false and index -1 if otherwise
    public bool TryWin (out int i)
    {
        for (i = 0; i < 9; ++i)
        {
            BoardPosition position = new BoardPosition(i);
            if (BoardAt(position) != ' ') 
                continue;

            if (
                ArePlayerPositions(position.Up().Up(), position.Up()) ||
                ArePlayerPositions(position.Left().Left(), position.Left())
            )
            {
                if (!(position.IsAlignedToDiagonal())) 
                    return true;
                else
                {
                    if (i != 4)
                    {
                        if (ArePlayerPositions(position.NextDiagonalSquare(), position.NextDiagonalSquare().NextDiagonalSquare()))
                            return true;
                    }
                    else
                    {
                        if (ArePlayerPositions(position.Down().Left(), position.Down().Left().Down().Left()))
                            return true;
                        if (ArePlayerPositions(position.Down().Right(), position.Down().Right().Down().Right()))
                            return true;
                    }
                }
            }
        }
        i = -1;
        return false;
    }
    // 2. Block: If the opponent has two in a row, the player must play the third themselves to block the opponent.
    // returns true and out index to play if block is possible, returns false and index -1 if otherwise
    public bool TryBlock (out int i)
    {
        for (i = 0; i < 9; ++i)
        {
            BoardPosition position = new BoardPosition(i);
            if (BoardAt(position) != ' ') 
                continue;

            if (
                AreOpponentPositions(position.Up().Up(), position.Up()) ||
                AreOpponentPositions(position.Left().Left(), position.Left())
            )
            {
                if (!(position.IsAlignedToDiagonal())) 
                    return true;
                else
                {
                    if (i != 4)
                    {
                        if (AreOpponentPositions(position.NextDiagonalSquare(), position.NextDiagonalSquare().NextDiagonalSquare()))
                            return true;
                    }
                    else
                    {
                        if (AreOpponentPositions(position.Down().Left(), position.Down().Left().Down().Left()))
                            return true;
                        if (AreOpponentPositions(position.Down().Right(), position.Down().Right().Down().Right()))
                            return true;
                    }
                }
            }
        }
        i = -1;
        return false;
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
    // 4. Blocking an opponent's fork: If there is only one possible fork for the opponent, the player should block it. Otherwise, the player should block all forks in any way that simultaneously allows them to create two in a row. Otherwise, the player should create a two in a row to force the opponent into defending, as long as it doesn't result in them creating a fork. For example, if "X" has two opposite corners and "O" has the center, "O" must not play a corner // move in order to win. (Playing a corner move in this scenario creates a fork for "X" to win.)
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
        else if (_board[8] == _opponent && _board[0] == ' ')
        {
            i = 0;
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
                if (ArePositionsOf(position.NextDiagonalSquare(), position.NextDiagonalSquare().NextDiagonalSquare(), player))
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
        if (player == _playingAs)
            return ArePlayerPositions(position1, position2);
        else
            return AreOpponentPositions(position1, position2);
    }

    public bool ArePlayerPositions (BoardPosition position1, BoardPosition position2)
    {
        return BoardAt(position1) == BoardAt(position2) && BoardAt(position1) == _playingAs;
    }

    public bool AreOpponentPositions (BoardPosition position1, BoardPosition position2)
    {
        return BoardAt(position1) == BoardAt(position2) && BoardAt(position1) == _opponent;
    }

    public char BoardAt (BoardPosition position)
    {
        return _board[position.ToArrayIndex()];
    }

    public string MakeMove (int index)
    {
        if (Board[index] != ' ') 
            return Board;
        char[] newBoard = _board.ToArray();
        newBoard[index] = _playingAs;
        Board = new string(newBoard);
        return Board;
    }
    
    // verifies that board is with equal number of Os and Xs or less Os than Xs
    // this garantees the possibility of being o's turn
    public bool IsOsTurn ()
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
        return OCount <= XCount;
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
                current == BoardAt(position.NextDiagonalSquare()) && 
                current == BoardAt(position.NextDiagonalSquare().NextDiagonalSquare()))
            )
                return true;
            else continue;
        }
        return false;
    }

    // verifies that the board string is 9 length and only contains x's, o's or spaces
    public static bool IsValidBoard (string board) 
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