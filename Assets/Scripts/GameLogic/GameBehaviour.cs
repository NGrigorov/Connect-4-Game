using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

public class GameBehaviour : MonoBehaviour
{
    public static int currentPlayerTurn = 1;

    [SerializeField] private List<Transform> positionTransforms;
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;
    [SerializeField] private TextMeshProUGUI text;


    private Connect4Game _game;
    private Vector3[,] _positions;
    private List<GameObject> _players;

    private bool _isAIOpponent;
    private MCTSNode _root_node;
    private GameSettings _settings;
    

    private int rows = 6, cols = 7;
    void Start()
    {
        _game = new Connect4Game(rows, cols);
        _positions = new Vector3[rows, cols];
        _settings = GameSettings.Instance;

        _players = new List<GameObject>
        {
            player1Prefab,
            player2Prefab
        };

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                _positions[row, col] = positionTransforms[row + col * rows].position;
            }
        }

        _isAIOpponent = true;

        Debug.Log(_game.Board.ToString());

        if (_settings.playVSAI) 
        {
            if (!_settings.playFirst)
            {
                ComputerMakeMove();
            }
        }
    }

    public void GameIsOver(string winner)
    {
        fillHistory(winner);
    }

    public void invalidMove(string error)
    {
        fillHistory(error);
    }

    public void fillHistory(string line)
    {
        text.text += "\n" + line;
    }

    public void MakeMove(int playedID, Vector2Int position)
    {
        if (_settings.playVSAI)
        {
            PlacePosition1P(position);
        }
        else
        {
            PlacePosition2P(playedID, position);
        }
    }


    public void PlacePosition2P(int playedID, Vector2Int position)
    {
        currentPlayerTurn = currentPlayerTurn == 1 ? 2 : 1;

        Vector2Int pos = _game.Board.placePosition(position, playedID);

        if (pos == Vector2Int.one * -1) { invalidMove("Column is full!"); return; }

        Instantiate(_players[playedID - 1], _positions[pos.x, pos.y], Quaternion.identity);
        if (_game.Victory(playedID, pos)) { GameIsOver("Winner is player: " + playedID + "!"); return;  }

        fillHistory("Player " + playedID + " Made a move on: " + pos);
        if (_game.Board.isBoardFull()) { GameIsOver("Draw!"); return; }

        
    }

    public void PlacePosition1P(Vector2Int position)
    {
        Vector2Int pos = _game.Board.placePosition(position, 1);

        //Player didnt make a valid move -> selected a column thats full
        if (pos == Vector2Int.one * -1) { invalidMove("Column is full!"); return; }

        Instantiate(_players[0], _positions[pos.x, pos.y], Quaternion.identity);
        if (_game.Victory(1, pos)) { GameIsOver("Winner is player: " + 1 + "!"); return; }
        
        fillHistory("Player " + 1 + " made a move on: " + pos);
        if (_game.Board.isBoardFull()) { GameIsOver("Draw!"); return; }

        ComputerMakeMove();
        
    }

    public void ComputerMakeMove()
    {   
        Vector2Int aiMove = aiPlay((int)_settings.difficulty);
        Vector2Int pos = _game.Board.placePosition(aiMove, 2);

        Instantiate(_players[1], _positions[aiMove.x, aiMove.y], Quaternion.identity);
        if (_game.Victory(2, pos)) { GameIsOver("Winner is player: " + 2 + "!"); return; }

        fillHistory("Computer made a move on: " + pos);
        //Ai couldn't make a move -> Board is full (AI wont select a full column)
        if (_game.Board.isBoardFull()) { GameIsOver("Draw!"); return; }
    }

    public Vector2Int aiPlay(int iterations)
    {
        _root_node = new MCTSNode(_game, null, null, 2, Vector2Int.one * -1, Vector2Int.one * -1);
        _root_node.visited = true;
        _root_node.root = _root_node;
        _root_node.expand(_game.Board.freePositions(), 2);
        for (int i = 0; i < iterations; i++)
        {
            if (this._root_node.end) break;

            MCTSNode selectedNode = _root_node.selection();
            if (selectedNode == null) continue;

            int playerID = 2;
            if(selectedNode.playerID == playerID) playerID = 1;

            if (selectedNode.visited)
            {
                selectedNode.expand(selectedNode.game.Board.freePositions(), playerID);
                MCTSNode best_node = selectedNode.selection();
                best_node.rollout();
            }
            else
            {
                selectedNode.rollout();
            }
        }
        float bestAvgWin = -10000;
        List<MCTSNode> best_moves = new List<MCTSNode>();
        foreach (MCTSNode child in _root_node.childrens)
        {
            if(child._visits != 0)
            {
                float averageWin = child._score / child._visits;

                if(averageWin >= bestAvgWin)
                {
                    bestAvgWin = averageWin;
                }
            }
        }
        foreach (MCTSNode child in _root_node.childrens)
        {
            float averageWin = child._score / child._visits;

            if (ApproximatelyEqual(averageWin, bestAvgWin))
            {
                best_moves.Add(child);
            }
        }
        return best_moves[Random.Range(0, best_moves.Count)].boardMove;
        //return best_move.boardMove;
    }

    public void GenerateBoardPositions()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (_game.Board.getValueAt(new Vector2Int(i, j)) == 1) Instantiate(player1Prefab, _positions[i, j], Quaternion.identity);
                if (_game.Board.getValueAt(new Vector2Int(i, j)) == 2) Instantiate(player2Prefab, _positions[i, j], Quaternion.identity);
            }
        }
    }

    public static bool ApproximatelyEqual(float a, float b, float threshold = 0.01f)
    {
        return Mathf.Abs(a - b) < threshold;
    }

}
