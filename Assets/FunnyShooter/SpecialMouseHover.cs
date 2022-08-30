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
    public Animator textAnim;

    private Selectable selectable;

    private void Start()
    {
        selectable = GetComponent<Selectable>();
    }

    public void Update()
    {
        if (eventSystem.currentSelectedGameObject == gameObject)
        {
            AboutText.text = WhatShouldTheTextSay;
            textAnim.Play("Text_Open");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        selectable.Select();
        EventSystem.current.firstSelectedGameObject = gameObject;

        AboutText.text = WhatShouldTheTextSay;
        textAnim.Play("Text_Open");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);

        //AboutText.text = DefaultText;
        textAnim.Play("Text_Close");
    }

    public void OnDeselect(BaseEventData baseEvent)
    {
        //AboutText.text = DefaultText;
        textAnim.Play("Text_Close");
    }
}
