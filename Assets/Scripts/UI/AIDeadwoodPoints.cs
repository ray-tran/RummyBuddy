using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIDeadwoodPoints : MonoBehaviour
{
    public int aiDeadwoodPoints;
    // Start is called before the first frame update
    void Start()
    {
        aiDeadwoodPoints = AIHand.instance.DeadwoodPoints;
        
    }

    // Update is called once per frame
    void Update()
    {
        aiDeadwoodPoints = AIHand.instance.DeadwoodPoints;

        Text aiscore = this.GetComponent<Text>();
        aiscore.text = "AI Deadwood Points: " + aiDeadwoodPoints;        
    }
}
