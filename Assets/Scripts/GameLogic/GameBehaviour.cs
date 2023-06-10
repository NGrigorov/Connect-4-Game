using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Class <c>GameBehaviour</c> acts as a manager of the current game. Regulates the turn order and AI decision making.
/// </summary>
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

    //Sounds
    [SerializeField] private AudioSource scribble;



    private Connect4Game _game;
    private RectTransform[,] _positions;
    private List<GameObject> _players;

    private MCTSNode _root_node;
    private GameSettings _settings;

    public static Color _p1Color = new Color(0, 0, 1);
    public static Color _p2Color = new Color(1, 0, 0);

    private int _rows = 6, _cols = 7;
    private int _historyCounter = 0;
    private int _turnCounter = 1;
    private bool _gameIsOver = false;

    /// <summary>
    /// Method <c>Start</c> acts as a Constructor of the Class. Creates a new Connect-4 game, world positions, and assigns the game settings.
    /// </summary>
    void Start()
    {
        _game = new Connect4Game(_rows, _cols);
        _positions = new RectTransform[_rows, _cols];
        _settings = GameSettings.Instance;

        _players = new List<GameObject>
        {
            player1Prefab,
            player2Prefab
        };

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                _positions[row, col] = positionTransforms[row + col * _rows];
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

    void playScribbleSound()
    {
        scribble.Play();
    }

    /// <summary>
    /// Method <c>showEndGameScreen</c> shows the end game panel when a winner or a draw is achieved
    /// </summary>
    public void showEndGameScreen(string winer)
    {
        panel.SetActive(true);
        winScreenText.text = winer;
        _gameIsOver = true;
    }

    /// <summary>
    /// Method <c>GameIsOver</c> adds the winner or draw to the history panel and calls <c>showEndGameScreen</c>
    /// </summary>
    public void GameIsOver(string winner)
    {
        fillHistory(winner);
        showEndGameScreen(winner);
    }

    /// <summary>
    /// Method <c>invalidMove</c> adds the error message to the history panel without using up a turn
    /// </summary>
    public void invalidMove(string error)
    {
        historyFields[_historyCounter].text = error;

        if (currentPlayerTurn == 1) historyFields[_historyCounter].color = _p1Color;
        else historyFields[_historyCounter].color = _p2Color;

        _historyCounter = (_historyCounter + 1) % historyFields.Count;
    }

    /// <summary>
    /// Method <c>fillHistory</c> adds a per turn message to the history panel 
    /// </summary>
    public void fillHistory(string line)
    {

        historyFields[_historyCounter].text = line + " | Turn: " + _turnCounter;

        if (currentPlayerTurn == 1) historyFields[_historyCounter].color = _p1Color;
        else historyFields[_historyCounter].color = _p2Color;

        _historyCounter = (_historyCounter + 1) % historyFields.Count;
        _turnCounter++;
    }

    /// <summary>
    /// Method <c>MakeMove</c> makes a move on the board with the given player ID and position
    /// </summary>
    public void MakeMove(int playedID, Vector2Int position)
    {
        if (!_gameIsOver)
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

    /// <summary>
    /// Method <c>PlacePosition2P</c> adds the position on the board and UI canvas depending on playerID calls it
    /// </summary>
    public void PlacePosition2P(int playedID, Vector2Int position)
    {
        Vector2Int pos = _game.Board.placePosition(position, playedID);

        if (pos == Vector2Int.one * -1) { invalidMove("Column is full!"); return; }

        Instantiate(_players[playedID - 1], _positions[pos.x, pos.y]);
        playScribbleSound();
        if (_game.Victory(playedID, pos)) { GameIsOver("Winner is player: " + playedID + "!"); return;  }

        fillHistory("Player " + playedID + ": " + pos);
        if (_game.Board.isBoardFull()) { GameIsOver("Draw!"); return; }

        currentPlayerTurn = currentPlayerTurn == 1 ? 2 : 1;
    }

    /// <summary>
    /// Method <c>PlacePosition1P</c> adds the position on the board and UI canvas for player 1 then adds a position made from the AI
    /// </summary>
    public void PlacePosition1P(Vector2Int position)
    {
        currentPlayerTurn = 1;
        Vector2Int pos = _game.Board.placePosition(position, 1);

        //Player didnt make a valid move -> selected a column thats full
        if (pos == Vector2Int.one * -1) { invalidMove("Column is full!"); return; }

        Instantiate(_players[0], _positions[pos.x, pos.y]);
        playScribbleSound();
        if (_game.Victory(1, pos)) { GameIsOver("Winner is player: " + 1 + "!"); return; }
        
        fillHistory("Player " + 1 + ": " + pos);
        if (_game.Board.isBoardFull()) { GameIsOver("Draw!"); return; }

        currentPlayerTurn = 2;
        ComputerMakeMove();

    }

    /// <summary>
    /// Method <c>ComputerMakeMove</c> calls <c>aiPlay</c> with the given iterations and returns the a move if possible
    /// </summary>
    public void ComputerMakeMove()
    {   
        Vector2Int aiMove = aiPlay((int)_settings.difficulty);
        Vector2Int pos = _game.Board.placePosition(aiMove, 2);

        Instantiate(_players[1], _positions[aiMove.x, aiMove.y]);
        playScribbleSound();
        if (_game.Victory(2, pos)) { GameIsOver("Winner is player: " + 2 + "!"); return; }

        fillHistory("Computer: " + pos);
        //Ai couldn't make a move -> Board is full (AI wont select a full column)
        if (_game.Board.isBoardFull()) { GameIsOver("Draw!"); return; }
    }

    /// <summary>
    /// Method <c>aiPlay</c> creates a game tree and evaluates a move for the AI to make then returns a random best move
    /// </summary>
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
            if(child.visits != 0)
            {
                float averageWin = child.score / child.visits;

                if(averageWin >= bestAvgWin)
                {
                    bestAvgWin = averageWin;
                }
            }
        }
        foreach (MCTSNode child in _root_node.childrens)
        {
            float averageWin = child.score / child.visits;

            if (ApproximatelyEqual(averageWin, bestAvgWin))
            {
                best_moves.Add(child);
            }
        }
        return best_moves[Random.Range(0, best_moves.Count)].boardMove;
        //return best_move.boardMove;
    }

    /// <summary>
    /// Method <c>ApproximatelyEqual</c> compares two floats with a threshold
    /// </summary>
    public static bool ApproximatelyEqual(float a, float b, float threshold = 0.01f)
    {
        return Mathf.Abs(a - b) < threshold;
    }

}
