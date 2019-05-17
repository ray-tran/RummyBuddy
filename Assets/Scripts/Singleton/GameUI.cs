using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour 
{
    public static GameUI instance;
    private Text currentDeadwoodPointsText;
    private Text PlayerUIScoreText;
    private Text AIUIScoreText;

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
        currentDeadwoodPointsText = GameObject.Find("CurrentPoints").GetComponent<Text>();
        PlayerUIScoreText = GameObject.Find("PlayerScoreTextUI").GetComponent<Text>();
        AIUIScoreText = GameObject.Find("AIScoreTextUI").GetComponent<Text>();
        currentDeadwoodPointsText.text = "0";
        PlayerUIScoreText.text = "0";
        AIUIScoreText.text = "0";

        RoundResultPanelGO = GameObject.Find("RoundResultsPanel");
        RoundResultPanelGO.SetActive(false);


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
    public void UpdateDWUI()
    {
        currentDeadwoodPointsText.text = ""+ GameObject.Find("PlayerHand").GetComponent<PlayerHand>().DeadwoodPoints;
    }
    public void GinUI()
    {
        GameObject.Find("PlayerHand").GetComponent<PlayerHand>().Gin();
    }
    public void BigGinUI()
    {
        GameObject.Find("PlayerHand").GetComponent<PlayerHand>().BigGin();
    }
    public void KnockUI()
    {
        GameObject.Find("PlayerHand").GetComponent<PlayerHand>().Knock();
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
        PlayerRoundDWPointsText.text = GameObject.Find("PlayerHand").GetComponent<PlayerHand>().DeadwoodPoints.ToString();
        AIRoundDWPointsText.text = GameObject.Find("AIHand").GetComponent<AIHand>().DeadwoodPoints.ToString();
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
        PlayerUIScoreText.text = "" + Match.instance.PlayerScore;
        AIUIScoreText.text = "" + Match.instance.AIScore;

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
