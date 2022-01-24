using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Handles most of the settings on the settings screen
public class SettingsManager : MonoBehaviour
{
    //Settings buttons
    public Dropdown WindowMode;
    public Dropdown WindowSize;
    public Dropdown InputType;
    public Slider Sensitivity;
    public Toggle DebugWindow;

    //UI element for delete button
    public GameObject DelPanel;
    public Button DelButton;
    public Text DelButtonTxt;
    public Text DelText;
    public Button DelCancel;
    public Text DelTextA;
    public Text DelTextB;
    public GameObject ReturnButton;
    public GameObject OriginalDeleteButton;

    //Elements for screen resolution confirm
    public GameObject ConfirmRevertScreen;
    public Text CountdownText;
    public Button TheConfirmButton;
    public Button TheRevertButton;

    //For UI funky business
    public Selectable LastButtonSelected;

    //To take over the selected object
    public EventSystem eventSystem;

    //Holds data loaded from prefs or if it needs to revert
    private bool CurrentlyFullscreen; //true = yes its fullscreen. false = window mode.
    private int OldWindow;
    private int OldResolution;
    private int OldInputType;
    private float OldSensitivity;
    private bool OldDebugWindow;

    private bool ForcedOverride; //Makes sure the revert menu doesn't appear while resetting Display values
    private int NumOfDelete = 0;

    //Default playerdata in-case data is reset or other
    public List<LevelFormat> DefaultLevelData;
    public bool[] DefaultUnlockables;

    //On start, check the saved values and set them to old
    private void Start()
    {
        OldWindow = PlayerPrefs.GetInt("WindowMode", 0);
        OldResolution = PlayerPrefs.GetInt("WindowSize", 0);
        OldInputType = PlayerPrefs.GetInt("InputType", 0);
        OldSensitivity = PlayerPrefs.GetFloat("Sensitivity", 4);
        OldDebugWindow = IntToBool(PlayerPrefs.GetInt("DebugWindow", 0));

        //Update the values of each setting
        ForcedOverride = true;

        WindowMode.value = OldWindow;
        WindowSize.value = OldResolution;
        InputType.value = OldInputType;
        Sensitivity.value = OldSensitivity;
        DebugWindow.isOn = OldDebugWindow;

        ForcedOverride = false;

        //Get the game to actually update the screen size
        RevertWindowed();
    }

    //Check the dropdown for window mode (fullscreen or not)
    public void CheckWindowed()
    {
        if (ForcedOverride == false)
        {
            LastButtonSelected = WindowMode;

            SetWindow(WindowMode.value);
            ConfirmRevertScreen.SetActive(true);
            StartCoroutine("StartCountdown", 10);

            StartCoroutine(WeirdOverride());
        }
    }

    //Check the dropdown for resolution (screen size)
    public void CheckResolution()
    {
        if (ForcedOverride == false)
        {
            LastButtonSelected = WindowSize;

            SetResolution(WindowSize.value);
            ConfirmRevertScreen.SetActive(true);
            StartCoroutine("StartCountdown", 10);

            StartCoroutine(WeirdOverride());
        }
    }

    //Check dropdown for input type
    public void CheckInputType()
    {
        PlayerPrefs.SetInt("InputType", InputType.value);
        PlayerPrefs.Save();

        GameManager.GM.gameObject.GetComponent<DebugLogCallbacks>().UpdatePlayPrefsText();
    }

    //Check slider for player's sensitivity in button controls
    public void CheckSensitivity(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
        PlayerPrefs.Save();

        GameManager.GM.gameObject.GetComponent<DebugLogCallbacks>().UpdatePlayPrefsText();
    }

