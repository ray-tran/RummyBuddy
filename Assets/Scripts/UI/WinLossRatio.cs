using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinLossRatio : MonoBehaviour
{
    int wins;
    int losses;
    int wlratio;

    // Start is called before the first frame update
    void Start()
    {
        wins = Match.instance.PlayerWinCount;
        losses = Match.instance.PlayerLossCount;
    }

    // Update is called once per frame
    void Update()
    {
        wins = Match.instance.PlayerWinCount;
        losses = Match.instance.PlayerLossCount;

        wlratio = wins / losses;

        Text wl = this.GetComponent<Text>();

        wl.text = "Win/Loss Ratio: " + wlratio;
    }
}
