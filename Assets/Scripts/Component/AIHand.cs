using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHand : PlayerHand
{
    public new static AIHand instance;
    private Dictionary<Card, CardSlot> KnownCards = new Dictionary<Card, CardSlot>();
    private HashSet<Card> DesiredCards = new HashSet<Card>();
    
    //Matrix represents cards and their locations
    //[4] : Suits. uses CardSuit.cs 
    //[13]: Face Value; 0 = Ace, 11 = Jack, 12 = Queen, 13 = King
    //Values: -1 = unknown, 0 = Player Hand, 1 = AI Hand, 3 Discard Stack
    //Will also be used for AI simulations
    private int[,] KnownCardSet = new int[4, 13]; 

    private float SimulationTimer;
    private int[,] SimulationRunsAndWins = new int[11,2];


    public new void Awake()
    {
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

    //TODO: Update KnownCardSet[,]
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
        //right now it's choosing the draw pile
        //TODO: write rules
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
    private Card DecideCardToDiscard()
    {
        System.Random rnd = new System.Random();
        CardSlot slot;
        do
        {
            int randomCardIndex = rnd.Next(0, 10); // creates a number between 0 and 10
            slot = CardSlotList[randomCardIndex];
        } while (slot.FaceValue() == 0);

        return slot.TopCard();

    }

    public void AIExecuteTurn()
    {
        Invoke("AIDraw", 1f);
    }

    private void AIDraw()
    {
        DrawCard(DecidePileToDraw().TopCard());
        Round.instance.UpdateTurn(Turn.AIDiscard);
        Invoke("AIDiscard", 1f);
    }

    private void AIDiscard()
    {
        DiscardCard(DecideCardToDiscard());
    }

}
