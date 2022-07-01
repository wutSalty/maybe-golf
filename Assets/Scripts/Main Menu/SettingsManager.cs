using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

//Handles most of the settings on the settings screen
public class SettingsManager : MonoBehaviour
{
    public Animator anim;
    private int CurrentSlide = 1;

    public Text NavCentre;
    public Text NavLeft;
    public Text NavRight;

    public Button NavButtonLeft;
    public Text NavButtonLeftText;
    public Button NavButtonRight;
    public Text NavButtonRightText;

    //Settings buttons
    public Dropdown WindowMode;
    public Dropdown WindowSize;
    public Dropdown InputType;
    public Slider Sensitivity;
    public Toggle DebugWindow;
    public Slider BGMSlider;
    public Slider UISlider;
    public Slider InGameSlider;
    public Slider ColourPickerSlider;
    public GameObject ColourPickerObj;
    public GameObject ShuffleGameplayObj;
    public Toggle ReduceMotion;
    public Toggle CRTSelect;

    public Text DemoColour;

    //UI element for delete button
    public GameObject DelPanel;
    public Button DelButton;
    public Text DelButtonTxt;
    public Text DelText;
    public Button DelCancel;
    public Text DelTextA;
    public Text DelTextB;
    public GameObject ReturnButton;
    public Button ReturnButtonButton;
    public GameObject OriginalDeleteButton;

    //Elements for screen resolution confirm
    public GameObject ConfirmRevertScreen;
    public Text CountdownText;
    public Button TheConfirmButton;
    public Button TheRevertButton;

    //Elements for Credits
    public GameObject CreditPanel;
    public Text CreditText;
    public Button CreditButton;
    public Button ReturnCredit;

    //Elements for Attributes
    public GameObject AttributePanel;
    public Button AttributeButton;
    public Button ReturnAttribute;

    //For UI funky business
    private Selectable LastButtonSelected;

    public UIManager uiManager;

    //To take over the selected object
    public EventSystem eventSystem;
    public OverrideRenderPipeline OverrideRenderPipeline;

    //Holds data loaded from prefs or if it needs to revert
    private bool CurrentlyFullscreen; //true = yes its fullscreen. false = window mode.
    private int OldWindow;
    private int OldResolution;
    private int OldInputType;
    private float OldSensitivity;
    private bool OldDebugWindow;
    private float OldBGM;
    private float OldUI;
    private float OldInGame;
    private float OldColourPicker;
    private bool OldReduceMotion;
    private bool OldCRT;

    private bool ForcedOverride; //Makes sure the revert menu doesn't appear while resetting Display values
    private int NumOfDelete = 0; //Keeps track of delete

    //Default playerdata in-case data is reset or other
    public List<LevelFormat> DefaultLevelData;
    public bool[] DefaultUnlockables;

    //Player data that has already been fully unlocked
    public List<LevelFormat> FullLevelData;
    public bool[] FullUnlockables;

    private Coroutine ScreenCheck;
    private Coroutine textScroll;

    //[HideInInspector]
    public bool HoldingDown = false;

    private Navigation ZeroUp = new Navigation() { mode = Navigation.Mode.Explicit };
    private Navigation OneUp = new Navigation() { mode = Navigation.Mode.Explicit };
    private Navigation TwoUp = new Navigation() { mode = Navigation.Mode.Explicit };
    private Navigation ThreeUp = new Navigation() { mode = Navigation.Mode.Explicit };

