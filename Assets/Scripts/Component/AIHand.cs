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


    //TODO: Search through current hand, possibly easier through matrix
    //with matrix, find cards next to eachother in rows or columns
    //all cards next to 2 or more possible runs or next to possible sets
    //those inbetween to cards Ex. [...1,0,1...] The 0 would be desired
    public void UpdateDesiredCards()
    {

    }

    private CardSlot DecidePileToDraw()
    {
        Card topDiscardCard = Round.instance.DiscardPile.TopCard();

        Round.instance.printGameState();

        Debug.Log("Top discard: " + topDiscardCard.name);
        Debug.Log("GetContRunCount: " + Round.instance.GetContRunCount(topDiscardCard, 1));
        Debug.Log("GetColumnValCount: " + Round.instance.GetColumnValCount(topDiscardCard, 1));

        //If drawing top discard card would result in 3 cards or more in a run
        //Or if drawing top discard card would result in 3 cards or more of same rank  
        if (Round.instance.GetContRunCount(topDiscardCard, 1) >= 3 || Round.instance.GetColumnValCount(topDiscardCard, 1) >= 3)
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

    //WILL: Create 11x2 array based on card slots, 0 = Runs, 1 = Wins
    //Send current known cards to AI sim
    //Evaluate based on simulations
    //Add runs and wins to slot
    //Repeat based on stats and algotithm
    //Return Card with most simulations
    private System.Random rnd = new System.Random();
    private Card DecideCardToDiscard()
    {
        if (Deadwoods.Count < 11) {

            //TEMPORARY STRATEGY//
            //The last card in the sorted hand is the highest value deadwood
            return CardSlotList[10].TopCard();
        }

        return null;
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
