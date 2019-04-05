using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

Round:
- instance
- turn: Enum {PlayerDraw, PlayerDrawDiscard AI}
- drawPile: Stack <card>
- discardPile: Stack <card>
- playerHand: <hand>
- AIHand: <hand>
--
+ InitializeRound
+ UpdateTurn

*/

public enum Turn { PlayerDraw, PlayerDiscard, AI };

public class Round : MonoBehaviour
{
    public static Round instance;
    public Turn CurrentTurn;
    public CardSlot DiscardPile;
    public CardSlot DrawPile;
    private PlayerHand PlayerHand;
    private AIHand AIHand;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        PlayerHand = PlayerHand.instance;
        AIHand = AIHand.instance;

    }

    public void InitializeRound()
    {
        //reset round data
        UpdateTurn(Turn.PlayerDraw);
        
    }

    public void UpdateTurn(Turn newTurn)
    {
        CurrentTurn = newTurn;
        GameUI.instance.currentTurnText.text = "Current turn: " + newTurn.ToString();
    }

}
