//She's a little rough, but this can get basic implementation started

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Only for AIHand class to see
//Honestly it might not matter tho, it will generate per run sooooo
public class AISimulationState: Round
{
    private bool AIWin = false;
    //Same as AI Hand KnownCardSet
    private int[,] GameState = new int[4, 13];
    //Used for evaluated hands in both Calculate functions
    private int[] AIHandEvaluation = new int[11];
    private int[] PlayerHandEvaluation = new int[11];

    //Values: 0 = Player Hand, 1 = AI Hand, 2 = Discard Stack, 3 = Draw Stack
    private enum CardLocations {PlayerHand, AIHand, DiscardPile, DrawPile};
    private System.Random rand = new System.Random();
    private int SimulationTurn;
    private Card ChosenCard;
    private PlayerHand PlayerHand;
    private AIHand AIHand;
    

    //On construction, GameStateSet will be generated
    //
    public AISimulationState(Card SimDiscardedCard, int[,] simGameState)
    {
        ChosenCard = SimDiscardedCard;
        GameState = simGameState;
        CurrentTurn = Round.instance.CurrentTurn; //since this inherits from Round I think it already has this
        DiscardPile = Round.instance.DiscardPile; //this too
        FillCards();
        SimulationTurn = 1;
    }

    //Finds all -1 spots, and fills out rest of cards based on empty spaces
    //Will track maximum PlayerHand cards
    //Will generate Stock Stack based on random filled cards
    //Also fills out PlayerHand and AIHand based on GameStateSet
    private void FillCards()
    {
        //Create DeckSize to ensure accurate simulation
        int UnknownDeckSize = 31 - this.DiscardPile.CardList.Count;
        int PlayerHandSize = 10; 
        int PlayerHandKnownSize = 0;
        List<(int, int)> UnknownSpots = new List<(int, int)>();
        int RandomSpot = 0;
        

        //Fill in for stock pile
        //Might move to AIHand?
        /*
        for (int i = this.DiscardPile.CardList.Count; i > 0; i--)
        {
            GameState[(int) this.DiscardPile.CardList[i].CardSuit, this.DiscardPile.CardList[i].Rank - 1] = (int) CardLocations.DiscardPile;
        }\
        */

        //Find all known cards in playerHand, then fill them
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                switch (GameState[i,j])
                {
                    case (int) CardLocations.PlayerHand:
                        PlayerHandKnownSize++;
                        Card newCard = this.GenerateCard(i,j);
                        PlayerHand.DrawCard(newCard,false);
                        UnknownDeckSize--;
                        break;
                    case -1:
                        //save locations so they don't have to reiterate the array
                        UnknownSpots.Add((i,j));
                        break;
                }
            }
        }

        //Fill rest of PlayerHand based on unknowns
        //Choose Random cards To fill PlayerHand
        //Create Card and add to PlayerHand
        //Remove card from unknown spot list
        //Increment KnownPlayerHandsize
        while (PlayerHandKnownSize < PlayerHandSize)
        {
            RandomSpot = rand.Next(0, UnknownSpots.Count - 1);
            GameState[UnknownSpots[RandomSpot].Item1,UnknownSpots[RandomSpot].Item2] = (int) CardLocations.PlayerHand;
            PlayerHand.CardsInHand.Add(this.GenerateCard(UnknownSpots[RandomSpot].Item1, UnknownSpots[RandomSpot].Item2));
            UnknownSpots.RemoveAt(RandomSpot);
            PlayerHandKnownSize++;
        }

        //Fill Stock Pile based on deckSize and unfilled cards
        //Create Stack from pile
        //Add to Gamestate as CardLocation.StockStack
        //Generate card and add to DrawPile
        //
        while (UnknownSpots.Count > 0)
        {
            RandomSpot = rand.Next(0, UnknownSpots.Count - 1);
            GameState[UnknownSpots[RandomSpot].Item1,UnknownSpots[RandomSpot].Item2] = (int) CardLocations.DrawPile;
            UnknownSpots.RemoveAt(RandomSpot);

            Card DrawCard = GenerateCard(UnknownSpots[RandomSpot].Item1,UnknownSpots[RandomSpot].Item2);
            this.DrawPile.AddCard(DrawCard);
        }


    }

    //Determines current turn, runs calculation method based on whos turn and action
    //Increments SimulationTurn and CurrentTurn, updates GameStateSet

    //Currently does not account for knocks or gins, just til cards run out of drawpile
    private void SimulateTurnMove()
    {
        bool isSimulating = true;
        while (isSimulating)
        {
            if (CurrentTurn.Equals(Turn.PlayerDiscard)||CurrentTurn.Equals(Turn.AIDiscard))
            {
                CalculateDrawCard();
            }
            else
            {
                CalculateDiscardCard();

            }
            isSimulating = CalculateIsSim();
        }
    }

    //TODO: Determines which card to draw, based on whos turn, their hand, and "Best guess" of desired cards
    //Updates GameState
    private void CalculateDrawCard()
    {
        // 0 being Draw Pile
        // 1 being Discard Pile
        int RandomPile = rand.Next(0,1);

        switch (CurrentTurn)
        {
            case Turn.PlayerDraw:
                CurrentTurn = Turn.PlayerDiscard;
                if (RandomPile == 0)
                {
                    (int,int) GameStateCard = CardToStateLocation(DrawPile.TopCard());
                    GameState[GameStateCard.Item1,GameStateCard.Item2] = (int) CardLocations.PlayerHand;
                    this.PlayerHand.DrawCard(this.DrawPile.TopCard(), false);

                }
                else
                {
                    (int,int) GameStateCard = CardToStateLocation(DiscardPile.TopCard());
                    GameState[GameStateCard.Item1,GameStateCard.Item2] = (int) CardLocations.PlayerHand;
                    this.PlayerHand.DrawCard(this.DiscardPile.TopCard(), false);
                }
                break;
            case Turn.AIDraw:
                CurrentTurn = Turn.AIDiscard;
                if (RandomPile == 0)
                {
                    (int,int) GameStateCard = CardToStateLocation(DrawPile.TopCard());
                    GameState[GameStateCard.Item1,GameStateCard.Item2] = (int) CardLocations.AIHand;
                    this.AIHand.DrawCard(this.DrawPile.TopCard(), false);
                }
                else
                {
                    (int,int) GameStateCard = CardToStateLocation(DiscardPile.TopCard());
                    GameState[GameStateCard.Item1,GameStateCard.Item2] = (int) CardLocations.AIHand;
                    this.AIHand.DrawCard(this.DiscardPile.TopCard(), false);
                }
                break;
        }

        
    } 

    //Determines which card to discard, based on whos turn, their hand, and calculated and "weighted" cards
    //Evaluates hand, makes best intentioned guess
    //
    //right now chooses random card, updates GameState
    private void CalculateDiscardCard()
    {
        CardLocations currentHand = CardLocations.PlayerHand;// Only initialized to stop errors
        switch (CurrentTurn)
        {
            case Turn.PlayerDiscard:
                currentHand = CardLocations.PlayerHand;
                break;
            case Turn.AIDiscard:
                currentHand = CardLocations.AIHand;
                break;
        }

        List<(int, int)> Spots = new List<(int, int)>();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                if (GameState[i,j] == (int) currentHand)
                {
                    Spots.Add((i,j));
                }
            }
        }

        int RandomSpot = rand.Next(0, Spots.Count - 1);

        //might need edits, potention for lotta garbage (which c# cleans out anyway)
        Card NewCard = GenerateCard(Spots[RandomSpot].Item1,Spots[RandomSpot].Item2);
            
        if (currentHand == CardLocations.PlayerHand)
        {
            this.PlayerHand.DiscardCard(NewCard, false, false);
        }
        else 
        {
            this.AIHand.DiscardCard(NewCard, false, false);
        }

        GameState[Spots[RandomSpot].Item1,Spots[RandomSpot].Item2] = (int) CardLocations.DiscardPile;
    }

    private Card  GenerateCard(int suit, int face)
    {
        GameObject emptyCardObject = new GameObject();
        emptyCardObject.AddComponent<Card>();
        Card ReturnCard = emptyCardObject.GetComponent<Card>();
        switch (suit)
            {
                case (int) Card.Suit.Clubs:
                    ReturnCard.CardSuit = Card.Suit.Clubs;
                    break;

                case (int) Card.Suit.Diamonds:
                    ReturnCard.CardSuit = Card.Suit.Diamonds;
                    break;

                case (int) Card.Suit.Hearts:
                    ReturnCard.CardSuit = Card.Suit.Hearts;
                    break;

                case (int) Card.Suit.Spades:
                    ReturnCard.CardSuit = Card.Suit.Spades;
                    break;
            }

            switch(face)
            {
                case 0: // Ace
                    ReturnCard.FaceValue = 1;
                    ReturnCard.Rank = 1;
                    break;
                case 1: //2
                    ReturnCard.FaceValue = 2;
                    ReturnCard.Rank = 2;
                    break;
                case 2: //3
                    ReturnCard.FaceValue = 3;
                    ReturnCard.Rank = 3;
                    break;
                case 3: //4
                    ReturnCard.FaceValue = 4;
                    ReturnCard.Rank = 4;
                    break;
                case 4: //5
                    ReturnCard.FaceValue = 5;
                    ReturnCard.Rank = 5;
                    break;
                case 5: //6
                    ReturnCard.FaceValue = 6;
                    ReturnCard.Rank = 6;
                    break;
                case 6: //7 
                    ReturnCard.FaceValue = 7;
                    ReturnCard.Rank = 7;
                    break;
                case 7: //8
                    ReturnCard.FaceValue = 8;
                    ReturnCard.Rank = 8;
                    break;
                case 8: //9
                    ReturnCard.FaceValue = 9;
                    ReturnCard.Rank = 9;
                    break;
                case 9: //10
                    ReturnCard.FaceValue = 10;
                    ReturnCard.Rank = 10;
                    break;
                case 10: //Jack
                    ReturnCard.FaceValue = 11;
                    ReturnCard.Rank = 10;
                    break;
                case 11: //Queen
                    ReturnCard.FaceValue = 12;
                    ReturnCard.Rank = 10;
                    break;
                case 12: //King 
                    ReturnCard.FaceValue = 13;
                    ReturnCard.Rank = 10;
                    break;
            }
        return ReturnCard;
    }

    //Takes a card, turns it into a GameState location tuple
    private (int,int) CardToStateLocation(Card card)
    {
        (int,int) cardLocation = (-1,-1);
        cardLocation.Item1 = (int) card.CardSuit;
        cardLocation.Item2 = (int) card.FaceValue - 1;
        return cardLocation;
    }
    

    private bool CalculateIsSim()
    {
        this.AIHand.ScanHand(false);
        this.PlayerHand.ScanHand(false);

        if (CurrentTurn == Turn.AIDiscard || CurrentTurn ==  Turn.AIDraw)
        {
            if (AIHand.DeadwoodPoints < 10)
            {
                if (AIHand.DeadwoodPoints == 0 || AIHand.Deadwoods.Count == 1)
                {
                    AIWin = true;
                    return false;
                }

                //0 knocks
                //1 doesn't
                if (rand.Next(0,1) == 0)
                {
                    if (AIHand.DeadwoodPoints < PlayerHand.DeadwoodPoints)
                    {
                        AIWin = true;
                        return false;
                    }
                    else 
                    {
                        return false;
                    }
                }
            
            }
        }
        else if (PlayerHand.DeadwoodPoints < 10)
        {
            if (PlayerHand.DeadwoodPoints == 0 || PlayerHand.Deadwoods.Count == 1)
                {
                    return false;
                }

                //0 knocks
                //1 doesn't
                if (rand.Next(0,1) == 0)
                {
                    if (AIHand.DeadwoodPoints < PlayerHand.DeadwoodPoints)
                    {
                        AIWin = true;
                        return false;
                    }
                    else 
                    {
                        return false;
                    }
                }
        }
        return true;
    }

    //Starts simulation, returns if simulation got to "win" or "advantage" state
    //runs SimulateTurnMove until reaches game state
    public bool GetSimulation()
    {
        this.AIHand.DiscardCard(ChosenCard, false, false);
        CurrentTurn = Turn.PlayerDraw;

        SimulateTurnMove();

        return AIWin;
    }


}