using UnityEngine;
using TMPro;

/// <summary>
/// Class <c>PlacementBehaviour</c> manages the moves made on the board. Holds a refference to the <c>GameBehaviour</c> and calls its <c>MakeMove</c> method
/// </summary>
public class PlacementBehaviour : MonoBehaviour
{
    [SerializeField] private Vector2Int position;
    [SerializeField] private TextMeshProUGUI text;
    private GameBehaviour manager;

    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameBehaviour>();
    }

    private void OnMouseDown()
    {
        manager.MakeMove(GameBehaviour.currentPlayerTurn, position);
        if (!GameSettings.Instance.playVSAI)
        {
            text.text = "Player " + GameBehaviour.currentPlayerTurn;

            if (GameBehaviour.currentPlayerTurn == 1) text.color = GameBehaviour._p1Color;
            else text.color = GameBehaviour._p2Color;
        }

        else
        {
            text.text = "Player 1";
        }

    }
}
