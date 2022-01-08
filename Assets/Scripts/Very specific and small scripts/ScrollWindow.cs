using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollWindow : MonoBehaviour
{
    public Button LeftButton;
    public Button RightButton;
    public Image ImageWindow;

    private int CurrentIndex = 0;
    private Sprite[] ListOfSprites;

    private void Start()
    {
        ListOfSprites = GameManager.GM.BallSkins;

        CurrentIndex = PlayerPrefs.GetInt("BallSkin", 0);
        ImageWindow.sprite = ListOfSprites[CurrentIndex];
    }

    public void OnLeftButton()
    {
        CurrentIndex = CurrentIndex - 1;

        while (CurrentIndex < 0 || GameManager.GM.UnlockedBallSkins[CurrentIndex] == false)
        {
            CurrentIndex = CurrentIndex - 1;

            if (CurrentIndex < 0)
            {
                CurrentIndex = ListOfSprites.Length - 1;
            }
        }

        ImageWindow.sprite = ListOfSprites[CurrentIndex];
        PlayerPrefs.SetInt("BallSkin", CurrentIndex);
        GameManager.GM.gameObject.GetComponent<DebugLogCallbacks>().UpdatePlayPrefsText();
    }

    public void OnRightButton()
    {
        CurrentIndex = CurrentIndex + 1;

        while (CurrentIndex > (ListOfSprites.Length - 1) || GameManager.GM.UnlockedBallSkins[CurrentIndex] == false)
        {
            CurrentIndex = CurrentIndex + 1;

            if (CurrentIndex > (ListOfSprites.Length - 1))
            {
                CurrentIndex = 0;
            }
        }

        ImageWindow.sprite = ListOfSprites[CurrentIndex];
        PlayerPrefs.SetInt("BallSkin", CurrentIndex);
        GameManager.GM.gameObject.GetComponent<DebugLogCallbacks>().UpdatePlayPrefsText();
    }
}
