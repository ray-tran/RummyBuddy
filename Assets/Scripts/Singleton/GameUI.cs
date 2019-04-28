using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour 
{
    public static GameUI instance;
    public Text currentTurnText;
    public Text currentDeadwoodPointsText;

    //Round results panel
    public Text PlayerRoundDWPointsText;
    public Text AIRoundDWPointsText;
    public Text WinScoreText;
    public Text WinTypeText;
    public Text PlayerMatchScoreText;
    public Text AIMatchScoreText;
    public Text MatchWinText;

    public GameObject RoundResultPanelGO;
    private GameObject ContinueButtonGO;

    private GameObject MatchWinTextGO;
    private GameObject MainMenuButtonGO;

    private void Awake()
	{
        instance = this;
        GameObject scoreGO = GameObject.Find("CurrentTurn");
        GameObject deadpointsGO = GameObject.Find("CurrentPoints");
        RoundResultPanelGO = GameObject.Find("RoundResultsPanel");
        RoundResultPanelGO.SetActive(false);

        currentTurnText = scoreGO.GetComponent<Text>();
        currentTurnText.text = "Current turn: PlayerDraw";
        currentDeadwoodPointsText = deadpointsGO.GetComponent<Text>();

        ContinueButtonGO = RoundResultPanelGO.transform.Find("ContinueButton").gameObject;
        PlayerRoundDWPointsText = RoundResultPanelGO.transform.Find("PlayerDWPointsText").gameObject.GetComponent<Text>();
        AIRoundDWPointsText = RoundResultPanelGO.transform.Find("AIDWPointsText").gameObject.GetComponent<Text>();
        WinScoreText = RoundResultPanelGO.transform.Find("WinScoreText").gameObject.GetComponent<Text>();   
        WinTypeText = RoundResultPanelGO.transform.Find("WinTypeText").gameObject.GetComponent<Text>();
        PlayerMatchScoreText = RoundResultPanelGO.transform.Find("PlayerScoreText").gameObject.GetComponent<Text>();
        AIMatchScoreText = RoundResultPanelGO.transform.Find("AIScoreText").gameObject.GetComponent<Text>();

        MatchWinTextGO = RoundResultPanelGO.transform.Find("MatchWinText").gameObject;
        MatchWinTextGO.SetActive(false);
        MatchWinText = MatchWinTextGO.GetComponent<Text>();

        MainMenuButtonGO = RoundResultPanelGO.transform.Find("MainMenuButton").gameObject;
        MainMenuButtonGO.SetActive(false);
    }

    //Called in Player.ScanHand() and Round.UpdateTurn()
    public void UpdateScoreUI()
    {
        currentDeadwoodPointsText.text = "Points: " + PlayerHand.instance.DeadwoodPoints;
    }
    public void GinUI()
    {
        PlayerHand.instance.Gin();
    }
    public void BigGinUI()
    {
        PlayerHand.instance.BigGin();
    }
    public void KnockUI()
    {
        PlayerHand.instance.Knock();
    }
	public void ContinueUI()
    {
        RoundResultPanelGO.SetActive(false);
        Match.instance.StartRound();
    }
    public void GoToMainMenuUI()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void DisplayRoundResult(string winner, int score, int winType, bool endMatch)
    {
        PlayerRoundDWPointsText.text = PlayerHand.instance.DeadwoodPoints.ToString();
        AIRoundDWPointsText.text = AIHand.instance.DeadwoodPoints.ToString();
        string winTypeText = "";
        switch(winType)
        {
            case 0:
                winTypeText = "KNOCK";
                break;
            case 1:
                winTypeText = "GIN";
                break;
            case 2:
                winTypeText = "BIG GIN";
                break;
            case -1:
                winTypeText = "UNDERCUT";
                break;
        }

        WinTypeText.text = winTypeText;
        WinScoreText.text = "+" + score.ToString() + " for " + winner.ToUpper();

        PlayerMatchScoreText.text = "Player score: " + Match.instance.PlayerScore;
        AIMatchScoreText.text = "AI score: " + Match.instance.AIScore;

        //Debug.Log("Round winner: " + winner);
        //Debug.Log("Winner round score: " + score);
        //Debug.Log("Win type round score: " + winType);

        //Debug.Log("Player match score: " + Match.instance.PlayerScore);
        //Debug.Log("AI match score: " + Match.instance.AIScore);

        if (endMatch)
        {
            MatchWinText.text = winner.ToUpper() + " WON";
            MatchWinTextGO.SetActive(true);
            ContinueButtonGO.SetActive(false);
            MainMenuButtonGO.SetActive(true);
        }


        RoundResultPanelGO.SetActive(true);


        //Flip cards around, requires new layout of card slots. Maybe done later as it's not super important now
        //Display round score
        //Display updated match score
        //Display continue button. If continue button is clicked then call StartRound()
    }

}
