using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUI : MonoBehaviour 
{
    public static GameUI instance;
    public Text currentTurnText;

    private void Awake()
	{
        instance = this;
        GameObject scoreGO = GameObject.Find("CurrentTurn");
        currentTurnText = scoreGO.GetComponent<Text>();
        currentTurnText.text = "Current turn: PlayerDraw";

	}

	public void Shuffle()
	{
		if (Dealer.instance.DealInProgress == 0)
		{
            StartCoroutine(Dealer.instance.ShuffleCoroutine());
        }
	}
	
}
