using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentTrend : MonoBehaviour
{
    //public List<List<string>> currentTrend;
    int currentTrend;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //currentTrend = Match.instance.RoundResult;

        Text display = this.GetComponent<Text>();

        if (GameUI.instance.endMatch == true)
        {
           

        }
        
    }
}
