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
    protected PlayerHand PlayerHand;
    protected AIHand AIHand;
    public string winner;
    public int winScore;

    //Matrix represents cards and their locations
    //[4] : Suits. Suit { Clubs, Diamonds, Hearts, Spades };
    //[13]: Face Value; 0 = Ace, 11 = Jack, 12 = Queen, 13 = King
    //Values: -1 = unknown, 0 = Player Hand, 1 = AI Hand, 2 = Discard Stack
    //Will also be used for AI simulations
    //KnownGameState
    public int[,] _CurrentGameState = new int[4, 13];


    public void Awake()
    {
        instance = this;
        PlayerHand = GameObject.Find("PlayerHand").GetComponent<PlayerHand>();
        AIHand = GameObject.Find("AIHand").GetComponent<AIHand>();
    }

    public void InitializeRound()
    {
        // Since AI logic hasn't been applied, commenting this part out
        /*
        //creates a random value
        System.Random binaryrand = new System.Random(2);
        //Makes that random value generate between 1 and 0, this in turn simulates a coin being flipped for the first turn
        int coinFlip = binaryrand.Next(2);
        if (coinFlip == 0)
        {
            UpdateTurn(Turn.PlayerDraw);
        }
        else UpdateTurn(Turn.AI);
        */

        UpdateTurn(Turn.PlayerDraw);

        GameObject.Find("PlayerHand").GetComponent<PlayerHand>().InitializeHand();
        GameObject.Find("AIHand").GetComponent<AIHand>().InitializeHand();
        GameObject.Find("AIHand").GetComponent<AIHand>().InitializeGameState();

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

    //callType:
    //-1: undercut
    //0: knock
    //1: gin
    //2: big gin
    //InstanceType (who called gin/knock):
    //0: playerHand
    //1: AIHand
    public void CalculateAndUpdateScore(int callType, int InstanceType)
    {
        int playerDeadwoodPoints = GameObject.Find("PlayerHand").GetComponent<PlayerHand>().DeadwoodPoints;
        int AIDeadwoodPoints = GameObject.Find("AIHand").GetComponent<AIHand>().DeadwoodPoints;
        string winner;
        int winScore;

        //Knocking
        if (callType == 0)
        {
            //Player calling knock
            if (InstanceType == 0)
            {
                if (playerDeadwoodPoints < AIDeadwoodPoints)
                {
                    winner = "player";
                    winScore = AIDeadwoodPoints - playerDeadwoodPoints;
                }
                //Undercut
                else
                {
                    winner = "ai";
                    winScore = playerDeadwoodPoints - AIDeadwoodPoints + 25;
                    callType = -1;
                }
            }
            //AI calling knocking
            else
            {
                if (AIDeadwoodPoints < playerDeadwoodPoints)
                {
                    winner = "ai";
                    winScore = playerDeadwoodPoints - AIDeadwoodPoints;
                }
                //Undercut
                else
                {
                    winner = "player";
                    winScore = AIDeadwoodPoints - playerDeadwoodPoints + 25;
                    callType = -1;
                }
            }
        }

        //Gin-ing
        else if (callType == 1)
        {
            //Player calling gin
            if (InstanceType == 0)
            {
                winner = "player";
                winScore = AIDeadwoodPoints - playerDeadwoodPoints + 25;
            }
            //AI calling gin
            else
            {
                winner = "ai";
                winScore = playerDeadwoodPoints - AIDeadwoodPoints + 25;
            }
        }

        //Big gin-ing
        else
        {
            //Player calling big gin
            if (InstanceType == 0)
            {
                winner = "player";
                winScore = AIDeadwoodPoints - playerDeadwoodPoints + 31;
            }
            //AI calling big gin
            else
            {
                winner = "ai";
                winScore = playerDeadwoodPoints - AIDeadwoodPoints + 31;
            }
        }

        Match.instance.UpdateMatchResult(winner, winScore, callType);

    }

}
