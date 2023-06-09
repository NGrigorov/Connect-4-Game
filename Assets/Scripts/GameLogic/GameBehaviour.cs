using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    public static int currentPlayerTurn = 1;

    [SerializeField] private List<Transform> positionTransforms;
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;

    private Connect4Game _game;
    private Vector3[,] _positions;
    private List<GameObject> _players;

    private bool _isAIOpponent;
    private MCTSNode _root_node;
    

    private int rows = 6, cols = 7;
    void Start()
    {
        _game = new Connect4Game(rows, cols);
        _positions = new Vector3[rows, cols];
        
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


        //if (_isAIOpponent)
        //{
        //    Vector2Int aiMove = aiPlay(1000);
        //    _game.Board.placePosition(aiMove, 2);
        //    Instantiate(_players[1], _positions[aiMove.x, aiMove.y], Quaternion.identity);

        //    Debug.Log(_game.Victory(2, aiMove) ? "Player " + 2 + " won!" : "No winner");
        //    //Debug.Log("END");
        //}
    }


    public void PlacePosition2P(int playedID, Vector2Int position)
    {
        Vector2Int pos = _game.Board.placePosition(position, playedID);
        if(pos != Vector2Int.one * -1) { 
            Instantiate(_players[playedID-1], _positions[pos.x, pos.y], Quaternion.identity);

            Debug.Log(_game.Board.ToString());
            Debug.Log(_game.Victory(playedID, pos) ? "Player " + currentPlayerTurn.ToString() + " won!" : "No winner");
            currentPlayerTurn = currentPlayerTurn == 1 ? 2 : 1;
        }
    }

    public void PlacePosition1P(Vector2Int position)
    {
        Vector2Int pos = _game.Board.placePosition(position, 1);
        if (pos != Vector2Int.one * -1)
        {
            Instantiate(_players[0], _positions[pos.x, pos.y], Quaternion.identity);

            
            Debug.Log(_game.Victory(1, pos) ? "Player " + 1 + " won!" : "No winner");

            if (_isAIOpponent) 
            { 
                Vector2Int aiMove = aiPlay(1000);
                _game.Board.placePosition(aiMove, 2);
                Instantiate(_players[1], _positions[aiMove.x, aiMove.y], Quaternion.identity);

                Debug.Log(_game.Victory(2, aiMove) ? "Player " + 2 + " won!" : "No winner");
                //Debug.Log("END");
            }

            //Debug.Log(_game.Board.ToString());
        }
    }

    public Vector2Int aiPlay(int iterations)
    {
        //Debug.Log("START");
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
        MCTSNode best_move = null;
        List<MCTSNode> best_moves = new List<MCTSNode>();
        foreach (MCTSNode child in _root_node.childrens)
        {
            if(child._visits != 0)
            {
                float averageWin = child._score / child._visits;

                if(averageWin >= bestAvgWin)
                {
                    bestAvgWin = averageWin;
                    best_move = child;
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
