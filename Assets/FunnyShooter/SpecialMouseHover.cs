using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpecialMouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler
{
    public EventSystem eventSystem;
    public Text AboutText;
    [TextArea(2, 5)]
    public string WhatShouldTheTextSay;
    [TextArea(2, 5)]
    public string DefaultText = "";

    public void Update()
    {
        if (eventSystem.currentSelectedGameObject == gameObject)
        {
            AboutText.text = WhatShouldTheTextSay;
        } 
        else if (eventSystem.firstSelectedGameObject == null)
        {
            AboutText.text = DefaultText;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AboutText.text = WhatShouldTheTextSay;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AboutText.text = DefaultText;
    }

    public void OnDeselect(BaseEventData baseEvent)
    {
        AboutText.text = DefaultText;
    }
}
