using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Handles the skin window in options
public class ScrollWindow : MonoBehaviour
{
    //UI elements
    public Button LeftButton;
    public Button RightButton;
    public Image ImageWindow;

    //Keeping track of things
    private int CurrentIndex = 0;
    private Sprite[] ListOfSprites;

    //On start, grab all the sprites from GameManager, grab the player prefs, then show the proper ball
    private void Start()
    {
        ListOfSprites = GameManager.GM.BallSkins;

        CurrentIndex = GameManager.GM.BallSkin;
        ImageWindow.sprite = ListOfSprites[CurrentIndex];
    }

    //When the player goes left
    public void OnLeftButton()
    {
        CurrentIndex = CurrentIndex - 1;

        //Need to check whether the next item is locked or out of range
        while (CurrentIndex < 0 || GameManager.GM.UnlockedBallSkins[CurrentIndex] == false) 
        {
            CurrentIndex = CurrentIndex - 1;

            if (CurrentIndex < 0)
            {
                CurrentIndex = ListOfSprites.Length - 1;
            }
        }

        //Updates the image, saves the preference, updates debug stuff
        ImageWindow.sprite = ListOfSprites[CurrentIndex];
        GameManager.GM.BallSkin = CurrentIndex;
    }

    //When the player goes right
    public void OnRightButton()
    {
        CurrentIndex = CurrentIndex + 1;

        //Also checks whether the next item is locked out out of range
        while (CurrentIndex > (ListOfSprites.Length - 1) || GameManager.GM.UnlockedBallSkins[CurrentIndex] == false)
        {
            CurrentIndex = CurrentIndex + 1;

            if (CurrentIndex > (ListOfSprites.Length - 1))
            {
                CurrentIndex = 0;
            }
        }

        //Then saves everything and fixes things up as usual
        ImageWindow.sprite = ListOfSprites[CurrentIndex];
        GameManager.GM.BallSkin = CurrentIndex;
    }
}
