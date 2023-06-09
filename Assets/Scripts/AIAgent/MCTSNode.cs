using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
/// <summary>
/// Class <c>MCTSNode</c> holds a game-state, a move and the value of the move
/// </summary>
public class MCTSNode
{
    public Connect4Game game;
    public MCTSNode parent;
    public MCTSNode root;
    public List<MCTSNode> childrens;

    public float visits;
    public float score;
    public float ucb;

    public bool visited;
    public bool end_node;
    public bool end;

    public int playerID;
    public int myID;

    public Vector2Int boardMove;
    public Vector2Int placementMove;

    /// <summary>
    /// Constructor <c>MCTSNode</c> creates a node with a copy of the give game, parent, root, playerID and board placement
    /// </summary>
    public MCTSNode(Connect4Game game, MCTSNode parent, MCTSNode root, int playerID, Vector2Int boardPlacement, Vector2Int boardMove)
    {
        this.game = new Connect4Game(game);
        this.parent = parent;
        this.root = root;
        this.playerID = playerID;
        this.boardMove = boardMove;
        this.placementMove = boardPlacement;

        childrens = new List<MCTSNode>();
        visits = 0;
        score = 0;
        ucb = float.PositiveInfinity;
        visited = false;
        end_node = false;
        end = false;
        myID = 2;
    }

    /// <summary>
    /// Method <c>computeUCB</c> computes the value of the node that is used for <c>selection</c> return decision. Uses 1.4f (rough version of Sqrt(2))
    /// </summary>
    public void computeUCB()
    {
        if (this.visits == 0) return;

        if(this.playerID == 2)
        {
            this.ucb = this.score / this.visits + 1.4f * Mathf.Sqrt(Mathf.Log(this.root.visits) / this.visits);
        }
        else
        {
            this.ucb = (-this.score) / this.visits + 1.4f * Mathf.Sqrt(Mathf.Log(this.root.visits) / this.visits);
        }
    }

    /// <summary>
    /// Method <c>computeVisits</c> adds the given visit and score to the node
    /// </summary>
    public void computeVisits(float visits, float score)
    {
        this.visits += visits;
        this.score += score;
    }

    /// <summary>
    /// Method <c>backPropagate</c> propagates the given visit and score to the parent node until the root node
    /// </summary>
    public void backPropagate(float visits, float score)
    {
        MCTSNode parent = this.parent;

        while (parent != null)
        {
            parent.computeVisits(visits, score);
            parent = parent.parent;
        }
    }

    /// <summary>
    /// Method <c>setEndNode</c> tags the node as end_node. Blocks it from being selected in the <c>selection</c> method
    /// </summary>
    public float setEndNode()
    {
        float result = 0;
        if(this.game.Victory(this.playerID, this.boardMove))
        {
            this.visited = true;
            this.end_node = true;
            if (this.playerID == this.myID) result = 1000f;
            else result = -1000f;
        }
        else if (this.game.Board.isBoardFull())
        {
            this.visited = true;
            this.end_node = true;
            result = 0f;
        }

        return result;
    }

    /// <summary>
    /// Method <c>rollout</c> simulates a play to determine outcome of the current game state and calculate a value
    /// </summary>
    public void rollout()
    {
        float result = setEndNode();

        if (!this.visited)
        {
            Connect4Game tempCopy = new Connect4Game(this.game);
            int winnerID = simulateGame(tempCopy);

            if (winnerID == 0)
            {
                result = 0f;
            }
            else if(winnerID == this.myID)
            {
                result = 1f;
            }
            else
            {
                result = -1f;
            }
        }

        this.visited = true;
        this.computeVisits(1, result);
        this.backPropagate(1, result);

    }

    /// <summary>
    /// Method <c>simulateGame</c> simulates a play from the given game state
    /// </summary>
    public int simulateGame(Connect4Game simGame)
    {
        if(this.playerID == 1)
        {
            Vector2Int move = simGame.Board.getRandomFreePosition();

            if (move == Vector2Int.one * -1) return 0;

            Vector2Int realMove = simGame.Board.placePosition(move, 2);

            if (game.Victory(2, realMove)) { return 2; }
        }

        while (!simGame.Board.isBoardFull()) 
        {
            for (int simPlayerID = 1; simPlayerID <= 2; simPlayerID++)
            {
                Vector2Int move = simGame.Board.getRandomFreePosition();

                if (move == Vector2Int.one * -1) break;

                Vector2Int realMove = simGame.Board.placePosition(move, simPlayerID);

                if (game.Victory(simPlayerID, realMove)) { return simPlayerID; }
            }
        }
        return 0;
    }

    /// <summary>
    /// Method <c>selection</c> selects the deepest node with the highest UCB
    /// </summary>
    public MCTSNode selection()
    {
        if (this.childrens.Count == 0) return this;

        foreach (MCTSNode node in this.childrens)
        {
            if(node.visited) node.computeUCB();
        }

        float best_ucb = -10000;
        MCTSNode best_child = null;

        foreach (MCTSNode node in this.childrens)
        {
            if (node.ucb >= best_ucb && !node.end_node)
            {
                best_ucb = node.ucb;
                best_child = node;
            }
        }

        if (best_child == null) { this.root.end = true; return null; }
        return best_child.selection();
    }

    /// <summary>
    /// Method <c>expand</c> creates children for the given node
    /// </summary>
    public void expand(List<Vector2Int> possibleMoves, int pID)
    {
        foreach (Vector2Int move in possibleMoves)
        {
            Connect4Game childGame = new Connect4Game(this.game);
            Vector2Int realMove = childGame.Board.placePosition(move, pID);
            MCTSNode node = new MCTSNode(childGame, this, this.root, pID, move, realMove);
            this.childrens.Add(node);
        }
    }
}