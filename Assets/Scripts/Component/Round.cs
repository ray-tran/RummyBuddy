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

    //Matrix represents cards and their locations
    //[4] : Suits. Suit { Clubs, Diamonds, Hearts, Spades };
    //[13]: Face Value; 0 = Ace, 11 = Jack, 12 = Queen, 13 = King
    //Values: -1 = unknown, 0 = Player Hand, 1 = AI Hand, 2 = Discard Stack
    //Will also be used for AI simulations
    public int[,] CurrentGameState = new int[4, 13];


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
        InitializeGameState();
    }

    private void InitializeGameState()
    {
        FillMatrix(CurrentGameState, -1);

        foreach (Card card in PlayerHand.instance.CardsInHand)
        {
            PutCardInGameState(card, 0);
        }

        foreach (Card card in AIHand.instance.CardsInHand)
        {
            PutCardInGameState(card, 1);
        }

        PutCardInGameState(DiscardPile.TopCard(), 2);
        printGameState();
    }

    public void PutCardInGameState(Card card, int val)
    {
        //Debug.Log("Putting " + card.name + " in game state");

        int row = (int)card.CardSuit;
        int col = card.Rank - 1;
        CurrentGameState[row, col] = val;
    }

    //Including the card we're testing on
    public int GetColumnValCount(Card card, int val)
    {
        int col = card.Rank - 1;
        int count = 1;

        for (int i = 0; i < 4; i++)
        {
            if (CurrentGameState[i, col] == val)
                count++;
        }

        return count;
    }

    //Including the card we're testing on
    public int GetContRunCount(Card card, int val)
    {
        int count = 1;
        int row = (int)card.CardSuit;
        int col = card.Rank - 1;
        int left = col - 1;
        while (left >= 0)
        {
            if (CurrentGameState[row,left] == val)
            {
                count++;
            }
            else
            {
                break;
            }
            left--;
        }

        int right = col + 1;
        while (right < 13)
        {
            if (CurrentGameState[row, right] == val)
            {
                count++;
            }
            else
            {
                break;
            }
            right++;
        }

        return count;
    }

    public void printGameState()
    {
        int rowCount = CurrentGameState.GetLength(0);
        int colCount = CurrentGameState.GetLength(1);

        Debug.Log("X 1 2 3 4 5 6 7 8 9 T J Q K");

        for (int row = 0; row < rowCount; row++)
        {
            string line = "";

            switch(row)
            {
                case 0:
                    line += "C ";
                    break;
                case 1:
                    line += "D ";
                    break;
                case 2:
                    line += "H ";
                    break;
                case 3:
                    line += "S ";
                    break;
            }

            for (int col = 0; col < colCount; col++)
            {
                if (CurrentGameState[row, col] == -1) line += "U ";
                else
                {
                    line += CurrentGameState[row, col].ToString() + " ";
                }
            }
            Debug.Log(line);
        }
        Debug.Log("---");
    }

    private void FillMatrix(int[,] matrix, int val)
    {
        int rowCount = matrix.GetLength(0);
        int colCount = matrix.GetLength(1);

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                matrix[row, col] = val;            
            }
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
        string winner;
        int winScore;
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
