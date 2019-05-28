using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    public int playerDeadwoodPoints;
    // Start is called before the first frame update
    void Start()
    {
        playerDeadwoodPoints = PlayerHand.instance.DeadwoodPoints;        
    }

    // Update is called once per frame
    void Update()
    {
        Text score = this.GetComponent<Text>();
        score.text = "Deadwood: " + playerDeadwoodPoints;
    }
}
