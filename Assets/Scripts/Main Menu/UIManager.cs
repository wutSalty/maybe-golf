using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEditor;

//Handles Main Menu and other wacky UI business
public class UIManager : MonoBehaviour
{
    [Header("All Screen Objects")]
    //The different screens
    public GameObject MainMenu; //Main Menu object
    public GameObject DevMsgPanel;
    public GameObject HelpPanel;
    public GameObject MultiplayerSelect; //Multiplayer controller screen
    public GameObject Settings; //Settings object
    public GameObject ConfirmRevertScreen; //Confirm Reject screen object
    public GameObject DeleteConfirmScreen; //Setting's delete everything screen
    public GameObject CreditsScreen;
    public GameObject AttributeScreen;
    public GameObject UpdaterPanel;
    public GameObject UpdatingScreen;

    public GameObject LevelSelectScreen; //Level select screen
    public GameObject LevelSelectGhostPanel;
    public GameObject LevelSelectHowToBoss;

    public GameObject RecordsScreen; //Records screen
    public GameObject ScrollStoryScreen;

    [Header("Main Screen Transforms")]
    public RectTransform MainMenuRect;
    public RectTransform SettingsRect;
    public RectTransform RecordsRect;
    public RectTransform MultiplayerRect;
    public RectTransform LevelSelectRect;

    [Header("Main Menu Buttons")]
    //Main Menu Buttons
    public Button PlayButton;
    public Button PlayGhostButton;
    public Button TwoPlayerBtn;
    public Button SettingsButton;
    public Button RecordButton;
    public Button QuitButton;
    public Button DevMsgButton;
    public Button HideUIButton;
    public Button ReturnUIButton;
    public Button HelpButton;
    public Button UpdaterButton;

    [Header("First Selected Buttons")]
    //The buttons that should be selected when menus are accessed
    public Selectable SettingsFirstButton;
    public Selectable MultiplayerFirstButton;
    public Selectable LevelSelectFirstButton;
    public Selectable RecordsFirstButton;
    public Selectable DevMsgFirstButton;
    public Selectable HelpFirstButton;
    public Selectable UpdaterFirstButton;

    [Header("Managers")]
    //Any event or input system
    public EventSystem eventSystem;
    public PlayerInput inputSystem;
    public PlayerInputManager inputManager;

    [Header("Scripts")]
    //Scripts for interfacing
    public SettingsManager settingsManager;
    public MultiplayerSelect MultiSelectScript;
    public LevelManager levelManager;
    public RecordsManager recordManager;
    public UpdateManager updateManager;

    [Header("Skybox and Light")]
    //Skybox for full clears
    public Material notFullSky;
    public Material fullSky;

    public Light dirLight;

    [HideInInspector]
    public Vector2 LeftMove;

    [HideInInspector]
    public Vector2 CurserPos;

    public bool EnableFunny = false;

