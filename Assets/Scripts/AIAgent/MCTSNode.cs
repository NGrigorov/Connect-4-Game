using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class MCTSNode
{
    public Connect4Game game;
    public MCTSNode parent;
    public MCTSNode root;
    public List<MCTSNode> childrens;

    public float _visits;
    public float _score;
    public float ucb;

    public bool visited;
    public bool end_node;
    public bool end;

    public int playerID;
    public int myID;

    public Vector2Int boardMove;
    public Vector2Int placementMove;

    public MCTSNode(Connect4Game game, MCTSNode parent, MCTSNode root, int playerID, Vector2Int boardPlacement, Vector2Int boardMove)
    {
        this.game = new Connect4Game(game);
        this.parent = parent;
        this.root = root;
        this.playerID = playerID;
        this.boardMove = boardMove;
        this.placementMove = boardPlacement;

        childrens = new List<MCTSNode>();
        _visits = 0;
        _score = 0;
        ucb = float.PositiveInfinity;
        visited = false;
        end_node = false;
        end = false;
        myID = 2;
    }

    public void computeUCB()
    {
        if (this._visits == 0) return;

        if(this.playerID == 2)
        {
            this.ucb = this._score / this._visits + 1.4f * Mathf.Sqrt(Mathf.Log(this.root._visits) / this._visits);
        }
        else
        {
            this.ucb = (-this._score) / this._visits + 1.4f * Mathf.Sqrt(Mathf.Log(this.root._visits) / this._visits);
        }
    }

    public void computeVisits(float visits, float score)
    {
        this._visits += visits;
        this._score += score;
    }

    public void backPropagate(float visits, float score)
    {
        MCTSNode parent = this.parent;

        while (parent != null)
        {
            parent.computeVisits(visits, score);
            parent = parent.parent;
        }
    }

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