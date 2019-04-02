using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match : MonoBehaviour
{
    // A match has multiple rounds
    public static Match instace;
    public int PlayerScore { get; set; }
    public int AIScore { get; set; }
    public int RoundCount { get; set; }
    public int PlayerWinCount { get; set; }
    public int PlayerLossCount { get; set; }
    public List<List<string>> RoundResult; //e.g. [["player", "23"], ["ai","49"],...]

    // Start is called before the first frame update
    void Awake()
    {
        instace = this;
    }

    public void StartRound()
    {
        RoundCount++;
        //Clean table, shuffle, deal
        Dealer.instace.ShuffleCoroutine();
        //Reset round data
        Round.instance.InitializeRound();
    }

    public void UpdateMatchResult(string winner, int score)
    {
        //update
        //if either +100 points, call EndMatch
    }

    public void EndMatch()
    {
        //Update stats, display result scene
    }
}