    public void GetFunnyInput(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            EnableFunny = true;
        }
        else if (value.canceled)
        {
            EnableFunny = false;
        }
    }

    public void GetCurserPos(InputAction.CallbackContext value)
    {
        CurserPos = value.ReadValue<Vector2>();
    }

    //Gets value of movement stick
    public void UpDownLeftRight(InputAction.CallbackContext value)
    {
        LeftMove = value.ReadValue<Vector2>();
    }

    //If button is pressed, make sure currentSelected isn't null
    public void EnterController(InputAction.CallbackContext value)
    {
        if (eventSystem.currentSelectedGameObject != null)
        {
            eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
        }
    }

    public void EscPressed(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            BackingOut();
        }
    }

    public void BackingOut()
    {
        if (MainMenu.activeSelf)
        {
            //Dev message panel
            if (DevMsgPanel.activeSelf)
            {
                ReturnDevMsg();
            }
            else if (HelpPanel.activeSelf)
            {
                CloseHelpMenu();
            } 
            else if (UpdaterPanel.activeSelf)
            {
                CloseUpdaterMenu();
            }
        }
        else if (MultiplayerSelect.activeSelf)
        {
            ReturnMainFromMulti();
        }
        else if (Settings.activeSelf) //If it's the settings
        {
            if (ConfirmRevertScreen.activeSelf) //Check the res menu isn't open. if it is, cancel the res screen instead
            {
                settingsManager.RevertButton();
            }
            else if (DeleteConfirmScreen.activeSelf) //Or else, check it isn't the delete screen. Cancel the delete instead
            {
                settingsManager.CancelDelete();
            }
            else if (CreditsScreen.activeSelf)
            {
                settingsManager.ReturnFromCredits();
            }
            else if (AttributeScreen.activeSelf)
            {
                settingsManager.CloseAttributes();
            }
            else //If none those screens are open
            {
                if (settingsManager.WindowMode.gameObject.transform.childCount == 3 && settingsManager.WindowSize.gameObject.transform.childCount == 3 && settingsManager.InputType.gameObject.transform.childCount == 3) //Make sure a dropdown isn't open
                {
                    PressReturnToMain(); //Then exit the screen
                }
            }
        }
        else if (RecordsScreen.activeSelf)
        {
            if (ScrollStoryScreen.activeSelf)
            {
                recordManager.ClickCloseScroll();
            }
            else
            {
                ReturnFromRecords();
            }
        }
        else if (LevelSelectScreen.activeSelf && GameManager.GM.SingleMode)
        {
            if (LevelSelectGhostPanel.activeSelf)
            {
                levelManager.CancelButton();
            }
            else if (LevelSelectHowToBoss.activeSelf)
            {
                levelManager.HowBossOK();
            }
            else
            {
                ReturnFromLevelSelect();
            }
        }
        else if (LevelSelectScreen.activeSelf && !GameManager.GM.SingleMode)
        {
            if (LevelSelectGhostPanel.activeSelf)
            {
                levelManager.CancelButton();
            }
            else if (LevelSelectHowToBoss.activeSelf)
            {
                levelManager.HowBossOK();
            }
            else
            {
                LevelSelectToMultiplayer();
            }
        }
        else if (ReturnUIButton.gameObject.activeSelf)
        {
            PressReturnUI();
        }
    }

    public void DownToWin(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            settingsManager.HoldingDown = true;
        } else if (value.canceled)
        {
            settingsManager.HoldingDown = false;
        }
    }

    //When the game starts, make sure all the menus are correctly active or not
    private void Awake()
    {
        ReturnUIButton.gameObject.SetActive(false);
        ScrollStoryScreen.SetActive(false);
        RecordsScreen.SetActive(false);
        LevelSelectScreen.SetActive(false);
        ConfirmRevertScreen.SetActive(false);
        Settings.SetActive(false);
        MultiplayerSelect.SetActive(false);
        MainMenu.SetActive(true);
    }

    //Default white FFFFFF
    //Sunset orange FFE7C4
    private void Start()
    {
        if (!GameManager.GM.FirstLoaded)
        {
            eventSystem.firstSelectedGameObject = PlayButton.gameObject;
        }

        if (GameManager.GM.FullCleared)
        {
            DevMsgButton.gameObject.SetActive(true);
            RenderSettings.skybox = fullSky;
            dirLight.color = Color.HSVToRGB(36/360f, 0.23f, 1f);
        }
        else
        {
            DevMsgButton.gameObject.SetActive(false);
            RenderSettings.skybox = notFullSky;
            dirLight.color = Color.HSVToRGB(0f, 0f, 1f);
        }
    }

    //Subscribes or unsubscribes from sceneLoaded, handles CourseSelect
    private void OnEnable()
    {
        SceneManager.sceneLoaded += CheckLevelSelect;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= CheckLevelSelect;
    }

    //Any time the user uses arrow keys, make sure FirstSeleced isn't missing, set the current item to the FirstSelected item, and or set the current item to FirstSelected. Handles wacky mouse to controller behaviour
    private void Update()
    {
        if (eventSystem.firstSelectedGameObject == null)
        {
            eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
        }

        if ((LeftMove != Vector2.zero) && (eventSystem.currentSelectedGameObject != null))
        {
            eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
        }

        if ((LeftMove != Vector2.zero) && (eventSystem.currentSelectedGameObject == null || eventSystem.currentSelectedGameObject.activeSelf))
        {
            eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
        }
    }

    //Level Select button
    public void PressPlay()
    {
        GameManager.GM.SingleMode = true;
        GameManager.GM.NumPlayers.Add(new MultiPlayerClass { PlayerIndex = 0, AimingSensitivity = PlayerPrefs.GetFloat("Sensitivity", 4) });
        MultiSelectScript.CurrentlyLoading = true;

        AudioManager.instance.PlaySound("UI_beep");
        StartCoroutine(SwipeUp(MainMenuRect, LevelSelectRect, LevelSelectFirstButton));
        levelManager.enabled = true;
    }

    public void PressPlayGhost()
    {
        GameManager.GM.SingleMode = true;
        GameManager.GM.GhostMode = true;
        GameManager.GM.NumPlayers.Add(new MultiPlayerClass { PlayerIndex = 0, AimingSensitivity = PlayerPrefs.GetFloat("Sensitivity", 4) });
        MultiSelectScript.CurrentlyLoading = true;

        AudioManager.instance.PlaySound("UI_beep");
        StartCoroutine(SwipeUp(MainMenuRect, LevelSelectRect, LevelSelectFirstButton));
        levelManager.enabled = true;
    }

    //Multiplayer button
    public void PressPlay2P()
    {
        inputSystem.enabled = false;

        inputManager.enabled = true;
        inputManager.EnableJoining();

        AudioManager.instance.PlaySound("UI_beep");
        StartCoroutine(SwipeDown(MainMenuRect, MultiplayerRect, MultiplayerFirstButton));
    }

    //Settings button
    public void PressSettings()
    {
        AudioManager.instance.PlaySound("UI_beep");
        StartCoroutine(SwipeLeft(MainMenuRect, SettingsRect, SettingsFirstButton));
    }

    //Records button
    public void PressRecords()
    {
        recordManager.UpdateThings();
        AudioManager.instance.PlaySound("UI_beep");
        StartCoroutine(SwipeRight(MainMenuRect, RecordsRect, RecordsFirstButton));
    }

    //Quit button
    public void PressQuit()
    {
        #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    //For displaying dev messages
    public void PressDevMsg()
    {
        if (EnableFunny)
        {
            AudioManager.instance.PlaySound("UI_beep");
            LoadingScreen.loadMan.LoadingMusic("FunnyShooter", false, "BGM_boss");
            return;
        }

        DevMsgPanel.SetActive(true);

        if (inputSystem.currentControlScheme == "Controller")
        {
            eventSystem.SetSelectedGameObject(DevMsgFirstButton.gameObject);
            eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
        }
        else
        {
            eventSystem.firstSelectedGameObject = DevMsgFirstButton.gameObject;
            eventSystem.SetSelectedGameObject(null);
        }

        AudioManager.instance.PlaySound("UI_beep");
    }

    public void ReturnDevMsg()
    {
        DevMsgPanel.SetActive(false);

        if (inputSystem.currentControlScheme == "Controller")
        {
            eventSystem.SetSelectedGameObject(DevMsgButton.gameObject);
            eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
        }
        else
        {
            eventSystem.firstSelectedGameObject = DevMsgButton.gameObject;
            eventSystem.SetSelectedGameObject(null);
        }

        AudioManager.instance.PlaySound("UI_beep");
    }

    //For hiding then returning UI
    public void PressHideUI()
    {
        MainMenu.SetActive(false);
        ReturnUIButton.gameObject.SetActive(true);

        if (inputSystem.currentControlScheme == "Controller")
        {
            eventSystem.SetSelectedGameObject(ReturnUIButton.gameObject);
            eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
        }
        else
        {
            eventSystem.firstSelectedGameObject = ReturnUIButton.gameObject;
            eventSystem.SetSelectedGameObject(null);
        }

        AudioManager.instance.PlaySound("UI_beep");
    }

    public void PressReturnUI()
    {
        MainMenu.SetActive(true);
        ReturnUIButton.gameObject.SetActive(false);

        if (inputSystem.currentControlScheme == "Controller")
        {
            eventSystem.SetSelectedGameObject(HideUIButton.gameObject);
            eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
        }
        else
        {
            eventSystem.firstSelectedGameObject = HideUIButton.gameObject;
            eventSystem.SetSelectedGameObject(null);
        }

        AudioManager.instance.PlaySound("UI_beep");
    }

    //Returning from Settings
    public void PressReturnToMain()
    {
        if (settingsManager.ChangedDisplayOption) //If critical display options have been changed, confirm with user first
        {
            AudioManager.instance.PlaySound("UI_beep");
            settingsManager.DisplayCheck();
            return;
        }

        AudioManager.instance.PlaySound("UI_confirm");

        settingsManager.ResetScreens();
        StartCoroutine(SwipeRight(SettingsRect, MainMenuRect, SettingsButton));

        GameManager.GM.SilentSave = true;
        GameManager.GM.SavePlayer();
    }

    //Returning from Multiplayer
    public void ReturnMainFromMulti()
    {
        inputManager.DisableJoining(); //Disable new players from joining

        MultiMenuPlayer[] objects = FindObjectsOfType<MultiMenuPlayer>(); //Find and remove all guest game objects
        foreach (var item in objects)
        {
            Destroy(item.gameObject);
        }

        GameManager.GM.NumPlayers.Clear();

        MultiSelectScript.ResetActions();

        inputManager.enabled = false; //Disable input manager
        
        inputSystem.enabled = true; //Re-enable menu input player

        AudioManager.instance.PlaySound("UI_beep");
        StartCoroutine(SwipeUp(MultiplayerRect, MainMenuRect, TwoPlayerBtn));
    }

    //Returning from Level Select
    public void ReturnFromLevelSelect()
    {
        if (GameManager.GM.SingleMode)
        {
            GameManager.GM.NumPlayers.Clear();

            MultiSelectScript.ResetActions();

            if (!GameManager.GM.GhostMode)
            {
                StartCoroutine(SwipeDown(LevelSelectRect, MainMenuRect, PlayButton)); //Not ghost mode
            } else
            {
                StartCoroutine(SwipeDown(LevelSelectRect, MainMenuRect, PlayGhostButton)); //Ghost mode
            }

            GameManager.GM.SingleMode = false;
            GameManager.GM.GhostMode = false;

            MultiSelectScript.CurrentlyLoading = false;

            AudioManager.instance.PlaySound("UI_beep");
            
            levelManager.enabled = false;
        } else
        {
            LevelSelectToMultiplayer();
        }
    }

    public void ReturnFromRecords()
    {
        AudioManager.instance.PlaySound("UI_beep");
        StartCoroutine(SwipeLeft(RecordsRect, MainMenuRect, RecordButton));
    }

    public void MultiplayerToLevelSelect()
    {
        AudioManager.instance.PlaySound("UI_confirm");
        StartCoroutine(SwipeUp(MultiplayerRect, LevelSelectRect, LevelSelectFirstButton));
    }

    public void LevelSelectToMultiplayer()
    {
        MultiSelectScript.CurrentlyLoading = false;
        inputManager.EnableJoining();
        AudioManager.instance.PlaySound("UI_beep");
        StartCoroutine(SwipeDown(LevelSelectRect, MultiplayerRect, MultiplayerFirstButton));
        levelManager.enabled = false;
    }

    //If returning from a Course to Course Select screen, this will load
    public void CheckLevelSelect(Scene scene, LoadSceneMode mode)
    {
        if (GameManager.GM.LoadIntoLevelSelect)
        {
            GameManager.GM.LoadIntoLevelSelect = false;

            MainMenu.SetActive(false);
            LevelSelectScreen.SetActive(true);

            MainMenuRect.offsetMin = new Vector2(-519.62f, 0);
            MainMenuRect.offsetMax = new Vector2(-519.62f, 0);

            LevelSelectRect.offsetMin = new Vector2(0, 0);
            LevelSelectRect.offsetMax = new Vector2(0, 0);

            levelManager.enabled = true;

            if (GameManager.GM.SingleMode == false)
            {
                MultiSelectScript.StraightIntoLevelSelect();
            }

            LevelSelectFirstButton.Select();
            eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
        }
    }

    //Buttons for help documents
    public void OpenHelpMenu()
    {
        HelpPanel.SetActive(true);

        if (inputSystem.currentControlScheme == "Controller")
        {
            eventSystem.SetSelectedGameObject(HelpFirstButton.gameObject);
            eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
        }
        else
        {
            eventSystem.firstSelectedGameObject = HelpFirstButton.gameObject;
            eventSystem.SetSelectedGameObject(null);
        }

        AudioManager.instance.PlaySound("UI_beep");
    }

    public void CloseHelpMenu()
    {
        HelpPanel.SetActive(false);

        if (inputSystem.currentControlScheme == "Controller")
        {
            eventSystem.SetSelectedGameObject(HelpButton.gameObject);
            eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
        }
        else
        {
            eventSystem.firstSelectedGameObject = HelpButton.gameObject;
            eventSystem.SetSelectedGameObject(null);
        }

        AudioManager.instance.PlaySound("UI_beep");
    }

    public void OpenHelpDocs(int value)
    {
        string file = "";

        switch (value)
        {
            case 0: //user manual
                file = Application.streamingAssetsPath + "/Software Major Yr 12 User Manual.pdf";
                break;

            case 1: //install guide
                file = Application.streamingAssetsPath + "/Software Major Yr 12 Installation Guide.pdf";
                break;

            case 2: //dev docs
                file = Application.streamingAssetsPath + "/Software Major Yr 12 Project Report.pdf";
                break;

            default:
                file = Application.streamingAssetsPath + "/Software Major Yr 12 User Manual.pdf";
                break;
        }

        Application.OpenURL(file);
        CloseHelpMenu();
    }

    //Buttons for the updater menu
    public void OpenUpdaterMenu()
    {
        UpdaterPanel.SetActive(true);

        if (inputSystem.currentControlScheme == "Controller")
        {
            eventSystem.SetSelectedGameObject(UpdaterFirstButton.gameObject);
            eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
        }
        else
        {
            eventSystem.firstSelectedGameObject = UpdaterFirstButton.gameObject;
            eventSystem.SetSelectedGameObject(null);
        }

        AudioManager.instance.PlaySound("UI_beep");
    }

    public void CloseUpdaterMenu()
    {
        UpdaterPanel.SetActive(false);

        if (inputSystem.currentControlScheme == "Controller")
        {
            eventSystem.SetSelectedGameObject(PlayButton.gameObject);
            eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
        }
        else
        {
            eventSystem.firstSelectedGameObject = PlayButton.gameObject;
            eventSystem.SetSelectedGameObject(null);
        }

        AudioManager.instance.PlaySound("UI_beep");
    }

    public void BeginUpdater()
    {
        if (Application.isEditor)
        {
            print(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\" + Application.productName + "\\Temp");
            print("Updating not available inside editor. Please access standalone to continue.");
            return;
        }

        UpdaterPanel.SetActive(false);
        MainMenu.SetActive(false);
        UpdatingScreen.SetActive(true);

        eventSystem.SetSelectedGameObject(null);
        eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;

        updateManager.BeginDownloadingUpdate();
    }

    //Handles swapping screens in and out and setting the active object
    public void ButtonManager(GameObject oldScreen, GameObject newScreen, Selectable setButton)
    {
        oldScreen.SetActive(false);
        newScreen.SetActive(true);
        if (inputSystem.currentControlScheme == "Controller")
        {
            setButton.Select();
            eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
        }
        else
        {
            eventSystem.firstSelectedGameObject = setButton.gameObject;
            eventSystem.SetSelectedGameObject(null);
        }
    }

    IEnumerator SwipeLeft(RectTransform oldScreen, RectTransform newScreen, Selectable setButton)
    {
        eventSystem.SetSelectedGameObject(null);

        float time = 0;
        float Duration = 0.3f;
        float newValue = 923.7604f;
        float otherTargetValue = -923.7604f;
        float oldValue = 0;

        float NewScreenLocation;
        float OldScreenLocation;

        newScreen.gameObject.SetActive(true);

        if (PlayerPrefs.GetInt("ReduceMotion", 0) == 0) //If false
        {
            while (time < Duration)
            {
                OldScreenLocation = Mathf.Lerp(oldValue, otherTargetValue, time / Duration);
                oldScreen.offsetMin = new Vector2(OldScreenLocation, 0);
                oldScreen.offsetMax = new Vector2(OldScreenLocation, 0);

                NewScreenLocation = Mathf.Lerp(newValue, oldValue, time / Duration);
                newScreen.offsetMin = new Vector2(NewScreenLocation, 0);
                newScreen.offsetMax = new Vector2(NewScreenLocation, 0);

                time += Time.deltaTime;
                yield return null;
            }
        }

        oldScreen.offsetMin = new Vector2(otherTargetValue, 0);
        oldScreen.offsetMax = new Vector2(otherTargetValue, 0);

        newScreen.offsetMin = new Vector2(0, 0);
        newScreen.offsetMax = new Vector2(0, 0);

        oldScreen.gameObject.SetActive(false);

        setButton.Select();
        yield return null;
        if (inputSystem.currentControlScheme == "Controller")
        {
            setButton.Select();
            eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
        }
        else
        {
            eventSystem.firstSelectedGameObject = setButton.gameObject;
            eventSystem.SetSelectedGameObject(null);
        }
    }

    IEnumerator SwipeRight(RectTransform oldScreen, RectTransform newScreen, Selectable setButton)
    {
        eventSystem.SetSelectedGameObject(null);

        float time = 0;
        float Duration = 0.3f;
        float newValue = -923.7604f;
        float otherTargetValue = 923.7604f;
        float oldValue = 0;

        float NewScreenLocation;
        float OldScreenLocation;

        newScreen.gameObject.SetActive(true);

        if (PlayerPrefs.GetInt("ReduceMotion", 0) == 0) //If false
        {
            while (time < Duration)
            {
                OldScreenLocation = Mathf.Lerp(oldValue, otherTargetValue, time / Duration);
                oldScreen.offsetMin = new Vector2(OldScreenLocation, 0);
                oldScreen.offsetMax = new Vector2(OldScreenLocation, 0);

                NewScreenLocation = Mathf.Lerp(newValue, oldValue, time / Duration);
                newScreen.offsetMin = new Vector2(NewScreenLocation, 0);
                newScreen.offsetMax = new Vector2(NewScreenLocation, 0);

                time += Time.deltaTime;
                yield return null;
            }
        }

        oldScreen.offsetMin = new Vector2(otherTargetValue, 0);
        oldScreen.offsetMax = new Vector2(otherTargetValue, 0);

        newScreen.offsetMin = new Vector2(0, 0);
        newScreen.offsetMax = new Vector2(0, 0);

        oldScreen.gameObject.SetActive(false);

        setButton.Select();
        yield return null;
        if (inputSystem.currentControlScheme == "Controller")
        {
            setButton.Select();
            eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
        }
        else
        {
            eventSystem.firstSelectedGameObject = setButton.gameObject;
            eventSystem.SetSelectedGameObject(null);
        }
    }

    IEnumerator SwipeUp(RectTransform oldScreen, RectTransform newScreen, Selectable setButton)
    {
        eventSystem.SetSelectedGameObject(null);

        float time = 0;
        float Duration = 0.3f;
        float newValue = -600f;
        float otherTargetValue = 600f;
        float oldValue = 0;

        float NewScreenLocation;
        float OldScreenLocation;

        newScreen.gameObject.SetActive(true);

        if (PlayerPrefs.GetInt("ReduceMotion", 0) == 0) //If false
        {
            while (time < Duration)
            {
                OldScreenLocation = Mathf.Lerp(oldValue, otherTargetValue, time / Duration);
                oldScreen.offsetMin = new Vector2(0, OldScreenLocation);
                oldScreen.offsetMax = new Vector2(0, OldScreenLocation);

                NewScreenLocation = Mathf.Lerp(newValue, oldValue, time / Duration);
                newScreen.offsetMin = new Vector2(0, NewScreenLocation);
                newScreen.offsetMax = new Vector2(0, NewScreenLocation);

                time += Time.deltaTime;
                yield return null;
            }
        }

        oldScreen.offsetMin = new Vector2(0, otherTargetValue);
        oldScreen.offsetMax = new Vector2(0, otherTargetValue);

        newScreen.offsetMin = new Vector2(0, 0);
        newScreen.offsetMax = new Vector2(0, 0);

        oldScreen.gameObject.SetActive(false);

        setButton.Select();
        yield return null;
        if (inputSystem.currentControlScheme == "Controller")
        {
            setButton.Select();
            eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
        }
        else
        {
            eventSystem.firstSelectedGameObject = setButton.gameObject;
            eventSystem.SetSelectedGameObject(null);
        }
    }

    IEnumerator SwipeDown(RectTransform oldScreen, RectTransform newScreen, Selectable setButton)
    {
        eventSystem.SetSelectedGameObject(null);

        float time = 0;
        float Duration = 0.3f;
        float newValue = 600f;
        float otherTargetValue = -600f;
        float oldValue = 0;

        float NewScreenLocation;
        float OldScreenLocation;

        newScreen.gameObject.SetActive(true);

        if (PlayerPrefs.GetInt("ReduceMotion", 0) == 0) //If false
        {
            while (time < Duration)
            {
                OldScreenLocation = Mathf.Lerp(oldValue, otherTargetValue, time / Duration);
                oldScreen.offsetMin = new Vector2(0, OldScreenLocation);
                oldScreen.offsetMax = new Vector2(0, OldScreenLocation);

                NewScreenLocation = Mathf.Lerp(newValue, oldValue, time / Duration);
                newScreen.offsetMin = new Vector2(0, NewScreenLocation);
                newScreen.offsetMax = new Vector2(0, NewScreenLocation);

                time += Time.deltaTime;
                yield return null;
            }
        }

        oldScreen.offsetMin = new Vector2(0, otherTargetValue);
        oldScreen.offsetMax = new Vector2(0, otherTargetValue);

        newScreen.offsetMin = new Vector2(0, 0);
        newScreen.offsetMax = new Vector2(0, 0);

        oldScreen.gameObject.SetActive(false);

        setButton.Select();
        yield return null;
        if (inputSystem.currentControlScheme == "Controller")
        {
            setButton.Select();
            eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
        }
        else
        {
            eventSystem.firstSelectedGameObject = setButton.gameObject;
            eventSystem.SetSelectedGameObject(null);
        }
    }
}
    
