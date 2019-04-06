using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Player Hand:
- instance
- deadwoods: Array<card>
- deadwoodPoints: int
- runs: Array<Array<card>>
- sets: Array<Array<card>>
--
+ ScanHand
+ AddCard
+ RemoveCard
+ Gin
+ Knock

*/

public class PlayerHand : MonoBehaviour
{
    public static PlayerHand instance;
    public List<Card> Deadwoods;
    public int DeadwoodPoints;
    public List<List<Card>> Runs = new List<List<Card>>();
    public List<List<Card>> Sets = new List<List<Card>>();
    public List<Card> Cards;

    public CardSlot CardSlot0;
    public CardSlot CardSlot1;
    public CardSlot CardSlot2;
    public CardSlot CardSlot3;
    public CardSlot CardSlot4;
    public CardSlot CardSlot5;
    public CardSlot CardSlot6;
    public CardSlot CardSlot7;
    public CardSlot CardSlot8;
    public CardSlot CardSlot9;
    public CardSlot CardSlot10;

    public readonly List<CardSlot> CardSlotList = new List<CardSlot>();

    public void Awake()
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

    public void InitializeHand()
    {
        foreach (CardSlot slot in CardSlotList)
        {
            Cards.Add(slot.TopCard());
        }
        ScanHand();
    }

    //Scan the hand to determine what are deadwoods, runs, sets and other data
    public void ScanHand()
    {

    }

    public void DrawCard(Card newCard)
    {
        //Move card from their old slot to their new slot in player's hand
        foreach (CardSlot slot in CardSlotList)
        {
            if (slot.FaceValue() == 0)
            {
                slot.AddCard(newCard);
            }
        }

        //Add to Cards list then call Scan hand to put the new card
        //in appropriate set, run or deadwood
        Cards.Add(newCard);
        ScanHand();
        if (Round.instance.CurrentTurn == Turn.PlayerDraw)
        {
            Round.instance.UpdateTurn(Turn.PlayerDiscard);
        }
    }

    public void DiscardCard(Card card)
    {
        Round.instance.DiscardPile.AddCard(card);

        Cards.Remove(card);
        Deadwoods.Remove(card);
        foreach (List<Card> l in Sets)
        {
            l.Remove(card);
        }
        foreach (List<Card> l in Runs)
        {
            l.Remove(card);
        }
        if (Round.instance.CurrentTurn == Turn.PlayerDiscard)
        {
            Round.instance.UpdateTurn(Turn.AI);
            AIHand.instance.AIExecuteTurn();
        }
        else
        {
            Round.instance.UpdateTurn(Turn.PlayerDraw);
        }
    }

    public void Gin()
    {
        //check eligibility
        //then call update round score
    }

    public void Knock()
    {
        //check eligibility
        //then call update round score
    }
}