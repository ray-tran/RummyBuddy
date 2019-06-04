using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class GameButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite HighlightedImage;
    public Sprite NormalImage;
    public Button button;

    public void OnPointerEnter(PointerEventData eventData)
    {
        button.GetComponent<Image>().sprite = HighlightedImage;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        button.GetComponent<Image>().sprite = NormalImage;
    }


}
