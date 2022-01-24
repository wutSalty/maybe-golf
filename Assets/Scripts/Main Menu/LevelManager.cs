using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Handles Level Select screen and determines which scene to load
public class LevelManager : MonoBehaviour
{
    List<LevelFormat> LevelList; //The list of levels copied from game manager

    public RectTransform LevelPanel;

    public Button TutorialButton;
    public Button BossButton;

    public Button OKGhost;
    public GameObject GhostWarningPanel;
    public EventSystem eventSystem;

    private int TempLevelInt;
    private GameObject LastSelected;

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

            //If in multiplayer mode, disable the tutorial as it isn't supported in multiplayer
            if (GameManager.GM.SingleMode)
            {
                TutorialButton.interactable = true;
            }
            else
            {
                TutorialButton.interactable = false;
            }

            //If the Boss Level hasn't been unlocked yet, keep it tight lipped
            if (GameManager.GM.BossLevelUnlocked)
            {
                BossButton.gameObject.SetActive(true);

                LevelPanel.offsetMax = new Vector2(0, -70);
                LevelPanel.offsetMin = new Vector2(0, 115);
            }
            else
            {
                BossButton.gameObject.SetActive(false);
                LevelPanel.offsetMax = new Vector2(0, -70);
                LevelPanel.offsetMin = new Vector2(0, 15);
            }

            //If GhostMode is active, disable Boss Level as it also isn't supported in GhostMode
            if (GameManager.GM.GhostMode)
            {
                BossButton.interactable = false;
            }
            else
            {
                BossButton.interactable = true;
            }
        }
    }

    //When button pressed, load into level
    public void LoadLevel(int LevelInt)
    {
        if (GameManager.GM.GhostMode && GameManager.GM.LevelData[LevelInt].ghostData.Count == 0)
        {
            TempLevelInt = LevelInt;

            GhostWarningPanel.SetActive(true);

            LastSelected = eventSystem.currentSelectedGameObject;

            eventSystem.SetSelectedGameObject(OKGhost.gameObject);
            eventSystem.firstSelectedGameObject = OKGhost.gameObject;
        }
        else
        {
            if (LevelInt == 0)
            {
                GameManager.GM.TutorialMode = true;
            }
            else
            {
                GameManager.GM.TutorialMode = false;
            }

            if (LevelInt == 5)
            {
                LoadingScreen.loadMan.BeginLoadingScene(LevelList[LevelInt].LevelName, false);
            }
            else
            {
                LoadingScreen.loadMan.BeginLoadingScene(LevelList[LevelInt].LevelName, true);
            }
        }        
    }

    public void OKButton()
    {
        if (TempLevelInt == 0)
        {
            GameManager.GM.TutorialMode = true;
        }
        else
        {
            GameManager.GM.TutorialMode = false;
        }

        if (TempLevelInt == 5)
        {
            LoadingScreen.loadMan.BeginLoadingScene(LevelList[TempLevelInt].LevelName, false);
        }
        else
        {
            LoadingScreen.loadMan.BeginLoadingScene(LevelList[TempLevelInt].LevelName, true);
        }
    }

    public void CancelButton()
    {
        GhostWarningPanel.SetActive(false);

        eventSystem.SetSelectedGameObject(LastSelected);
        eventSystem.firstSelectedGameObject = LastSelected;
    }
}
