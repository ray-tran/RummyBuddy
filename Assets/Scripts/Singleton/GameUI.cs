using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour 
{
    public static GameUI instance;
    public bool endMatch = false;
    public Text currentDeadwoodPointsText;
    public Text currentTurnText;

    //Round results panel
    private Text PlayerRoundDWPointsText;
    private Text AIRoundDWPointsText;
    private Text WinScoreText;
    private Text WinTypeText;
    private Text PlayerMatchScoreText;
    private Text AIMatchScoreText;
    private Text MatchWinText;

    private GameObject RoundResultPanelGO;
    private GameObject ContinueButtonGO;

    private GameObject MatchWinTextGO;
    private GameObject MainMenuButtonGO;


    private GameObject EndRoundMoveButtonsPanelGO;
    private GameObject BigGinButtonGO;
    private GameObject GinButtonGO;
    private GameObject KnockButtonGO;

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


        //End round move buttons
        EndRoundMoveButtonsPanelGO = GameObject.Find("EndRoundMoveButtonsPanel");
        EndRoundMoveButtonsPanelGO.SetActive(false);

        BigGinButtonGO = EndRoundMoveButtonsPanelGO.transform.Find("BigGinButton").gameObject;
        GinButtonGO = EndRoundMoveButtonsPanelGO.transform.Find("GinButton").gameObject;
        KnockButtonGO = EndRoundMoveButtonsPanelGO.transform.Find("KnockButton").gameObject;
        BigGinButtonGO.SetActive(false);
        GinButtonGO.SetActive(false);
        KnockButtonGO.SetActive(false);
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
        DisableEndRoundMoveButton();
        Match.instance.StartRound();
    }
    public void GoToMainMenuUI()
    {
        SceneManager.LoadScene("MainMenu");
    }

    //End round call type:
    //0: knock
    //1: gin
    //2: big gin
    public void DisplayEndRoundMoveButton(int legalType)
    {
        switch(legalType)
        {
            case 0:
                KnockButtonGO.SetActive(true);
                GinButtonGO.SetActive(false);
                BigGinButtonGO.SetActive(false);
                break;
            case 1:
                GinButtonGO.SetActive(true);
                KnockButtonGO.SetActive(false);
                BigGinButtonGO.SetActive(false);
                break;
            case 2:
                BigGinButtonGO.SetActive(true);
                GinButtonGO.SetActive(false);
                KnockButtonGO.SetActive(false);
                break;
        }
        EndRoundMoveButtonsPanelGO.SetActive(true);
    }

    public void DisableEndRoundMoveButton()
    {
        EndRoundMoveButtonsPanelGO.SetActive(false);
    }


    public void DisplayRoundResult(string winner, int score, int winType, bool endMatch)
    {
        endMatch = true;
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
