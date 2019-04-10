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
        GameUI.instance.UpdateScoreUI();
    }

    public void CalculateAndUpdateScore()
    {
        int playerScore = PlayerHand.instance.DeadwoodPoints;
        int AIScore = AIHand.instance.DeadwoodPoints;
        string winner;
        int winScore;
        if (playerScore < AIScore)
        {
            winner = "ai";
            winScore = AIScore - playerScore;
        }
        else if (playerScore > AIScore)
        {
            winner = "player";
            winScore = playerScore - AIScore;
        }
        else 
        {
            winner = "draw";
            winScore = 0;
        }
        Match.instance.UpdateMatchResult(winner, winScore);

        //Call UpdateMatchResult in Match.cs
    }

}