    //Check toggle for debug menu
    public void CheckDebugWindow(bool value)
    {
        var bean = BoolToInt(value);

        PlayerPrefs.SetInt("DebugWindow", bean);
        GameManager.GM.gameObject.GetComponent<DebugLogCallbacks>().enabled = value;

        PlayerPrefs.Save();

        GameManager.GM.gameObject.GetComponent<DebugLogCallbacks>().UpdatePlayPrefsText();
        
        if (value)
        {
            Debug.Log("--Debug viewer now active--");
            Debug.Log("NOTE: This will output all developer messages.");
            Debug.Log("Some errors may appear in the log.");
            Debug.Log("There should be no need to worry about these.");
            Debug.Log("Please use this purely for your own curiosity");
            Debug.Log("If this message get cut, please change resolution");
        }
    }

    //Initiates delete sequence. Player must click button 3 times before data will be deleted
    public void DeleteButton()
    {
        switch (NumOfDelete)
        {
            case 0:
                NumOfDelete = 1;

                DelPanel.SetActive(true);

                eventSystem.SetSelectedGameObject(DelCancel.gameObject);
                DelText.text = "Are you sure?";
                DelButtonTxt.text = "Yes";

                StartCoroutine(FadeText(1, 1, 0));
                break;

            case 1:
                NumOfDelete = 2;

                DelText.text = "Really, really sure? (Last chance)";
                DelButtonTxt.text = "Yes, nuke it all";
                break;

            case 2:
                NumOfDelete = 0;

                DelButton.interactable = false;
                DelCancel.interactable = false;

                ReturnButton.SetActive(false);
                OriginalDeleteButton.SetActive(false);

                DelText.text = "Done. Game will reload in 3";
                DelButtonTxt.text = "Confirmed";
                StartCoroutine(DeleteCountdown());
                break;

            default:
                break;
        }
    }

    //The countdown for deleting and then the actual deleting
    IEnumerator DeleteCountdown()
    {
        int CountdownValue = 3;

        while (CountdownValue > 0)
        {
            yield return new WaitForSeconds(1f);
            CountdownValue -= 1;
            DelText.text = "Done. Game will reload in " + CountdownValue;
        }

        Debug.Log("Save data reset.");

        //Resets all preferences, resets player data, then reloads to main menu
        PlayerPrefs.SetInt("WindowMode", 0);
        PlayerPrefs.SetInt("WindowSize", 0);
        PlayerPrefs.SetInt("InputType", 0);
        PlayerPrefs.SetFloat("Sensitivity", 4);
        PlayerPrefs.SetInt("DebugWindow", 0);
        GameManager.GM.gameObject.GetComponent<DebugLogCallbacks>().UpdatePlayPrefsText();

        SetWindow(0);
        SetResolution(0);

        GameManager.GM.LevelData = DefaultLevelData;
        GameManager.GM.UnlockedBallSkins = DefaultUnlockables;
        GameManager.GM.TimesPlayedSolo = 0;
        GameManager.GM.TimesPlayedMulti = 0;
        GameManager.GM.BallSkin = 0;
        GameManager.GM.BossLevelUnlocked = false;
        GameManager.GM.CheckLocked();
        GameManager.GM.SavePlayer();

        LoadingScreen.loadMan.BeginLoadingScene("MainMenu", false);
    }

    //If the player cancels deleting data, reset the counter
    public void CancelDelete()
    {
        NumOfDelete = 0;
        DelPanel.SetActive(false);
        DelText.text = "Are you sure?";
        DelButtonTxt.text = "Yes";

        eventSystem.SetSelectedGameObject(WindowMode.gameObject);
    }

    //Text fade for the ominous delete message
    IEnumerator FadeText(float targetValue, float duration, float ogValue)
    {
        float startValue = ogValue;
        float time = 0;
        float alpha;

        while (time < duration)
        {
            alpha = Mathf.Lerp(startValue, targetValue, time / duration);

            DelTextA.color = new Color(DelTextA.color.r, DelTextA.color.g, DelTextA.color.b, alpha);
            DelTextB.color = new Color(DelTextB.color.r, DelTextB.color.g, DelTextB.color.b, alpha);

            time += Time.deltaTime;
            yield return null;
        }
        DelTextA.color = new Color(DelTextA.color.r, DelTextA.color.g, DelTextA.color.b, targetValue);
        DelTextB.color = new Color(DelTextB.color.r, DelTextB.color.g, DelTextB.color.b, targetValue);
    }