    //On start, check the saved values and set them to old
    private void Start()
    {
        OldWindow = PlayerPrefs.GetInt("WindowMode", 0);
        OldResolution = PlayerPrefs.GetInt("WindowSize", 0);
        OldInputType = PlayerPrefs.GetInt("InputType", 0);
        OldSensitivity = PlayerPrefs.GetFloat("Sensitivity", 4);
        OldDebugWindow = IntToBool(PlayerPrefs.GetInt("DebugWindow", 0));
        OldBGM = PlayerPrefs.GetFloat("BGM", 5f);
        OldUI = PlayerPrefs.GetFloat("UI", 5f);
        OldInGame = PlayerPrefs.GetFloat("InGame", 5f);
        OldColourPicker = GameManager.GM.SparkleColour;
        OldReduceMotion = IntToBool(PlayerPrefs.GetInt("ReduceMotion", 0));
        OldCRT = IntToBool(PlayerPrefs.GetInt("CRT", 0));

        //Update the values of each setting
        ForcedOverride = true;

        WindowMode.value = OldWindow;
        WindowSize.value = OldResolution;
        InputType.value = OldInputType;
        Sensitivity.value = OldSensitivity;
        DebugWindow.isOn = OldDebugWindow;
        BGMSlider.value = OldBGM;
        UISlider.value = OldUI;
        InGameSlider.value = OldInGame;
        ColourPickerSlider.value = OldColourPicker;
        DemoColour.color = Color.HSVToRGB(OldColourPicker, 0.75f, 1f);
        ReduceMotion.isOn = OldReduceMotion;
        CRTSelect.isOn = OldCRT;

        ForcedOverride = false;

        //Get the game to actually update the screen size
        RevertWindowed();

        if (GameManager.GM.FullCleared)
        {
            ShuffleGameplayObj.transform.localPosition = new Vector3(0, 0, 0);
            ColourPickerObj.SetActive(true);
        }
        else
        {
            ShuffleGameplayObj.transform.localPosition = new Vector3(0, 25, 0);
            ColourPickerObj.SetActive(false);
        }

        ReturnButtonButton = ReturnButton.GetComponent<Button>();
        ZeroUp.selectOnUp = CRTSelect;
        OneUp.selectOnUp = Sensitivity;
        TwoUp.selectOnUp = InGameSlider;
        ThreeUp.selectOnUp = OriginalDeleteButton.GetComponent<Button>();
    }

