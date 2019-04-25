using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Match : MonoBehaviour
{
    // A match has multiple rounds
    public static Match instance;
    public int PlayerScore { get; set; }
    public int AIScore { get; set; }
    public int RoundCount { get; set; }
    public int PlayerWinCount { get; set; }
    public int PlayerLossCount { get; set; }
    public List<List<string>> RoundResult; //e.g. [["player", "23"], ["ai","49"],...] 
                                           //means player wins first round with 23 points
    public List<string> UpdateResult;  

    // Start is called before the first frame update
    void Awake()
    {
        RoundCount = 0;
        instance = this;
    }

    //Reset match data
    public void InitializeMatch()
    {
        PlayerScore = 0;
        AIScore = 0;
        RoundCount = 0;
        PlayerWinCount = 0;
        PlayerLossCount = 0;
        RoundResult = new List<List<string>>();
        StartRound();
    }


    public void StartRound()
    { 
        if (RoundCount != 0)
        {
            Dealer.instance.ShuffleCoroutine(); //Clean table, shuffle, deal
        }
        RoundCount++;
        Round.instance.InitializeRound();
    }

    public void UpdateMatchResult(string winner, int score)
    {
        //Add the winner and string to our updated results
        UpdateResult.Add(winner);
        UpdateResult.Add(score.ToString());

        //Add our list of updated results to the round result list
        RoundResult.Add(UpdateResult);

        //if either +100 points, call EndMatch
        if (score > 100)
        {
            print("Score is over 50");
            //EndMatch();
        }
        //else call DisplayRoundResult
        else
        {
            print("Score is under 50");
            //DisplayRoundResult();
        }
    }

        public void DisplayRoundResult()
    {
        GameObject roundResults;
        //Flip cards around, requires new layout of card slots. Maybe done later as it's not super important now
        //Display round score
        //Display updated match score
        //Display continue button. If continue button is clicked then call StartRound()
    }


    public void EndMatch()
    {
        //Update stats, display result scene
        SceneManager.LoadScene(0);
    }
}
