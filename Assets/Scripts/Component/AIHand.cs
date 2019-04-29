using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHand : PlayerHand
{
    public new static AIHand instance;
    private Dictionary<Card, CardSlot> KnownCards = new Dictionary<Card, CardSlot>();
    private HashSet<Card> DesiredCards = new HashSet<Card>();

    public new void Awake()
    {
        InstanceType = 1;
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

    public void AddKnownCard(Card card, CardSlot slot)
    {
        KnownCards.Add(card, slot);
    }

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
        Invoke("AIDiscard", 1f);
    }

    private void AIDiscard()
    {
        DiscardCard(DecideCardToDiscard());
    }

}
