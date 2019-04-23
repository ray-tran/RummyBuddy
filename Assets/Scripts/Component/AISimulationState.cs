using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Only for AIHand class to see
//Honestly it might not matter tho, it will generate per run sooooo
public class AISimulationState: Round
{
    //Same as AI Hand KnownCardSet
    private int[,] RootGameState = new int[4, 13];

    //Used for evaluated hands in both Calculate functions
    private int[] AIHandEvaluation = new int[11];
    private int[] PlayerHandEvaluation = new int[11];

    //Values: 0 = Player Hand, 1 = AI Hand, 3 Discard Stack, 4 Stock Stack
    private enum CardLocations {PlayerHand, AIHand, DiscardStack, StockStack};
    private int SimulationTurn;
    private Card ChosenCard;
    

    //On construction, GameStateSet will be generated
    //
    public AISimulationState(Card SimDiscardedCard, int[,] CurrentGameState)
    {
        ChosenCard = SimDiscardedCard;
        RootGameState = CurrentGameState;
        CurrentTurn = Round.instance.CurrentTurn;
        DiscardPile = Round.instance.DiscardPile;
        FillCards();
        SimulationTurn = 1;
    }

    //TODO: Runs through GameStateSet at construction
    //Finds all -1 spots, and fills out rest of cards based on empty spaces
    //Will track maximum PlayerHand cards
    //Will generate Stock Stack based on random filled cards
    //Also fills out PlayerHand and AIHand based on GameStateSet
    private void FillCards()
    {

    }

    //TODO: Determines current turn, runs calculation method based on whos turn and action
    //Increments SimulationTurn and CurrentTurn, updates GameStateSet
    private void SimulateTurnMove()
    {

    }

    //TODO: Determines which card to draw, based on whos turn, their hand, and "Best guess" of desired cards
    //Returns which pile to draw from
    private int CalculateDrawCard()
    {
        return -1;
    } 

    //TODO: Determines which card to discard, based on whos turn, their hand, and calculated and "weighted" cards
    //Evaluates hand, makes best intentioned guess
    private Card CalculateDiscardCard()
    {
        return null;
    }


    //TODO: Starts simulation, returns if simulation got to "win" or "advantage" state
    //runs SimulateTurnMove until reaches game state
    public bool GetSimulation()
    {   
        return true;
    }


}