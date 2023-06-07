using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    private int[,] _board;

    private int _rowCount;
    private int _colCount;

    public int RowCount { get => _rowCount;}
    public int ColCount { get => _colCount;}

    public Board(int rows, int collums)
    {
        _board = new int[rows, collums];
        _rowCount = rows;
        _colCount = collums;
    }

    public Board(Board board)
    {
        _rowCount = board._rowCount;
        _colCount = board._colCount;
        _board = new int[_rowCount, _colCount];
        Array.Copy(board._board, _board, board._board.Length);
    }

    public bool is_FreeAt(Vector2Int position)
    {
        return getValueAt(position) == 0;
    }

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

    public Vector2Int getRandomFreePosition()
    {
        List<Vector2Int> temp = freePositions();

        if (temp.Count == 0) return Vector2Int.one * -1;

        int rd = UnityEngine.Random.Range(0, temp.Count);
        return temp[rd];
    }

    public bool isBoardFull()
    {
        for (int y = 0; y < _colCount; y++)
        {
            if (is_FreeAt(new Vector2Int(0, y))) return false;
        }

        return true;
    }

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
        return Vector2Int.one * -1;
    }

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
