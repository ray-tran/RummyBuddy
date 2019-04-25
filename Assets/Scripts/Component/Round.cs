using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Redo Turns based on AIDraw and AIDiscard
public enum Turn { PlayerDraw, PlayerDiscard, AIDraw, AIDiscard};

//VARIABLES AND CLASS NAMES ARE AMBIGUOUS "AIHand PlayerHand"
public class Round : MonoBehaviour
{
    public static Round instance;
    public Turn CurrentTurn;
    public CardSlot DiscardPile;
    public CardSlot DrawPile;
    private PlayerHand PlayerHand;
    private AIHand AIHand;
    public string winner;
    public int winScore;

    //Matrix represents cards and their locations
    //[4] : Suits. Suit { Clubs, Diamonds, Hearts, Spades };
    //[13]: Face Value; 0 = Ace, 11 = Jack, 12 = Queen, 13 = King
    //Values: -1 = unknown, 0 = Player Hand, 1 = AI Hand, 2 = Discard Stack
    //Will also be used for AI simulations
    //KnownGameState
    public int[,] _CurrentGameState = new int[4, 13];


    void Awake()
    {
        instance = this;
        PlayerHand = PlayerHand.instance;
        AIHand = AIHand.instance;
    }

    public void InitializeRound()
    {
        PlayerHand.instance.InitializeHand();
        AIHand.instance.InitializeHand();
        AIHand.instance.InitializeGameState();

        if (winner == "AI")
        {
            UpdateTurn(Turn.PlayerDraw);
        }
        else if (winner == "Player")
        {
            UpdateTurn(Turn.AIDraw);
        }
    }

    public void UpdateTurn(Turn newTurn)
    {
        CurrentTurn = newTurn;
        GameUI.instance.currentTurnText.text = "Current turn: " + newTurn.ToString();
        GameUI.instance.UpdateScoreUI();
    }

    public void CalculateAndUpdateScore()
    {
        int playerDeadwoodPoints = PlayerHand.instance.DeadwoodPoints;
        int AIDeadwoodPoints = AIHand.instance.DeadwoodPoints;
        if (playerDeadwoodPoints < AIDeadwoodPoints)
        {
            winner = "ai";
            winScore = AIDeadwoodPoints - playerDeadwoodPoints;
        }
        else if (playerDeadwoodPoints > AIDeadwoodPoints)
        {
            winner = "player";
            winScore = playerDeadwoodPoints - AIDeadwoodPoints;
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
