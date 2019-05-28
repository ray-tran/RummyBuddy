using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviousResults : MonoBehaviour
{
    public List<string> result;
    public Text display;
    public string getResult;

    // Start is called before the first frame update
    void Start()
    {
        result = Match.instance.ResultType;
    }

    // Update is called once per frame
    void Update()
    {
        Text roundresult = this.GetComponent<Text>();

        roundresult.text = "Previous Results: ";

        result = Match.instance.ResultType;

        foreach(var r in result)
        {
            getResult = getResult.ToString() + r.ToString() + "\n";  
        }

        roundresult.text = getResult;
        
    }
}
