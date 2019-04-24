using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHand : PlayerHand
{
    public new static AIHand instance;
    private Dictionary<Card, CardSlot> KnownCards = new Dictionary<Card, CardSlot>();
    //private HashSet<Card> DesiredCards = new HashSet<Card>();

    private float SimulationTimer;
    private int[,] SimulationRunsAndWins = new int[11,2];

    //Matrix represents cards and their locations
    //[4] : Suits. Suit { Clubs, Diamonds, Hearts, Spades };
    //[13]: Face Value; 0 = Ace, 11 = Jack, 12 = Queen, 13 = King
    //Values: -1 = unknown, 0 = Player Hand, 1 = AI Hand, 2 = Discard Stack
    //Will also be used for AI simulations
    //KnownGameState
    public int[,] KnownGameState = new int[4, 13];

    public new void Awake()
    {
        MatrixValue = 1;
        instance = this;
        CardSlotList.Add(CardSlot0);
        CardSlotList.Add(CardSlot1);
        CardSlotList.Add(CardSlot2);
        CardSlotList.Add(CardSlot3);
        CardSlotList.Add(CardSlot4);
        CardSlotList.Add(CardSlot5);
        CardSlotList.Add(CardSlot6);
        CardSlotList.Add(CardSlot7);
        CardSlotList.Add(CardSlot8);
        CardSlotList.Add(CardSlot9);
        CardSlotList.Add(CardSlot10);
    }

    //TODO: Update CurrentGameState[,]
    //Possibly remove dictionary based on KnownCards
    public void AddKnownCard(Card card, CardSlot slot)
    {
        KnownCards.Add(card, slot);
    }

    private CardSlot DecidePileToDraw()
    {
        Card topDiscardCard = Round.instance.DiscardPile.TopCard();

        printGameState();
        
        //If drawing top discard card would result in 3 cards or more in a run
        //Or if drawing top discard card would result in 3 cards or more of same rank  
        if (GetContRunCount(topDiscardCard, 1) >= 3 || GetColumnValCount(topDiscardCard, 1) >= 3)
        {
            //Temporary add card to AIHand without UI display to calculate Deadwoods
            int origDeadwoodPoints = DeadwoodPoints;

            Debug.Log("origDeadwoodPoints: " + origDeadwoodPoints);

            //false flag to disable UI drawing (this is happening in the background
            DrawCard(topDiscardCard, false);
            int testingDeadwoodPoints = DeadwoodPoints;

            Debug.Log("testingDeadwoodPoints: " + testingDeadwoodPoints);

            DiscardCard(topDiscardCard, false);

            if (testingDeadwoodPoints <= origDeadwoodPoints)
                return Round.instance.DiscardPile;
        }


        // else
        //choosing the draw pile
        if ((Round.instance.DrawPile.TopCard()) != null)
        {
            return Round.instance.DrawPile;
        }
        
        return null;
    }

    //TODO: call monte carlo here
    //right now it's choosing a random card to discard

    //WILL: Create 11x2 array based on card slots, 0 = Sims, 1 = Wins
    //Send current known cards to AI sim
    //Evaluate based on simulations
    //Add runs and wins to slot
    //Repeat based on stats and algotithm
    //Return Card with most simulations
    private System.Random rnd = new System.Random();
    private Card DecideCardToDiscard()
    {
        if (Deadwoods.Count < 11) {



            //Get set of cards not in melds
            //Choose card from that set by UCT
            //AISimulationState() constructor
            //GetSimulation
            //Recording
            //Loop until you done
            //Get one with most sism

            //TEMPORARY STRATEGY//
            //The last card in the sorted hand is the highest value deadwood
            return CardSlotList[10].TopCard();
        }

        return null;
    }

    public void InitializeGameState()
    {
        FillMatrix(KnownGameState, -1);

        foreach (Card card in CardsInHand)
        {
            PutCardInGameState(card, 1);
        }

        PutCardInGameState(Round.instance.DiscardPile.TopCard(), 2);
        printGameState();
    }

    public void PutCardInGameState(Card card, int val)
    {
        //Debug.Log("Putting " + card.name + " in game state");

        int row = (int)card.CardSuit;
        int col = card.Rank - 1;
        KnownGameState[row, col] = val;
    }

    //Including the card we're testing on
    public int GetColumnValCount(Card card, int val)
    {
        int col = card.Rank - 1;
        int count = 1;

        for (int i = 0; i < 4; i++)
        {
            if (KnownGameState[i, col] == val)
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
            if (KnownGameState[row, left] == val)
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
            if (KnownGameState[row, right] == val)
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
        int rowCount = KnownGameState.GetLength(0);
        int colCount = KnownGameState.GetLength(1);

        Debug.Log("X 1 2 3 4 5 6 7 8 9 T J Q K");

        for (int row = 0; row < rowCount; row++)
        {
            string line = "";

            switch (row)
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
                if (KnownGameState[row, col] == -1) line += "U ";
                else
                {
                    line += KnownGameState[row, col].ToString() + " ";
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

    public void AIExecuteTurn()
    {
        Invoke("AIDraw", 1f);
    }

    private void AIDraw()
    {
        DrawCard(DecidePileToDraw().TopCard(), true);

        //If gin or big gin then end round?

        Round.instance.UpdateTurn(Turn.AIDiscard);
        Invoke("AIDiscard", 1f);
    }

    private void AIDiscard()
    {
        DiscardCard(DecideCardToDiscard(), true);
    }

}
