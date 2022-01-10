using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Handles Level Select screen and determines which scene to load
public class LevelManager : MonoBehaviour
{
    List<LevelFormat> LevelList; //The list of levels copied from game manager

    public Button TutorialButton;

    //Copies data from GameManager
    private void Start()
    {
        LevelList = GameManager.GM.LevelData;
    }

    //Checks whether playing in Singleplayer or not. Determines whether Tutorial can be played or not
    private void OnEnable()
    {
        if (gameObject.activeSelf)
        {
            LevelList = GameManager.GM.LevelData;

            if (GameManager.GM.SingleMode)
            {
                TutorialButton.interactable = true;
            }
            else
            {
                TutorialButton.interactable = false;
            }
        }
    }

    //When button pressed, load into level
    public void LoadLevel(int LevelInt)
    {
        if (LevelInt == 0)
        {
            GameManager.GM.TutorialMode = true;
        } else
        {
            GameManager.GM.TutorialMode = false;
        }
        LoadingScreen.loadMan.BeginLoadingScene(LevelList[LevelInt].LevelName, true);
    }
}
