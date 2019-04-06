using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerHand : MonoBehaviour
{
    public static PlayerHand instance;

    public List<Card> Deadwoods;
    public int DeadwoodPoints;

    public List<List<Card>> AllMelds;
    public List<List<Card>> OptimalMelds;

    public List<Card> SpadesList;
    public List<Card> ClubsList;
    public List<Card> DiamondsList;
    public List<Card> HeartsList;

    public List<Card> CardsInHand;

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

    //Called at start of round
    public void InitializeHand()
    {
        CardsInHand = new List<Card>();
        SpadesList = new List<Card>();
        ClubsList = new List<Card>();
        DiamondsList = new List<Card>();
        HeartsList= new List<Card>();

        foreach (CardSlot slot in CardSlotList)
        {
            if (slot.TopCard() != null)
            {
                Card card = slot.TopCard();
                CardsInHand.Add(card);
                AddToSuitList(card);
            }
        }

        ScanHand();
    }

    //Scan the hand to determine what are deadwoods, runs, sets and other data
    public void ScanHand()
    {
        AllMelds = new List<List<Card>>();
        OptimalMelds = new List<List<Card>>();
        SortCardList(CardsInHand);

        ScanForSets();
        ScanForRuns(SpadesList);
        ScanForRuns(ClubsList);
        ScanForRuns(DiamondsList);
        ScanForRuns(HeartsList);
        Log2DList(AllMelds); //Log to console

        DecideOptimalMelds(); //TODO

        SortHandUI(); //TODO
    }

    //Function that scan the hand and put any possible sets
    //(3+ of the same rank) into AllMelds list
    private void ScanForSets()
    {
        List<Card> set = new List<Card>();
        int curSetRank = CardsInHand[0].Rank;
        int curSetCount = 1;
        int j = 0;
        int i = 1;
        for (; i< CardsInHand.Count; i++)
        {
            if (CardsInHand[i].Rank == curSetRank)
            {
                curSetCount++;
            }
            else
            {
                if (curSetCount > 2)
                {
                    while (j < i)
                    {
                        set.Add(CardsInHand[j]);
                        j++;
                    }
                    AllMelds.Add(set);
                    set = new List<Card>();
                }
                curSetCount = 1;
                curSetRank = CardsInHand[i].Rank;
                while (j < i)
                {
                    j++;
                }
            }
        }
        if (curSetCount > 2)
        {
            while (j < i)
            {
                set.Add(CardsInHand[j]);
                j++;
            }
            AllMelds.Add(set);
        }
    }

    //Function that scan the hand and put any possible runs
    //(sequence of 3+ card of same suit) into AllMelds list
    private void ScanForRuns(List<Card> suitList)
    {
        //No need to scan if the suit list only contains 2 cards (have to have 3 minimum for a run)
        if (suitList.Count > 2)
        {
            List<Card> run = new List<Card>();
            int prevCardRank = suitList[0].Rank;
            int curRunCount = 1;
            int j = 0;
            int i = 1;
            for (; i < suitList.Count; i++)
            {
                if (suitList[i].Rank == prevCardRank + 1)
                {
                    curRunCount++;
                    prevCardRank = suitList[i].Rank;
                }
                else
                {
                    if (curRunCount > 2)
                    {
                        while (j < i)
                        {
                            run.Add(suitList[j]);
                            j++;
                        }
                        AllMelds.Add(run);
                        run = new List<Card>();
                    }
                    curRunCount = 1;
                    prevCardRank = suitList[i].Rank;
                    while (j < i)
                    {
                        j++;
                    }
                }
            }
            if (curRunCount > 2)
            {
                while (j < i)
                {
                    run.Add(suitList[j]);
                    j++;
                }
                AllMelds.Add(run);
            }
        }
    }

    //TODO
    //Function to use the melds in AllMelds and decide the optimal subset of AllMelds:
    //meaning no overlap in cards (each card appear only once) 
    //and have the highest total face value (consequentially lowest total deadwood value)
    private void DecideOptimalMelds()
    {

    }

    //TODO
    private void CalculateDeadwoods()
    {
        //CardsInHand - OptimalMelds = Deadwoods
        //Update Deadwoods list and DeadwoodPoints
    }

    private void SortCardList(List<Card> cardList)
    {
        cardList.Sort((a, b) =>
        {
            int firstCompare = a.Rank.CompareTo(b.Rank);
            return firstCompare != 0 ? firstCompare : a.CardSuit.CompareTo(b.CardSuit);
        });
    }

    //Function to display the hand in the UI according to the order of CardsInHand list
    //TODO: modify so that it displays the optimal melds on the left most slots, then sorted deadwoods
    //on the remaining slots
    private void SortHandUI()
    {
        for (int i = 0; i < CardsInHand.Count; i++)
        {
            CardSlotList[i].AddCard(CardsInHand[i]);
        }
    }

    public void DrawCard(Card newCard)
    {
        //Add to Cards list then call Scan hand to put the new card
        //in appropriate set, run or deadwood
        CardsInHand.Add(newCard);
        AddToSuitList(newCard);

        ScanHand();

        if (Round.instance.CurrentTurn == Turn.PlayerDraw)
        {
            Round.instance.UpdateTurn(Turn.PlayerDiscard);
        }
    }

    public void DiscardCard(Card card)
    {
        Round.instance.DiscardPile.AddCard(card);

        CardsInHand.Remove(card);
        Deadwoods.Remove(card);
        SpadesList.Remove(card);
        ClubsList.Remove(card);
        DiamondsList.Remove(card);
        HeartsList.Remove(card);
        foreach (List<Card> l in AllMelds)
        {
            l.Remove(card);
        }
        foreach (List<Card> l in OptimalMelds)
        {
            l.Remove(card);
        }

        ScanHand();

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

    private void AddToSuitList(Card card)
    {
        switch (card.CardSuit)
        {
            case Suit.Clubs:
                ClubsList.Add(card);
                SortCardList(ClubsList);
                break;
            case Suit.Diamonds:
                DiamondsList.Add(card);
                SortCardList(DiamondsList);
                break;
            case Suit.Hearts:
                HeartsList.Add(card);
                SortCardList(HeartsList);
                break;
            case Suit.Spades:
                SpadesList.Add(card);
                SortCardList(SpadesList);
                break;
        }

    }

    public void Gin()
    {
        //check eligibility
        //then call CalculateAndUpdateScore in Round.cs
    }

    public void Knock()
    {
        //check eligibility
        //then call CalculateAndUpdateScore in Round.cs
    }

    private void Log2DList(List<List<Card>> list)
    {
        Debug.Log("============");
        string line = "";
        foreach (List<Card> inner in list)
        {
            line = ""; 
            foreach (Card card in inner)
            {
                line += card.name + " ";
            }
            Debug.Log(line);
        }
    }
}