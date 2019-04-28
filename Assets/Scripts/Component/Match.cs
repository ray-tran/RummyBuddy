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
            StartCoroutine(Dealer.instance.ShuffleCoroutine());//Clean table, shuffle, deal
            //Dealer.instance.ShuffleCoroutine();
        }
        else
        {
            Round.instance.InitializeRound();
        }
        RoundCount++;
    }

    //winType:
    //-1: undercut
    //0: knock
    //1: gin
    //2: big gin
    public void UpdateMatchResult(string winner, int score, int winType)
    {
        //Add the winner and string to our updated results
        List<string> UpdateResult = new List<string>
        {
            winner,
            score.ToString()
        };

        if (winner == "player")
        {
            PlayerScore += score;
        }
        else if (winner == "ai")
        {
            AIScore += score;
        }

        //Add our list of updated results to the round result list
        RoundResult.Add(UpdateResult);

        if (PlayerScore >= 100 || AIScore >= 100)
        {
            EndMatch(winner, score, winType);
        }
        else
        {
            GameUI.instance.DisplayRoundResult(winner, score, winType, false);
        }

    }

    //public void DisplayRoundResult(string winner, int score, int winType) 
    //{
    //    GameObject roundResults;

    //    Debug.Log("Round winner: " + winner);
    //    Debug.Log("Winner round score: " + score);
    //    Debug.Log("Win type round score: " + winType);

    //    Debug.Log("Player match score: " + PlayerScore);
    //    Debug.Log("AI match score: " + AIScore);


    //    //Flip cards around, requires new layout of card slots. Maybe done later as it's not super important now
    //    //Display round score
    //    //Display updated match score
    //    //Display continue button. If continue button is clicked then call StartRound()
    //}


    public void EndMatch(string winner, int score, int winType)
    {
        //Update stats, display result scene
        GameUI.instance.DisplayRoundResult(winner, score, winType, true);
        //SceneManager.LoadScene(0);
    }
}
