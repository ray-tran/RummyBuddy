using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUI : MonoBehaviour 
{
    public static GameUI instance;
    public Text currentTurnText;
    public Text currentDeadwoodPointsText;

    private void Awake()
	{
        instance = this;
        GameObject scoreGO = GameObject.Find("CurrentTurn");
        GameObject deadpointsGO = GameObject.Find("CurrentPoints");
        currentTurnText = scoreGO.GetComponent<Text>();
        currentTurnText.text = "Current turn: PlayerDraw";
        currentDeadwoodPointsText = deadpointsGO.GetComponent<Text>();


	}

	public void Shuffle()
	{
		if (Dealer.instance.DealInProgress == 0)
		{
            StartCoroutine(Dealer.instance.ShuffleCoroutine());
        }
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
    public void KnockUI()
    {
        PlayerHand.instance.Knock();
    }
	
}
