using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Meld
{
    public List<(int,int)> Cards;
    public int Points;
}

//Only for AIHand class to see
//Honestly it might not matter tho, it will generate per run sooooo
public class AISimulationState : MonoBehaviour
{
    public new static AISimulationState instance;
    private Turn CurrentTurn;
    private Stack<(int, int)> DrawPile = new Stack<(int, int)>();
    private Stack<(int, int)> DiscardPile = new Stack<(int, int)>();
    private List<Meld> PlayerMelds = new List<Meld>();
    private List<Meld> AIMelds = new List<Meld>();
    private int AIDeadwoodPoints = 0;
    private int PlayerDeadwoodPoints = 0;
    //public string winner;
    //public int winScore;
    private bool AIWin = false;
    //Same as AI Hand KnownCardSet0
    private int[,] GameState = new int[4, 13];
    //Used for evaluated hands in both Calculate functions
    //private int[] AIHandEvaluation = new int[11];
    //private int[] PlayerHandEvaluation = new int[11];
    

    //Values: 0 = Player Hand, 1 = AI Hand, 2 = Discard Stack, 3 = Draw Stack
    private enum CardLocations {PlayerHand, AIHand, DiscardPile, DrawPile};
    private System.Random rand = new System.Random();
    private int SimulationTurn;
    private Card ChosenCard;
    

    //Constructor as Awake is no longer available 
    public AISimulationState()
    {
        instance = this;
        AIWin = false;
    }

    //GameStateSet will be generated
    public void InitializeAISimState(Card SimDiscardedCard, int[,] simGameState)
    {
        ChosenCard = SimDiscardedCard;
        GameState = simGameState;
        CurrentTurn = Round.instance.CurrentTurn; //since this inherits from Round I think it already has this
        FillCards();
        SimulationTurn = 1;
        Debug.Log("Sim Started");
    }


    //DONE:
    //fill cards by first turning discardpile into stack
    //discard pile based on round instance discard pile
    //then generates remaining playerhand cards into gamestate
    //fills in rest of unknown gamestate as drawpile 
    //creates fills drawpile stack randomly
    private void FillCards()
    {
        //Create DeckSize to ensure accurate simulation

        int UnknownDeckSize = 31 - Round.instance.DiscardPile.CardList.Count;
        const int PlayerHandSize = 10; 
        int PlayerHandKnownSize = 0;
        List<(int, int)> UnknownSpots = new List<(int, int)>();
        int RandomSpot = 0;
        
        //Takes each discardpile card and turns it into a stack
        for (int i = 0; i < Round.instance.DiscardPile.CardList.Count; i++)
        {
            DiscardPile.Push(CardToStateLocation(Round.instance.DiscardPile.GetCard(i))); 
        }


        //Find all known cards in playerHand for randomization later
        //records all unknown cards
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                switch (GameState[i,j])
                {
                    case (int) CardLocations.PlayerHand:
                        PlayerHandKnownSize++;
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
        //Update gamestate for the playerhand location
        //Remove card from unknown spot list
        //Increment KnownPlayerHandsize
        while (PlayerHandKnownSize < PlayerHandSize)
        {
            RandomSpot = rand.Next(0, UnknownSpots.Count - 1);
            GameState[UnknownSpots[RandomSpot].Item1,UnknownSpots[RandomSpot].Item2] = (int) CardLocations.PlayerHand;
            UnknownSpots.RemoveAt(RandomSpot);
            PlayerHandKnownSize++;
        }

        //Fill Drawpile based on deckSize and unfilled cards
        //Create Stack from pile
        //Add to Gamestate as CardLocation.StockStack
        //Add card to gamestate stack for accurate simulation
        //remove from unknown spots
        while (UnknownSpots.Count > 0)
        {
            RandomSpot = rand.Next(0, UnknownSpots.Count - 1);
            GameState[UnknownSpots[RandomSpot].Item1,UnknownSpots[RandomSpot].Item2] = (int) CardLocations.DrawPile;
            this.DrawPile.Push(UnknownSpots[RandomSpot]);
            UnknownSpots.RemoveAt(RandomSpot);
        }


    }


    //Determines current turn, runs calculation method based on whos turn and action
    //Increments SimulationTurn and CurrentTurn, updates GameStateSet
    private void SimulateTurnMove()
    {
        bool isSimulating = true;
        while (isSimulating)
        {
            if (CurrentTurn.Equals(Turn.PlayerDiscard)||CurrentTurn.Equals(Turn.AIDiscard))
            {
                CalculateDiscardCard();
            }
            else
            {
                CalculateDrawCard();
                isSimulating = CalculateIsSim();

            }  
        }
    }

    //randomly picks a pile to draw from
    //updates the gamestate based on player turn and pile drawn from
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
                    (int,int) GameStateCard = DrawPile.Pop();
                    GameState[GameStateCard.Item1,GameStateCard.Item2] = (int) CardLocations.PlayerHand;
                }
                else
                {
                    (int,int) GameStateCard = DiscardPile.Pop();
                    GameState[GameStateCard.Item1,GameStateCard.Item2] = (int) CardLocations.PlayerHand;
                }
                break;
            case Turn.AIDraw:
                CurrentTurn = Turn.AIDiscard;
                if (RandomPile == 0)
                {
                    (int,int) GameStateCard = DrawPile.Pop();
                    GameState[GameStateCard.Item1,GameStateCard.Item2] = (int) CardLocations.AIHand;
                }
                else
                {
                    (int,int) GameStateCard = DiscardPile.Pop();
                    GameState[GameStateCard.Item1,GameStateCard.Item2] = (int) CardLocations.AIHand;
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
                CurrentTurn = Turn.AIDraw;
                break;
            case Turn.AIDiscard:
                currentHand = CardLocations.AIHand;
                CurrentTurn = Turn.PlayerDraw;
                break;
        }

