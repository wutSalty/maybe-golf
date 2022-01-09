using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    List<LevelFormat> LevelList;

    public Button TutorialButton;

    private void Start()
    {
        LevelList = GameManager.GM.LevelData;
    }

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
