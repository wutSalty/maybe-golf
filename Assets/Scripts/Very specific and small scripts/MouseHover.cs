using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler
{
    public EventSystem eventSystem;
    public Text AboutText;
    [TextArea(2, 5)]
    public string WhatShouldTheTextSay;
    [TextArea(2, 5)]
    public string DefaultText = "Welcome to Golf, enjoy your stay!";

    public void Update()
    {
        if (eventSystem.currentSelectedGameObject == gameObject)
        {
            AboutText.text = WhatShouldTheTextSay;
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

    public void ForceDeselect()
    {
        AboutText.text = DefaultText;
    }
}
