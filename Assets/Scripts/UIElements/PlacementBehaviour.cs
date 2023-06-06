using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        manager.PlacePosition1P(position);
        text.text = "Player " + GameBehaviour.currentPlayerTurn;
    }
}
