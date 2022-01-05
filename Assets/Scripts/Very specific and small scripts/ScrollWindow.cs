using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollWindow : MonoBehaviour
{
    public Button LeftButton;
    public Button RightButton;
    public Image ImageWindow;
    public Sprite[] ListOfSprites;

    private int CurrentIndex = 0;

    public void OnLeftButton()
    {
        CurrentIndex = CurrentIndex - 1;
        if (CurrentIndex == -1)
        {
            CurrentIndex = ListOfSprites.Length - 1;
        }
        ImageWindow.sprite = ListOfSprites[CurrentIndex];
    }

    public void OnRightButton()
    {
        CurrentIndex = CurrentIndex + 1;
        if (CurrentIndex > (ListOfSprites.Length - 1))
        {
            CurrentIndex = 0;
        }
        ImageWindow.sprite = ListOfSprites[CurrentIndex];
    }
}
