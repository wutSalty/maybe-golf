using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class VerifySaveData : MonoBehaviour
{
    public UIManager uiManager;

    public GameObject ErrorCanvas;
    public Text ErrorText;
    public Button QuitBtn;

    public int CheckError = 0;

    private void Start()
    {
        //Checks if this is the first time the scene is loaded
        //If it is, then check the status of the save files
        //If the save data is valid, continue as normal. Or else, show error
        if (GameManager.GM.FirstLoaded)
        {
            CheckError = GameManager.GM.ErrorStatus;

            if (CheckError != 0)
            {
                ErrorHasOccurred();
            }
            else
            {
                GameManager.GM.FirstLoaded = false;
                ConfirmedToStart();
            }
        }
    }

    public void ConfirmedToStart()
    {
        uiManager.eventSystem.firstSelectedGameObject = uiManager.PlayButton.gameObject;
        AudioManager.instance.PlaySound("BGM_title");
    }

    public void ErrorHasOccurred()
    {
        switch (CheckError)
        {
            case 1:
                ErrorText.text = "ERR: Save Data seems to be corrupted";
                break;

            case 2:
                //do nothing
                break;

            default:
                ErrorText.text = "ERR: Unknown event occurred";
                break;
        }

        ErrorCanvas.SetActive(true);

        uiManager.eventSystem.SetSelectedGameObject(QuitBtn.gameObject);
        uiManager.eventSystem.firstSelectedGameObject = QuitBtn.gameObject;
    }

    public void ButtonQuit()
    {
        #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
        #else
                 Application.Quit();
        #endif
    }

    public void ButtonContinue()
    {
        GameManager.GM.SilentSave = true;
        GameManager.GM.SavePlayer();
        GameManager.GM.LoadPlayer();
        LoadingScreen.loadMan.BeginLoadingScene("MainMenu", false);
    }
}
