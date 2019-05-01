//She's a little rough, but this can get basic implementation started

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Only for AIHand class to see
//Honestly it might not matter tho, it will generate per run sooooo
public class AISimulationState: Round
{
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
        for (int i = this.DiscardPile.CardList.Count; i > 0; i--)
        {
            GameState[(int) this.DiscardPile.CardList[i].CardSuit, this.DiscardPile.CardList[i].Rank - 1] = (int) CardLocations.DiscardPile;
        }

        //Find all known cards in playerHand, then fill them
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                switch (GameState[i,j])
                {
                    case (int) CardLocations.PlayerHand:
                        PlayerHandKnownSize++;
                        PlayerHand.CardsInHand.Add(this.GenerateCard(i,j));
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
        //Generate card based on spot, put in DrawPile
        //Add to Gamestate as CardLocation.StockStack
        //
        //POTENTIALLY create new data stack to limit object generation
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
        while (DrawPile.CardList.Count > 0)
        {
            if (CurrentTurn.Equals(Turn.PlayerDiscard)||CurrentTurn.Equals(Turn.AIDiscard))
            {
                CalculateDrawCard();
            }
            else
            {
                CalculateDiscardCard();
            }
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
                    this.PlayerHand.DrawCard(DrawPile.TopCard(), false);
                    //this.PlayerHand.CardsInHand.Add(this.DrawPile.TopCard());
                    this.DrawPile.CardList.Remove(this.DrawPile.TopCard());

                }
                else
                {
                    (int,int) GameStateCard = CardToStateLocation(DiscardPile.TopCard());
                    GameState[GameStateCard.Item1,GameStateCard.Item2] = (int) CardLocations.PlayerHand;
                    this.PlayerHand.DrawCard(this.DiscardPile.TopCard(), false);
                    //PlayerHand.CardsInHand.Add(this.DiscardPile.TopCard());
                    this.DiscardPile.CardList.Remove(this.DiscardPile.TopCard());
                }
                break;
            case Turn.AIDraw:
                CurrentTurn = Turn.AIDiscard;
                if (RandomPile == 0)
                {
                    (int,int) GameStateCard = CardToStateLocation(DrawPile.TopCard());
                    GameState[GameStateCard.Item1,GameStateCard.Item2] = (int) CardLocations.AIHand;
                    this.AIHand.DrawCard(this.DrawPile.TopCard(), false);
                    //AIHand.CardsInHand.Add(DrawPile.TopCard());
                    this.DrawPile.CardList.Remove(this.DrawPile.TopCard());

                }
                else
                {
                    (int,int) GameStateCard = CardToStateLocation(DiscardPile.TopCard());
                    GameState[GameStateCard.Item1,GameStateCard.Item2] = (int) CardLocations.AIHand;
                    this.AIHand.DrawCard(this.DiscardPile.TopCard(), false);
                    //this.AIHand.CardsInHand.Add(this.DiscardPile.TopCard());
                    this.DiscardPile.CardList.Remove(this.DiscardPile.TopCard());

                }
                break;
        }

        
    } 

    //Determines which card to discard, based on whos turn, their hand, and calculated and "weighted" cards
    //Evaluates hand, makes best intentioned guess

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
                Spots.Add((i,j));
            }
        }

        int RandomSpot = rand.Next(0, Spots.Count - 1);
        if (GameState[Spots[RandomSpot].Item1,Spots[RandomSpot].Item2] == (int) currentHand)
        {        
            Card NewCard = GenerateCard(Spots[RandomSpot].Item1,Spots[RandomSpot].Item2);
            
            if (currentHand == CardLocations.PlayerHand)
            {
                this.PlayerHand.CardsInHand.Remove(NewCard);
            }
            else 
            {
               this.AIHand.CardsInHand.Remove(NewCard);
            }
            this.DiscardPile.AddCard(NewCard);
            //Destroy(NewCard);
        }
    }

    private Card GenerateCard(int suit, int face)
    {
        Card ReturnCard = new Card();
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

    //Starts simulation, returns if simulation got to "win" or "advantage" state
    //runs SimulateTurnMove until reaches game state
    public bool GetSimulation()
    {
        bool AIWin = false;
        this.AIHand.DiscardCard(ChosenCard, false, false);
        this.DiscardPile.CardList.Remove(ChosenCard);
        CurrentTurn = Turn.PlayerDraw;

        SimulateTurnMove();

        this.AIHand.ScanHand(false);
        this.PlayerHand.ScanHand(false);

        if (AIHand.DeadwoodPoints < PlayerHand.DeadwoodPoints)
        {
            AIWin = true;
        }

        return AIWin;
    }


}