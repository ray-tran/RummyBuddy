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

        PlayerHand.instance.InitializeHand();
        AIHand.instance.InitializeHand();
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
        int playerDeadwoodPoints = PlayerHand.instance.DeadwoodPoints;
        int AIDeadwoodPoints = AIHand.instance.DeadwoodPoints;
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
