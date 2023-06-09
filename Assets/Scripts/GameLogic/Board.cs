using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>Board</c> holds a 2D array equal to the dimensions of the game and int values for player-made moves
/// </summary>
public class Board
{
    private int[,] _board;

    private int _rowCount;
    private int _colCount;

    public int RowCount { get => _rowCount;}
    public int ColCount { get => _colCount;}

    /// <summary>
    /// Constructor <c>Board</c> creates a 2D array of given size (rows/cols
    /// </summary>
    public Board(int rows, int collums)
    {
        _board = new int[rows, collums];
        _rowCount = rows;
        _colCount = collums;
    }
    /// <summary>
    /// Constructor <c>Board</c> creates a new board thats a copy of the given board
    /// </summary>
    public Board(Board board)
    {
        _rowCount = board._rowCount;
        _colCount = board._colCount;
        _board = new int[_rowCount, _colCount];
        Array.Copy(board._board, _board, board._board.Length);
    }

    /// <summary>
    /// Method <c>is_FreeAt</c> checks if the given position is free
    /// </summary>
    public bool is_FreeAt(Vector2Int position)
    {
        return getValueAt(position) == 0;
    }

    /// <summary>
    /// Method <c>getValueAt</c> returns the value at the given position, if the position is invallid it instead returns <c>-1</c>
    /// </summary>
    public int getValueAt(Vector2Int position)
    {
        int value;
        try
        {
            value = _board[position.x, position.y];
        }
        catch (System.Exception)
        {
            value = -1;
        }
        return value;
    }

    /// <summary>
    /// Method <c>freePositions()</c> returns all free positions on the board, or an empty list if there are none
    /// </summary>
    public List<Vector2Int> freePositions()
    {
        List<Vector2Int> freePosistions = new List<Vector2Int>();

        for (int y = 0; y < _colCount; y++)
        {
            Vector2Int position = new Vector2Int(0, y);
            if (is_FreeAt(position)) freePosistions.Add(position);
        }

        return freePosistions;
    }

    /// <summary>
    /// Method <c>getRandomFreePosition()</c> returns a random free position on the board. If there are no positions left <c>Vector2(-1,-1)</c> is returned
    /// </summary>
    public Vector2Int getRandomFreePosition()
    {
        List<Vector2Int> temp = freePositions();

        if (temp.Count == 0) return Vector2Int.one * -1;

        int rd = UnityEngine.Random.Range(0, temp.Count);
        return temp[rd];
    }

    /// <summary>
    /// Method <c>isBoardFull()</c> returns true if the board is full.
    /// </summary>
    public bool isBoardFull()
    {
        for (int y = 0; y < _colCount; y++)
        {
            if (is_FreeAt(new Vector2Int(0, y))) return false;
        }

        return true;
    }

    /// <summary>
    /// Method <c>placePosition()</c> assigns the playerID value on the given position. The function tries to put the ID at the deepest possible place. 
    /// If the position is invalid Vector2(-1,-1) is returned, if the board is full Vector2(-2,-2) is returned.
    /// </summary>
    public Vector2Int placePosition(Vector2Int position, int playedID)
    {
        int row = position.x;
        int col = position.y;
        while(getValueAt(new Vector2Int(row,col)) == 0)
        {
            row++;
        }

        try
        {
            _board[row - 1, col] = playedID;
            return new Vector2Int(row-1, col);
        }
        catch (Exception)
        {
            Debug.LogError("Invalid Move - Column is full");
        }
        return isBoardFull() ? Vector2Int.one * -2 : Vector2Int.one * -1;
    }

    /// <summary>
    /// Method <c>ToString()</c> prints the current board state to in a string format
    /// </summary>
    public override string ToString()
    {
        string temp = "";

        for (int row = 0; row < _rowCount; row++)
        {
            for (int col = 0; col < _colCount; col++)
            {
                temp += _board[row, col] + " ";
            }
            temp += "\n";
        }

        return temp;
    }
}
