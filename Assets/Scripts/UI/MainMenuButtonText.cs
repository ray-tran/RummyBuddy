using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuButtonText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Text theText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        theText.color = new Color32(255, 94, 150, 255); //Or however you do your color
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        theText.color = new Color32(255, 255, 167, 255); //Or however you do your color
    }
}
