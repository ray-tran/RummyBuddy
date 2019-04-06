using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Turn { PlayerDraw, PlayerDiscard, AI };

public class Round : MonoBehaviour
{
    public static Round instance;
    public Turn CurrentTurn;
    public CardSlot DiscardPile;
    public CardSlot DrawPile;
    private PlayerHand PlayerHand;
    private AIHand AIHand;

    void Awake()
    {
        instance = this;
        PlayerHand = PlayerHand.instance;
        AIHand = AIHand.instance;
    }

    public void InitializeRound()
    {
        UpdateTurn(Turn.PlayerDraw);
        PlayerHand.instance.InitializeHand();
        AIHand.instance.InitializeHand();
    }

    public void UpdateTurn(Turn newTurn)
    {
        CurrentTurn = newTurn;
        GameUI.instance.currentTurnText.text = "Current turn: " + newTurn.ToString();
    }

    public void CalculateAndUpdateScore()
    {
        //Call UpdateMatchResult in Match.cs
    }

}