    //Check the dropdown for window mode (fullscreen or not)
    public void CheckWindowed()
    {
        if (ForcedOverride == false)
        {
            LastButtonSelected = WindowMode;

            SetWindow(WindowMode.value);
            ConfirmRevertScreen.SetActive(true);
            ScreenCheck = StartCoroutine(StartCountdown(10));

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
            ScreenCheck = StartCoroutine(StartCountdown(10));

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
                AudioManager.instance.PlaySound("UI_beep");
                break;

            case 1:
                NumOfDelete = 2;

                DelText.text = "Really, really sure? (Last chance)";
                DelButtonTxt.text = "Yes, nuke it all";
                AudioManager.instance.PlaySound("UI_beep");
                break;

            case 2:
                NumOfDelete = 0;

                eventSystem.SetSelectedGameObject(DelCancel.gameObject);
                eventSystem.firstSelectedGameObject = null;

                DelButton.interactable = false;
                DelCancel.interactable = false;

                ReturnButton.SetActive(false);
                OriginalDeleteButton.SetActive(false);

                DelText.text = "Done. Game will reload in 3";
                DelButtonTxt.text = "Confirmed";
                StartCoroutine(DeleteCountdown());
                AudioManager.instance.PlaySound("UI_confirm");
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
        PlayerPrefs.SetFloat("BGM", 5f);
        PlayerPrefs.SetFloat("UI", 5f);
        PlayerPrefs.SetFloat("InGame", 5f);
        PlayerPrefs.SetInt("ReduceMotion", 0);
        PlayerPrefs.SetInt("CRT", 0);
        GameManager.GM.gameObject.GetComponent<DebugLogCallbacks>().UpdatePlayPrefsText();

        SetWindow(0);
        SetResolution(0);
        OverrideRenderPipeline.SwitchToDefault();

        //foreach (Sound s in AudioManager.instance.sounds)
        //{
        //    s.source.volume = 5f / 10f;
        //}

        if (!HoldingDown)
        {
            GameManager.GM.LevelData = DefaultLevelData;
            GameManager.GM.UnlockedBallSkins = DefaultUnlockables;
            GameManager.GM.TimesPlayedSolo = 0;
            GameManager.GM.TimesPlayedMulti = 0;
            GameManager.GM.BallSkin = 0;
            GameManager.GM.BossLevelUnlocked = false;
            GameManager.GM.FullCleared = false;
            GameManager.GM.SparkleColour = 0;
            GameManager.GM.CheckLocked();
            GameManager.GM.SavePlayer();
        } else
        {
            GameManager.GM.LevelData = FullLevelData;
            GameManager.GM.UnlockedBallSkins = FullUnlockables;
            GameManager.GM.TimesPlayedSolo = 0;
            GameManager.GM.TimesPlayedMulti = 0;
            GameManager.GM.BallSkin = 0;
            GameManager.GM.BossLevelUnlocked = true;
            GameManager.GM.FullCleared = true;
            GameManager.GM.SparkleColour = 0;
            GameManager.GM.CheckLocked();
            GameManager.GM.SavePlayer();
        }

        LoadingScreen.loadMan.BeginLoadingScene("MainMenu", false);
    }

    //If the player cancels deleting data, reset the counter
    public void CancelDelete()
    {
        NumOfDelete = 0;
        DelPanel.SetActive(false);
        DelText.text = "Are you sure?";
        DelButtonTxt.text = "Yes";

        eventSystem.SetSelectedGameObject(OriginalDeleteButton);
        AudioManager.instance.PlaySound("UI_beep");
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

    //Confirming display options buttons
    public void ConfirmButton()
    {
        StopCoroutine(ScreenCheck);
        ConfirmRevertScreen.SetActive(false);

        OldWindow = WindowMode.value;
        OldResolution = WindowSize.value;

        PlayerPrefs.SetInt("WindowMode", WindowMode.value);
        PlayerPrefs.SetInt("WindowSize", WindowSize.value);
        PlayerPrefs.Save();

        GameManager.GM.gameObject.GetComponent<DebugLogCallbacks>().UpdatePlayPrefsText();

        LastButtonSelected.Select();
        AudioManager.instance.PlaySound("UI_beep");
    }

    public void RevertButton()
    {
        StopCoroutine(ScreenCheck);
        RevertWindowed();
        ConfirmRevertScreen.SetActive(false);

        ForcedOverride = true;

        WindowMode.value = OldWindow;
        WindowSize.value = OldResolution;

        ForcedOverride = false;

        LastButtonSelected.Select();
        AudioManager.instance.PlaySound("UI_beep");
    }

    //Audio stuff
    public void AdjustBGM(float value)
    {
        foreach (Sound s in AudioManager.instance.sounds)
        {
            if (s.audioPurpose == 0) //0 = BGM
            {
                s.source.volume = value / 10;
            }
        }
        PlayerPrefs.SetFloat("BGM", value);
        PlayerPrefs.Save();
    }

    public void AdjustUI(float value)
    {
        foreach (Sound s in AudioManager.instance.sounds)
        {
            if (s.audioPurpose == 1) //1 = UI sounds
            {
                s.source.volume = value / 10;
            }
        }
        PlayerPrefs.SetFloat("UI", value);
        PlayerPrefs.Save();
        if (!ForcedOverride)
        {
            AudioManager.instance.PlaySound("UI_beep");
        }
    }

    public void AdjustInGame(float value)
    {
        foreach (Sound s in AudioManager.instance.sounds)
        {
            if (s.audioPurpose == 2) //2 = InGame sounds
            {
                s.source.volume = value / 10;
            }
        }
        PlayerPrefs.SetFloat("InGame", value);
        PlayerPrefs.Save();
        if (!ForcedOverride)
        {
            AudioManager.instance.PlaySound("IG_golfhit");
        }
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
                StopCoroutine(ScreenCheck);

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

    //Credits stuff
    public void CreditsButton()
    {
        AudioManager.instance.PlaySound("UI_beep");
        CreditPanel.SetActive(true);
        eventSystem.firstSelectedGameObject = ReturnCredit.gameObject;
        eventSystem.SetSelectedGameObject(ReturnCredit.gameObject);

        AudioManager.instance.StopPlaying("BGM_title");
        AudioManager.instance.PlaySound("BGM_credits");
        textScroll = StartCoroutine(ScrollCredits());
    }

    // Closing credits
    public void ReturnFromCredits()
    {
        AudioManager.instance.PlaySound("UI_beep");
        CreditPanel.SetActive(false);
        eventSystem.firstSelectedGameObject = CreditButton.gameObject;
        eventSystem.SetSelectedGameObject(CreditButton.gameObject);

        if (textScroll != null)
        {
            StopCoroutine(textScroll);
        }
        AudioManager.instance.StopPlaying("BGM_credits");
        AudioManager.instance.PlaySound("BGM_title");
    }

    //Scrolling credits
    IEnumerator ScrollCredits()
    {
        float time = 0;
        float duration = 31.8f;
        float oldValue = -609f;
        float newValue;
        float lerping;

        double aspect = (Screen.width * 1.0) / Screen.height;
        if (aspect == (16 * 1.0) / 9)
        {
            newValue = 635f;
        }
        else
        {
            newValue = 690f;
        }

        while (time < duration)
        {
            lerping = Mathf.Lerp(oldValue, newValue, time / duration);
            CreditText.rectTransform.anchoredPosition = new Vector2(0, lerping);
            time += Time.deltaTime;
            yield return null;
        }

        CreditText.rectTransform.anchoredPosition = new Vector2(0, newValue);
    }

    //Attribute stuff
    public void OpenAttributes()
    {
        AudioManager.instance.PlaySound("UI_beep");
        AttributePanel.SetActive(true);
        eventSystem.firstSelectedGameObject = ReturnAttribute.gameObject;
        eventSystem.SetSelectedGameObject(ReturnAttribute.gameObject);
    }

    public void CloseAttributes()
    {
        AudioManager.instance.PlaySound("UI_beep");
        AttributePanel.SetActive(false);
        eventSystem.firstSelectedGameObject = AttributeButton.gameObject;
        eventSystem.SetSelectedGameObject(AttributeButton.gameObject);
    }

    //Colour picker
    public void AdjustColourPicker(float value)
    {
        DemoColour.color = Color.HSVToRGB(value, 0.75f, 1f);

        GameManager.GM.SparkleColour = value;
    }

    //Reduced Motion
    public void ToggleReduceMotion(bool value)
    {
        if (value)
        {
            LoadingScreen.loadMan.SetReducedMotion();
            GameManager.GM.mat.DisableKeyword("BOOLEAN_B653FFBD9C0241A680FF0CEADD5501B6_ON");
            GameManager.GM.mat.DisableKeyword("BOOLEAN_4E2168F80EE34B66B671D45E9D0775A0_ON");

        } else
        {
            LoadingScreen.loadMan.SetNormal();
            GameManager.GM.mat.EnableKeyword("BOOLEAN_B653FFBD9C0241A680FF0CEADD5501B6_ON");
            GameManager.GM.mat.EnableKeyword("BOOLEAN_4E2168F80EE34B66B671D45E9D0775A0_ON");
        }

        PlayerPrefs.SetInt("ReduceMotion", BoolToInt(value));
        PlayerPrefs.Save();

        GameManager.GM.gameObject.GetComponent<DebugLogCallbacks>().UpdatePlayPrefsText();
    }

    //CRT Screen Effect
    public void ToggleCRTEffect(bool value)
    {
        if (value)
        {
            OverrideRenderPipeline.SwitchToCRT();
        } else
        {
            OverrideRenderPipeline.SwitchToDefault();
        }

        PlayerPrefs.SetInt("CRT", BoolToInt(value));
        PlayerPrefs.Save();

        GameManager.GM.gameObject.GetComponent<DebugLogCallbacks>().UpdatePlayPrefsText();
    }

    public void ButtonPanelGoRight(UnityEngine.InputSystem.InputAction.CallbackContext value)
    {
        if (value.canceled)
        {
            PanelGoRight();
        }
    }

    public void ButtonPanelGoLeft(UnityEngine.InputSystem.InputAction.CallbackContext value)
    {
        if (value.canceled)
        {
            PanelGoLeft();
        }
    }

    public void PanelGoRight()
    {
        if (CurrentSlide == 0 || !uiManager.Settings.activeSelf)
        {
            return;
        }

        CurrentSlide -= 1;
        anim.SetInteger("currentslide", CurrentSlide);

        SwitchPanelsMaster();
    }

    public void PanelGoLeft()
    {
        if (CurrentSlide == 3 || !uiManager.Settings.activeSelf)
        {
            return;
        }

        CurrentSlide += 1;
        anim.SetInteger("currentslide", CurrentSlide);

        SwitchPanelsMaster();
    }

    private void SwitchPanelsMaster()
    {
        AudioManager.instance.PlaySound("UI_beep");

        switch (CurrentSlide)
        {
            case 0:
                NavCentre.text = "Graphics";
                NavLeft.text = "";
                NavRight.text = "Gameplay";
                ReturnButtonButton.navigation = ZeroUp;

                NavButtonLeft.interactable = false;
                NavButtonRight.interactable = true;

                if (!eventSystem.firstSelectedGameObject.CompareTag("ReturnButton"))
                {
                    eventSystem.firstSelectedGameObject = WindowMode.gameObject;
                    eventSystem.SetSelectedGameObject(WindowMode.gameObject);
                }

                break;

            case 1:
                NavCentre.text = "Gameplay";
                NavLeft.text = "Graphics";
                NavRight.text = "Audio";
                ReturnButtonButton.navigation = OneUp;

                NavButtonLeft.interactable = true;
                NavButtonRight.interactable = true;

                if (!eventSystem.firstSelectedGameObject.CompareTag("ReturnButton"))
                {
                    eventSystem.firstSelectedGameObject = InputType.gameObject;
                    eventSystem.SetSelectedGameObject(InputType.gameObject);
                }

                break;

            case 2:
                NavCentre.text = "Audio";
                NavLeft.text = "Gameplay";
                NavRight.text = "Other";
                ReturnButtonButton.navigation = TwoUp;

                NavButtonLeft.interactable = true;
                NavButtonRight.interactable = true;

                if (!eventSystem.firstSelectedGameObject.CompareTag("ReturnButton"))
                {
                    eventSystem.firstSelectedGameObject = BGMSlider.gameObject;
                    eventSystem.SetSelectedGameObject(BGMSlider.gameObject);
                }
                break;

            case 3:
                NavCentre.text = "Other";
                NavLeft.text = "Audio";
                NavRight.text = "";
                ReturnButtonButton.navigation = ThreeUp;

                NavButtonLeft.interactable = true;
                NavButtonRight.interactable = false;

                if (!eventSystem.firstSelectedGameObject.CompareTag("ReturnButton"))
                {
                    eventSystem.firstSelectedGameObject = DebugWindow.gameObject;
                    eventSystem.SetSelectedGameObject(DebugWindow.gameObject);
                }
                break;

            default:
                NavCentre.text = "Broke.";
                NavLeft.text = "Something";
                NavRight.text = "Whoops";
                break;
        }
    }

    public void ResetScreens()
    {
        CurrentSlide = 1;
        NavCentre.text = "Gameplay";
        NavLeft.text = "Graphics";
        NavRight.text = "Audio";
        ReturnButtonButton.navigation = ZeroUp;

        NavButtonLeft.interactable = true;
        NavButtonRight.interactable = true;
    }

    public void ControlsChangesEvent(PlayerInput input)
    {
        if (input.currentControlScheme == "Controller")
        {
            NavButtonLeftText.text = "<LB";
            NavButtonRightText.text = "RB>";
        }
        else
        {
            NavButtonLeftText.text = "<Z";
            NavButtonRightText.text = "X>";
        }
    }
}
