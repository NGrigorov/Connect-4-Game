using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    public static int currentPlayerTurn = 1;

    [SerializeField] private List<RectTransform> positionTransforms;
    [SerializeField] private List<TextMeshProUGUI> historyFields;
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;

    //Game over screen
    [SerializeField] private TextMeshProUGUI winScreenText;
    [SerializeField] private GameObject panel;



    private Connect4Game _game;
    private RectTransform[,] _positions;
    private List<GameObject> _players;

    private MCTSNode _root_node;
    private GameSettings _settings;

    public static Color _p1Color = new Color(0, 0, 1);
    public static Color _p2Color = new Color(1, 0, 0);

    private int rows = 6, cols = 7;
    private int historyCounter = 0;
    private int turnCounter = 1;
    private bool gameIsOver = false;

    void Start()
    {
        _game = new Connect4Game(rows, cols);
        _positions = new RectTransform[rows, cols];
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
                _positions[row, col] = positionTransforms[row + col * rows];
            }
        }

        if (_settings.playVSAI) 
        {
            if (_settings.aiFirst)
            {
                currentPlayerTurn = 2;
                ComputerMakeMove();
            }
        }
    }

    public void showEndGameScreen(string winer)
    {
        panel.SetActive(true);
        winScreenText.text = winer;
        gameIsOver = true;
    }

    public void GameIsOver(string winner)
    {
        fillHistory(winner);
        showEndGameScreen(winner);
    }

    public void invalidMove(string error)
    {
        historyFields[historyCounter].text = error;

        if (currentPlayerTurn == 1) historyFields[historyCounter].color = _p1Color;
        else historyFields[historyCounter].color = _p2Color;

        historyCounter = (historyCounter + 1) % historyFields.Count;
    }

    public void fillHistory(string line)
    {

        historyFields[historyCounter].text = line + " | Turn: " + turnCounter;

        if (currentPlayerTurn == 1) historyFields[historyCounter].color = _p1Color;
        else historyFields[historyCounter].color = _p2Color;

        historyCounter = (historyCounter + 1) % historyFields.Count;
        turnCounter++;
    }

    public void MakeMove(int playedID, Vector2Int position)
    {
        if (!gameIsOver)
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
    }


    public void PlacePosition2P(int playedID, Vector2Int position)
    {
        Vector2Int pos = _game.Board.placePosition(position, playedID);

        if (pos == Vector2Int.one * -1) { invalidMove("Column is full!"); return; }

        Instantiate(_players[playedID - 1], _positions[pos.x, pos.y]);
        Instantiate(_players[playedID - 1], _positions[pos.x, pos.y]);
        if (_game.Victory(playedID, pos)) { GameIsOver("Winner is player: " + playedID + "!"); return;  }

        fillHistory("Player " + playedID + ": " + pos);
        if (_game.Board.isBoardFull()) { GameIsOver("Draw!"); return; }

        currentPlayerTurn = currentPlayerTurn == 1 ? 2 : 1;
    }

    public void PlacePosition1P(Vector2Int position)
    {
        currentPlayerTurn = 1;
        Vector2Int pos = _game.Board.placePosition(position, 1);

        //Player didnt make a valid move -> selected a column thats full
        if (pos == Vector2Int.one * -1) { invalidMove("Column is full!"); return; }

        Instantiate(_players[0], _positions[pos.x, pos.y]);
        if (_game.Victory(1, pos)) { GameIsOver("Winner is player: " + 1 + "!"); return; }
        
        fillHistory("Player " + 1 + ": " + pos);
        if (_game.Board.isBoardFull()) { GameIsOver("Draw!"); return; }

        currentPlayerTurn = 2;
        ComputerMakeMove();

    }

    public void ComputerMakeMove()
    {   
        Vector2Int aiMove = aiPlay((int)_settings.difficulty);
        Vector2Int pos = _game.Board.placePosition(aiMove, 2);

        Instantiate(_players[1], _positions[aiMove.x, aiMove.y]);
        if (_game.Victory(2, pos)) { GameIsOver("Winner is player: " + 2 + "!"); return; }

        fillHistory("Computer: " + pos);
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
                if (_game.Board.getValueAt(new Vector2Int(i, j)) == 1) Instantiate(player1Prefab, _positions[i, j]);
                if (_game.Board.getValueAt(new Vector2Int(i, j)) == 2) Instantiate(player2Prefab, _positions[i, j]);
            }
        }
    }

    public static bool ApproximatelyEqual(float a, float b, float threshold = 0.01f)
    {
        return Mathf.Abs(a - b) < threshold;
    }

}
