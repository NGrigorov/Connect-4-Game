using UnityEngine;
/// <summary>
/// Class <c>Connect4Game</c> represents a game of a Connect-4
/// </summary>
public class Connect4Game
{
    private Board _board;

    public Board Board { get => _board;}

    /// <summary>
    /// Constructor <c>Connect4Game</c> creates a Connect-4 game with a board size of given dimensions (rows/cols)
    /// </summary>
    public Connect4Game(int rows, int collums)
    {
        _board = new Board(rows, collums);
    }

    /// <summary>
    /// Constructor <c>Connect4Game</c> creates a new Connect-4 game that is a copy of the given game
    /// </summary>
    public Connect4Game(Connect4Game game)
    {
        _board = new Board(game.Board);
    }

    /// <summary>
    /// Method <c>Victory</c> returns true if the given position and playerID have a winning pattern else returns false.
    /// </summary>
    public bool Victory(int playedID, Vector2Int position)
    {
        return verticalWin(playedID, position) || horizontalWin(playedID, position) || diagonalWin(playedID, position);
    }

    /// <summary>
    /// Method <c>verticalWin</c> returns true if there is a winning pattern vertically else returns false
    /// </summary>
    private bool verticalWin(int playerID, Vector2Int position)
    {
        int count = 1;
        for (int x = position.x + 1; x < _board.RowCount; x++)
        {
            if (_board.getValueAt(new Vector2Int(x, position.y)) != playerID) break;
            count++;
        }

        for (int x = position.x - 1; x >= 0; x--)
        {
            if (_board.getValueAt(new Vector2Int(x, position.y)) != playerID) break;
            count++;
        }

        if (count >= 4) return true;
        
        return false;
    }

    /// <summary>
    /// Method <c>horizontalWin</c> returns true if there is a winning pattern horizontally else returns false
    /// </summary>
    private bool horizontalWin(int playerID, Vector2Int position)
    {
        int count = 1;
        for (int y = position.y + 1; y < _board.ColCount; y++)
        {
            if (_board.getValueAt(new Vector2Int(position.x, y)) != playerID) break;
            count++;
        }

        for (int y = position.y - 1; y >= 0; y--)
        {
            if (_board.getValueAt(new Vector2Int(position.x, y)) != playerID) break;
            count++;
        }

        if (count >= 4) return true;

        return false;
    }

    /// <summary>
    /// Method <c>diagonalWin</c> returns true if there is a winning pattern diagonally else returns false
    /// </summary>
    private bool diagonalWin(int playerID, Vector2Int position)
    {
        //Left to right diagonal
        int count = 1;

        int x = position.x + 1;
        int y = position.y + 1;

        //From top to bottom
        for (int i = 0; i < 4; i++)
        {
            if (_board.getValueAt(new Vector2Int(x, y)) != playerID) break;
            count++;
            x++;
            y++;
        }

        x = position.x - 1;
        y = position.y - 1;

        //From bottom to top
        for (int i = 0; i < 4; i++)
        {
            if (_board.getValueAt(new Vector2Int(x, y)) != playerID) break;
            count++;
            x--;
            y--;
        }

        if (count >= 4) return true;

        //Right to left diagonal
        count = 1;

        x = position.x + 1;
        y = position.y - 1;

        //From top to bottom
        for (int i = 0; i < 4; i++)
        {
            if (_board.getValueAt(new Vector2Int(x, y)) != playerID) break;
            count++;
            x++;
            y--;
        }

        x = position.x - 1;
        y = position.y + 1;

        //From bottom to top
        for (int i = 0; i < 4; i++)
        {
            if (_board.getValueAt(new Vector2Int(x, y)) != playerID) break;
            count++;
            x--;
            y++;
        }

        if (count >= 4) return true;

        return false;
    }
}
