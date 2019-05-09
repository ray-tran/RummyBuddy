using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHand : PlayerHand
{
    public new static AIHand instance;
    private Dictionary<Card, CardSlot> KnownCards = new Dictionary<Card, CardSlot>();
    //private HashSet<Card> DesiredCards = new HashSet<Card>();

    public int SimulationCount = 1000;
    public float c = Mathf.Sqrt(2);

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
        if (!silent)
            InitializeCardSlots();
        //Debug.Log("Check y (AI):" + CardSlotList[4].TargetTransform.position.y);

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

        //printGameState();

        //If drawing top discard card would result in 3 cards or more in a run
        //Or if drawing top discard card would result in 3 cards or more of same rank
        if (GetContRunCount(topDiscardCard, 1) >= 3 || GetColumnValCount(topDiscardCard, 1) >= 3)
        {
            //Temporary add card to AIHand without UI display to calculate Deadwoods
            int origDeadwoodPoints = DeadwoodPoints;

            //Debug.Log("origDeadwoodPoints: " + origDeadwoodPoints);

            //false flag to disable UI drawing (this is happening in the background)
            DrawCard(topDiscardCard, false);
            int testingDeadwoodPoints = DeadwoodPoints;

            //Debug.Log("testingDeadwoodPoints: " + testingDeadwoodPoints);

            DiscardCard(topDiscardCard, false, false);

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
    //WILL: Create 11x2 array based on card slots, 0 = Wins, 1 = Sims
    //Send current known cards to AI sim
    //Evaluate based on simulations
    //Add runs and wins to slot
    //Repeat based on stats and algotithm
    //Return Card with most simulations
    private Card DecideCardToDiscard()
    {
        //TODO: Uncomment when testing AISimulationState
        
        //We only consider which deadwood cards to discard
        int deadwoodsCount = Deadwoods.Count;

        // 2D array to store simulations result
        // There are deadwoodsCount arrays of 2 numbers
        // Each array tells if discarding this card will likely result in a win or not
        // Each array of number represents: wins (index 0) over sims (index 1)
        int[,] simsResult = new int[deadwoodsCount, 2];
        FillMatrix(simsResult, 1);

         for (int totalSims = 1; totalSims <= SimulationCount; totalSims++)
         {
            //Debug.Log("-----------");
            //Debug.Log("Running sim " + totalSims);

             // UCT = Wins/Sims + c*sqrt(Log(total sims of all choices)/Sims))
             // exploitation component : Wins/Sims
             // exploration component: sqrt(Log(total sims)/sims))
             // c: trade-off between exploitation and exploration
             // CHOOSE ONE WITH LARGEST UCT

            double maxUCT = 0; int maxUCTIndex = 0;
            if (totalSims == SimulationCount)
            {
                for (int i = 0; i < deadwoodsCount; i++)
                {
                    int wins = simsResult[i, 0]; int sims = simsResult[i, 1];

                    Debug.Log("-----Branch " + i + ": " + simsResult[i, 0] + "/" + simsResult[i, 1]);
                    //double winOverSim = (double)wins / sims;

                    //Debug.Log("wins / sims: " + winOverSim);
                    //Debug.Log("Mathf.Log(totalSims): " + Mathf.Log(totalSims));
                    //double logOVerSims = Mathf.Log(totalSims) / sims;
                    //Debug.Log("Mathf.Log(totalSims)/sims: " + logOVerSims);
                    //double exploitation = logOVerSims * c;
                    //Debug.Log("c * Mathf.Sqrt(Mathf.Log(totalSims) / sims" + exploitation);
                    //double full = winOverSim + exploitation;
                    //Debug.Log("FULL: " + full);

                    double UCT = ((double)wins / sims) + (c * Mathf.Sqrt(Mathf.Log(totalSims) / sims));
                    Debug.Log("UCT " + UCT);

                    if (UCT > maxUCT)
                    {
                        maxUCT = UCT;
                        maxUCTIndex = i;
                    }
                    Debug.Log("Max UCT " + maxUCT);
                }

            }
            else
            {
                for (int i = 0; i < deadwoodsCount; i++)
                {
                    int wins = simsResult[i, 0]; int sims = simsResult[i, 1];
                    double UCT = (wins / sims) + (c * Mathf.Sqrt(Mathf.Log(totalSims) / sims));
                    if (UCT > maxUCT)
                    {
                        maxUCT = UCT;
                        maxUCTIndex = i;
                    }
                }

            }
            //for (int i = 0; i < deadwoodsCount; i++)
            //{               
            //    int wins = simsResult[i,0]; int sims = simsResult[i,1];
            //    double UCT = (wins / sims) + (c * Mathf.Sqrt(Mathf.Log(totalSims) / sims));
            //    if (UCT > maxUCT)
            //    {
            //        maxUCT = UCT;
            //        maxUCTIndex = i;
            //    }
            //}

            Debug.Log("Branch taken: " + maxUCTIndex);

            int[,] simGameState = (int[,])KnownGameState.Clone(); //Create a deep copy of game state
            //PutCardInGameState(simGameState, Deadwoods[maxUCTIndex], 2); //Discard chosen card in the game state
            AISimulationState.instance.InitializeAISimState(Deadwoods[maxUCTIndex], simGameState);
            //AISimulationState sim = new AISimulationState(Deadwoods[maxUCTIndex], simGameState);
            if (AISimulationState.instance.GetSimulation())
            {
                simsResult[maxUCTIndex, 0]++; //If sim returns a win, update win count for this card
                //Debug.Log("This sim is a win!");

            }
            simsResult[maxUCTIndex, 1]++; //Update # of sims for this card
         }


         //Choose the card with most simulations
        int maxSimCount = 0; int maxSimCountIndex = 0;
        for (int i = 0; i < deadwoodsCount; i++)
        {
            Debug.Log("Branch " + i + ": " + simsResult[i, 0] + "/" + simsResult[i, 1]);
            if (simsResult[i,1] > maxSimCount)
            {
                maxSimCount = simsResult[i, 1];
                maxSimCountIndex = i;
            }
        }

        int maxWin = simsResult[maxSimCountIndex, 0];
        int optimalIndex = maxSimCountIndex;
        for (int i = 0; i < deadwoodsCount; i++)
        {
            if (simsResult[i,1] == maxSimCount && simsResult[i, 0] >= maxWin)
            {
                maxWin = simsResult[i, 0];
                optimalIndex = i;
            }
        }



        Debug.Log("Choosing branch: " + optimalIndex);
        //TODO: uncomment following when AISimulationState is done
        return Deadwoods[optimalIndex];


        //TEMPORARY STRATEGY//
        //The last card in Deadwoods the highest valued deadwood
        //return Deadwoods[Deadwoods.Count-1];
    }



    public void InitializeGameState()
    {
        FillMatrix(KnownGameState, -1);

        foreach (Card card in CardsInHand)
        {
            PutCardInGameState(KnownGameState,card, 1);
        }

        PutCardInGameState(KnownGameState, Round.instance.DiscardPile.TopCard(), 2);
        //printGameState();
    }

    public void PutCardInGameState(int[,] gameState, Card card, int val)
    {
        //Debug.Log("Putting " + card.name + " in game state");

        try
        {
            int row = (int)card.CardSuit;
            int col = card.Rank - 1;
            gameState[row, col] = val;
        }
        catch (System.Exception) 
        { 
            Debug.Log("PutCardInGameState error");
            Debug.Log("row: " + (int)card.CardSuit);
            Debug.Log("col: " + (card.Rank - 1));
            Debug.Log("matrix size: " + gameState.GetLength(0) + " x " + gameState.GetLength(1));

        }

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


        int legalEndRoundType = CheckLegalEndRoundMove();

        //End round call type:
        //0: knock
        //1: gin
        //2: big gin
        //-1: no legal end round move
        switch (legalEndRoundType)
        {
            case 0:
                Knock();
                break;
            case 1:
                Gin();
                break;
            case 2:
                BigGin();
                break;
            case -1:
                Round.instance.UpdateTurn(Turn.AIDiscard);
                Invoke("AIDiscard", 1f);
                break;
        }
    }

    private void AIDiscard()
    {
        DiscardCard(DecideCardToDiscard(), true, false);
    }

}
