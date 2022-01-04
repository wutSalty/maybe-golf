using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsManager : MonoBehaviour
{
    //Settings buttons
    public Dropdown WindowMode;
    public Dropdown WindowSize;
    public Dropdown InputType;
    public Slider Sensitivity;
    public Toggle DebugWindow;

    public GameObject ConfirmRevertScreen;
    public Text CountdownText;

    public Button TheConfirmButton;
    public Button TheRevertButton;

    public Selectable LastButtonSelected;

    public EventSystem eventSystem;

    [HideInInspector]
    public bool CurrentlyFullscreen; //true = yes its fullscreen. false = window mode.
    private int OldWindow;
    private int OldResolution;
    private int OldInputType;
    private float OldSensitivity;
    private bool OldDebugWindow;

    private bool ForcedOverride; //Makes sure the revert menu doesn't appear while resetting Display values

    //On start, check the saved values and set them to int
    private void Start()
    {
        OldWindow = PlayerPrefs.GetInt("WindowMode", 0);
        OldResolution = PlayerPrefs.GetInt("WindowSize", 0);
        OldInputType = PlayerPrefs.GetInt("InputType", 0);
        OldSensitivity = PlayerPrefs.GetFloat("Sensitivity", 4);
        OldDebugWindow = IntToBool(PlayerPrefs.GetInt("DebugWindow", 0));

        ForcedOverride = true;

        WindowMode.value = OldWindow;
        WindowSize.value = OldResolution;
        InputType.value = OldInputType;
        Sensitivity.value = OldSensitivity;
        DebugWindow.isOn = OldDebugWindow;

        ForcedOverride = false;

        RevertWindowed();
    }

    //Check the dropdown for window mode
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

    //Check the dropdown for resolution
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

    //public void UpdateInputType()
    //{
    //    PlayerPrefs.SetInt("InputType", InputType.value);
    //    PlayerPrefs.Save();

    //    GameManager.GM.gameObject.GetComponent<DebugLogCallbacks>().UpdatePlayPrefsText();
    //}

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
        Debug.Log("Debug viewer set to: " + value);
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

    //Countdown timer
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

    public IEnumerator WeirdOverride()
    {
        yield return new WaitForSeconds(0.05f);
        eventSystem.SetSelectedGameObject(null);
        TheConfirmButton.Select();
        eventSystem.firstSelectedGameObject = TheConfirmButton.gameObject;
    }

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