        int count = 0;
        List<(int, int)> Spots = new List<(int, int)>();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                count++;
                //Debug.Log("Count: "+ count+"Spot: " + GameState[i,j] + " Suit: " + i + " Face:" + j);
                if (GameState[i,j] == (int) currentHand)
                {
                    Spots.Add((i,j));
                    //Debug.Log(Spots.Count);
                    //Debug.Log(currentHand + " Suit: " + i + " Face:" + j);
                    //Debug.Log(Spots[Spots.Count]);
                }
            }
        }

        Debug.Log("Spots"+Spots.Count);
        int RandomSpot = rand.Next(0, Spots.Count - 1);

        (int,int) GameStateCard = (Spots[RandomSpot].Item1, Spots[RandomSpot].Item2);
        DiscardPile.Push(GameStateCard);
        GameState[GameStateCard.Item1, GameStateCard.Item2] = (int) CardLocations.DiscardPile;

    }

    //Empty Melds for AI and Player
    //Count all total deadwoods

    //Runs throught limited nested forloop for runs
    //Add runs to ai and player melds

    //Runs through limited nested forloop for sets
    //Adds sets to ai and player melds

    //Compare melds, discard melds with same cards and lower deadwoods
    //Subtract meld deadwoods from total deadwoods
    //Store into ai and player deadwood points

    //OPTIMIZATION, not recreating new melds based on existing ones
    private void CalculateHands()
    {

        //optimization ideas
        //overlaps only occur when RUNS and SETS have the same card
        //only check for runs and sets via counts

        PlayerMelds.Clear();
        AIMelds.Clear();

        PlayerDeadwoodPoints = 0;
        AIDeadwoodPoints = 0;

        //Run nested for loop
        //check for hand location
        //convert face value to total deadwood points
        //add to respective deadwood hands
        for (int suit = 0; suit < 4; suit++)
        {
            for (int face = 0; face < 13; face++)
            {
                if (GameState[suit, face] == (int) CardLocations.PlayerHand)
                {
                    PlayerDeadwoodPoints += CardFaceToPoints(face);
                }
                else if (GameState[suit, face] == (int) CardLocations.AIHand)
                {
                    AIDeadwoodPoints += CardFaceToPoints(face);
                }
            }
        }

        
        //nested for loop to check for runs
        //only runs through face values until the last 2 cards
        //to optimize runs
        for (int suit = 0; suit < 4; suit++)
        {
            for (int face = 12; face > 1; face--)
            {
                if (GameState[suit, face] == (int) CardLocations.AIHand)
                {
                    Meld newMeld = GetRun(CardLocations.AIHand, suit, face);

                    //check if meld is valid
                    if (newMeld.Cards.Count > 2)
                    {
                        AIMelds.Add(newMeld);
                    }

                }
                
                else if (GameState[suit, face] == (int) CardLocations.PlayerHand)
                {
                    Meld newMeld = GetRun(CardLocations.PlayerHand,suit, face);

                    if (newMeld.Cards.Count > 2)
                    {
                        PlayerMelds.Add(newMeld);
                    }
                }
            }
        }

        //Nested for loop to check for sets
        //only checks 2 of 4 suit values
        //to optimize runs
        for (int face = 0; face < 13; face++)
        {
            for (int suit = 0; suit < 2; suit++)
            {
                if (GameState[suit, face] == (int) CardLocations.AIHand)
                {
                    Meld newMeld = GetRun(CardLocations.AIHand, suit, face);

                    //check if meld is valid
                    if (newMeld.Cards.Count > 2)
                    {
                        AIMelds.Add(newMeld);
                    }

                }
                
                else if (GameState[suit, face] == (int) CardLocations.PlayerHand)
                {
                    Meld newMeld = GetSet(CardLocations.PlayerHand, suit, face);

                    if (newMeld.Cards.Count > 2)
                    {
                        PlayerMelds.Add(newMeld);
                    }
                }
            }
        }



        //Run through melds linearly
        //Checks all cards in meld, checks if each meld contains that card
        //compares deadwood, removes the meld with less deadwoods

        //-1 to account for both OBOB and no need to compare last meld
        for (int currentMeld = 0; currentMeld < PlayerMelds.Count - 1; currentMeld++)
        {
            for (int compareMeld = currentMeld + 1; currentMeld < PlayerMelds.Count; compareMeld++)
            {
                int numCards = PlayerMelds[currentMeld].Cards.Count;
                for (int currentCard = 0; currentCard < numCards; currentCard++)
                {
                    if (PlayerMelds[compareMeld].Cards.Contains(PlayerMelds[currentMeld].Cards[currentCard]))
                    {
                        //Prioritizes runs over sets
                        if (PlayerMelds[compareMeld].Points < PlayerMelds[currentMeld].Points)
                        {
                            PlayerMelds.RemoveAt(currentMeld);
                        }
                        
                        else
                        {
                            PlayerMelds.RemoveAt(compareMeld);
                        }

                    }
                }
            }
        }

        //does the same thing but with AI hand
        //Run through melds linearly
        //Checks all cards in meld, checks if each meld contains that card
        //compares deadwood, removes the meld with less deadwoods
        //-1 to account for both OBOB and no need to compare last meld
        for (int currentMeld = 0; currentMeld < AIMelds.Count - 1; currentMeld++)
        {
            for (int compareMeld = currentMeld + 1; currentMeld < AIMelds.Count; compareMeld++)
            {
                int numCards = AIMelds[currentMeld].Cards.Count;
                for (int currentCard = 0; currentCard < numCards; currentCard++)
                {
                    if (AIMelds[compareMeld].Cards.Contains(AIMelds[currentMeld].Cards[currentCard]))
                    {
                        //Prioritizes runs over sets
                        if (AIMelds[compareMeld].Points < AIMelds[currentMeld].Points)
                        {
                            AIMelds.RemoveAt(currentMeld);
                        }
                        
                        else
                        {
                            AIMelds.RemoveAt(compareMeld);
                        }
                    }
                }
            }
        }

        //Subtract hand points from their total melds points
        for (int currentMeld = 0; currentMeld < AIMelds.Count; currentMeld++)
        {
            AIDeadwoodPoints -= AIMelds[currentMeld].Points;
        }

        for (int currentMeld = 0; currentMeld < PlayerMelds.Count; currentMeld++)
        {
            PlayerDeadwoodPoints -= PlayerMelds[currentMeld].Points;
        }

        Debug.Log("AI: " + AIDeadwoodPoints+" Player: "+ PlayerDeadwoodPoints);

    }

    //runs through next few locations to check if spot creates a set
    //if a set is create, create meld and add card locations to set
    //calulates all deadwood and stores that
    //returns empty meld if not a set
    private Meld GetRun(CardLocations hand, int suit, int face)
    {
        Meld newRun = new Meld();
        newRun.Cards = new List<(int,int)>();
        newRun.Points = 0;
        int count = 0;

        int currentCardFace = face;
        if (GameState[suit,currentCardFace] == (int) hand && currentCardFace >= 0)
        {
            Debug.Log("Run");
            newRun.Cards.Add((currentCardFace, face));
            newRun.Points += CardFaceToPoints(currentCardFace);
            count++;
            currentCardFace--;
        }
        return newRun;
    }

    //runs through next few locations to check if spot creates a run
    //if a set is create, create meld and add card locations to run
    //calculates all deadwoods and stores that
    //returns empty meld if not a set
    private Meld GetSet(CardLocations hand, int suit, int face)
    {
        Meld newSet = new Meld();
        newSet.Cards = new List<(int,int)>();
        newSet.Points = 0;
        int count = 0;

        int currentCardSuit = suit;
        if (GameState[currentCardSuit, face] == (int) hand && currentCardSuit <= 3)
        {
            Debug.Log("Set");
            newSet.Cards.Add((currentCardSuit, face));
            newSet.Points += CardFaceToPoints(face);
            count++;
            currentCardSuit--;
        }

        return newSet;
    }

    //Converts face value of card
    //to desired point value of card 
    //return -1 on failure
    private int CardFaceToPoints(int faceValue)
    {
        if (faceValue >= 10)
        {
            return 10;
        }
        else
        {
            return faceValue + 1;
        }
    }
    
 
    //Takes a card, turns it into a GameState location tuple
    private (int,int) CardToStateLocation(Card card)
    {
        (int,int) cardLocation = (-1,-1);
        cardLocation.Item1 = (int) card.CardSuit;
        cardLocation.Item2 = (int) card.FaceValue - 1;
        return cardLocation;
    }
    


    //Calculate deadwoods via the table
    //determine if knocking is worthwhile for any 
    private bool CalculateIsSim()
    {
        bool isSim = true;
        this.CalculateHands();

        //If a draw, return a loss
        if (this.DrawPile.Count < 2)
        {
            return false;
        }

        if (CurrentTurn ==  Turn.AIDraw)
        {
            if (AIDeadwoodPoints < 10)
            {
                //Gin / Big Gin
                if (AIDeadwoodPoints == 0)
                {
                    AIWin = true;
                    isSim = false;
                }

                //Undercut
                else if (AIDeadwoodPoints < PlayerDeadwoodPoints)
                {
                    AIWin = true;
                    isSim = false;
                }
            
            }
        }
        else if (PlayerDeadwoodPoints < 10)
        {
            //Gin / Big Gin
            if (PlayerDeadwoodPoints == 0)
                {
                    isSim = false;
                }
                
                //Undercut
                else if (AIDeadwoodPoints < PlayerDeadwoodPoints)
                {
                    AIWin = true;
                    isSim = false;
                }
        }
        
        return isSim;
    }

    //Starts simulation, returns if simulation got to "win" or "advantage" state
    //runs SimulateTurnMove until reaches game state

    //Start by discarding chosen card 
    //run simulations
    public bool GetSimulation()
    {
        (int,int) StateChosenCard = CardToStateLocation(ChosenCard);  
        GameState[StateChosenCard.Item1, StateChosenCard.Item2] = (int) CardLocations.DiscardPile;
        CurrentTurn = Turn.PlayerDraw;
        SimulationTurn++;

        SimulateTurnMove();

        Debug.Log("Sim Ended");
        return AIWin;
    }


}




   /*
    WITH THE GAMESTATE ITERATION OF SIMULATIONS, 
    private SimCard  GenerateCard(int suit, int face)
    {
        SimCard ReturnCard = new SimCard();
        switch (suit)
            {
                case (int) Card.Suit.Clubs:
                    ReturnCard.CardSuit = SimCard.Suit.Clubs;
                    break;

                case (int) Card.Suit.Diamonds:
                    ReturnCard.CardSuit = SimCard.Suit.Diamonds;
                    break;

                case (int) Card.Suit.Hearts:
                    ReturnCard.CardSuit = SimCard.Suit.Hearts;
                    break;

                case (int) Card.Suit.Spades:
                    ReturnCard.CardSuit = SimCard.Suit.Spades;
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
    */
