using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Connect4Game
{
    private Board _board;

    public Board Board { get => _board;}

    public Connect4Game(int rows, int collums)
    {
        _board = new Board(rows, collums);
    }

    public Connect4Game(Connect4Game game)
    {
        _board = new Board(game.Board);
    }

    public bool Victory(int playedID, Vector2Int position)
    {
        return horizontalWin(playedID, position) || verticalWin(playedID, position) || diagonalWin(playedID, position);
    }

    private bool horizontalWin(int playerID, Vector2Int position)
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

    private bool verticalWin(int playerID, Vector2Int position)
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