    //Reset windows to original values
    public void RevertWindowed()
    {
        SetWindow(OldWindow);
        SetResolution(OldResolution);
    }

    //Change window mode or window size
    public void SetWindow(int integer)
    {
        switch (integer)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                CurrentlyFullscreen = true;
                break;

            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                CurrentlyFullscreen = false;
                break;

            default:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }
    }

    public void SetResolution(int integer)
    {
        switch (integer)
        {
            case 0:
                Screen.SetResolution(1920, 1080, CurrentlyFullscreen);
                break;

            case 1:
                Screen.SetResolution(1600, 900, CurrentlyFullscreen);
                break;

            case 2:
                Screen.SetResolution(1280, 720, CurrentlyFullscreen);
                break;

            case 3:
                Screen.SetResolution(1024, 576, CurrentlyFullscreen);
                break;

            case 4:
                Screen.SetResolution(960, 540, CurrentlyFullscreen);
                break;

            case 5:
                Screen.SetResolution(800, 600, CurrentlyFullscreen);
                break;
        }
    }

    //Button business
    public void ConfirmButton()
    {
        StopCoroutine("StartCountdown");
        ConfirmRevertScreen.SetActive(false);

        OldWindow = WindowMode.value;
        OldResolution = WindowSize.value;

        PlayerPrefs.SetInt("WindowMode", WindowMode.value);
        PlayerPrefs.SetInt("WindowSize", WindowSize.value);
        PlayerPrefs.Save();

        GameManager.GM.gameObject.GetComponent<DebugLogCallbacks>().UpdatePlayPrefsText();

        LastButtonSelected.Select();
    }

    public void RevertButton()
    {
        StopCoroutine("StartCountdown");
        RevertWindowed();
        ConfirmRevertScreen.SetActive(false);

        ForcedOverride = true;

        WindowMode.value = OldWindow;
        WindowSize.value = OldResolution;

        ForcedOverride = false;

        LastButtonSelected.Select();
    }

    //Countdown timer before reverting screen options
    float currentCountdownValue;
    public IEnumerator StartCountdown(float countdownValue = 10)
    {
        EventSystem.current.SetSelectedGameObject(null);
        TheConfirmButton.Select();

        currentCountdownValue = countdownValue;

        while (currentCountdownValue >= 0)
        {
            CountdownText.text = "Display will revert in " + currentCountdownValue.ToString() + " seconds";
            yield return new WaitForSeconds(1f);
            currentCountdownValue--;

            if (currentCountdownValue == 0)
            {
                ForcedOverride = true;

                WindowMode.value = OldWindow;
                WindowSize.value = OldResolution;

                ForcedOverride = false;
                RevertWindowed();
                ConfirmRevertScreen.SetActive(false);
                Debug.Log("Co has been stopped");
                StopCoroutine("StartCountdown");

                LastButtonSelected.Select();
            }
        }
    }

    //To allow confirm button to be default when revert screen appears
    public IEnumerator WeirdOverride()
    {
        yield return new WaitForSeconds(0.05f);
        eventSystem.SetSelectedGameObject(null);
        TheConfirmButton.Select();
        eventSystem.firstSelectedGameObject = TheConfirmButton.gameObject;
    }

    //Converts boolean to int and int to boolean so that they can be saved in playerprefs
    int BoolToInt(bool val)
    {
        if (val)
            return 1;
        else
            return 0;
    }

    //0 is false
    bool IntToBool(int val)
    {
        if (val != 0)
            return true;
        else
            return false;
    }

}
