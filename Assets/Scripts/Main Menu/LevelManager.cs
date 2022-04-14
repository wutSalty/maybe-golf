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

    public GameObject HowToBossPanel;
    public Button HowToBossButton;
    public Button HowToBossOK;

    public Text CurrentMode;

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
                CurrentMode.text = "Singleplayer Mode";
            }
            else
            {
                TutorialButton.interactable = false;
                CurrentMode.text = "Multiplayer Mode";
            }

            //If the Boss Level hasn't been unlocked yet, keep it tight lipped
            if (GameManager.GM.BossLevelUnlocked)
            {
                BossButton.gameObject.SetActive(true);
                HowToBossButton.gameObject.SetActive(true);

                LevelPanel.offsetMax = new Vector2(0, -70);
                LevelPanel.offsetMin = new Vector2(0, 115);
            }
            else
            {
                BossButton.gameObject.SetActive(false);
                HowToBossButton.gameObject.SetActive(false);

                LevelPanel.offsetMax = new Vector2(0, -70);
                LevelPanel.offsetMin = new Vector2(0, 15);
            }

            //If GhostMode is active, disable Boss Level as it also isn't supported in GhostMode
            if (GameManager.GM.GhostMode)
            {
                BossButton.interactable = false;
                CurrentMode.text = "Vs. Ghost Mode";
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
            AudioManager.instance.PlaySound("UI_beep");
        }
        else
        {
            GameManager.GM.TutorialMode = false;
            string BGM = "";
            bool Starter = true;

            switch (LevelInt)
            {
                case 0:
                    GameManager.GM.TutorialMode = true;
                    BGM = "BGM_tutorial";                
                    break;

                case 1:
                    BGM = "BGM_one";
                    break;

                case 2:
                    BGM = "BGM_two";
                    break;

                case 3:
                    BGM = "BGM_three";
                    break;

                case 4:
                    BGM = "BGM_four";
                    break;

                case 5:
                    LoadingScreen.loadMan.LoadingMusic(LevelList[LevelInt].LevelName, false, "BGM_boss");
                    AudioManager.instance.PlaySound("UI_confirm");
                    return;

                default:
                    break;
            }
            LoadingScreen.loadMan.LoadingMusic(LevelList[LevelInt].LevelName, Starter, BGM);
            AudioManager.instance.PlaySound("UI_confirm");
        }        
    }

    public void OKButton()
    {
        GameManager.GM.TutorialMode = false;
        string BGM = "";
        bool Starter = true;

        switch (TempLevelInt)
        {
            case 0:
                GameManager.GM.TutorialMode = true;
                BGM = "BGM_tutorial";
                break;

            case 1:
                BGM = "BGM_one";
                break;

            case 2:
                BGM = "BGM_two";
                break;

            case 3:
                BGM = "BGM_three";
                break;

            case 4:
                BGM = "BGM_four";
                break;

            case 5:
                LoadingScreen.loadMan.LoadingMusic(LevelList[TempLevelInt].LevelName, false, "BGM_boss");
                AudioManager.instance.PlaySound("UI_confirm");
                return;

            default:
                break;
        }
        LoadingScreen.loadMan.LoadingMusic(LevelList[TempLevelInt].LevelName, Starter, BGM);
        AudioManager.instance.PlaySound("UI_confirm");
    }

    public void CancelButton()
    {
        GhostWarningPanel.SetActive(false);

        eventSystem.SetSelectedGameObject(LastSelected);
        eventSystem.firstSelectedGameObject = LastSelected;
        AudioManager.instance.PlaySound("UI_beep");
    }

    public void HowToBoss()
    {
        HowToBossPanel.SetActive(true);
        eventSystem.firstSelectedGameObject = HowToBossOK.gameObject;
        eventSystem.SetSelectedGameObject(HowToBossOK.gameObject);
        AudioManager.instance.PlaySound("UI_beep");
    }

    public void HowBossOK()
    {
        HowToBossPanel.SetActive(false);
        eventSystem.firstSelectedGameObject = HowToBossButton.gameObject;
        eventSystem.SetSelectedGameObject(HowToBossButton.gameObject);
        AudioManager.instance.PlaySound("UI_beep");
    }
}
