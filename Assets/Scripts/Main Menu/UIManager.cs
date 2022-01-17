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
    //The different screens
    public GameObject MainMenu; //Main Menu object
    public GameObject MultiplayerSelect; //Multiplayer controller screen
    public GameObject Settings; //Settings object
    public GameObject ConfirmRevertScreen; //Confirm Reject screen object
    public GameObject DeleteConfirmScreen; //Setting's delete everything screen
    public GameObject LevelSelectScreen; //Level select screen
    public GameObject RecordsScreen; //Records screen
    public GameObject ScrollStoryScreen;

    //Main Menu Buttons
    public Button PlayButton;
    public Button TwoPlayerBtn;
    public Button SettingsButton;
    public Button RecordButton;
    public Button QuitButton;

    //The buttons that should be selected when menus are accessed
    public Selectable SettingsFirstButton;
    public Selectable MultiplayerFirstButton;
    public Selectable LevelSelectFirstButton;
    public Selectable RecordsFirstButton;

    //Any event or input system
    public EventSystem eventSystem;
    public PlayerInput inputSystem;
    public PlayerInputManager inputManager;

    //Scripts for interfacing
    public SettingsManager settingsManager;
    public MultiplayerSelect MultiSelectScript;
    public LevelManager levelManager;
    public RecordsManager recordManager;

    [HideInInspector]
    public Vector2 LeftMove;

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
            if (MainMenu.activeSelf)
            {
                //Lol shouldn't do anything here so its fine
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
            else if (LevelSelectScreen.activeSelf)
            {
                ReturnFromLevelSelect();
            }
        }
    }

    //When the game starts, make sure all the menus are correctly active or not
    private void Awake()
    {
        ScrollStoryScreen.SetActive(false);
        RecordsScreen.SetActive(false);
        LevelSelectScreen.SetActive(false);
        ConfirmRevertScreen.SetActive(false);
        Settings.SetActive(false);
        MultiplayerSelect.SetActive(false);
        MainMenu.SetActive(true);
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

        ButtonManager(MainMenu, LevelSelectScreen, LevelSelectFirstButton);
        levelManager.enabled = true;
    }

    public void PressPlayGhost()
    {
        GameManager.GM.SingleMode = true;
        GameManager.GM.GhostMode = true;
        GameManager.GM.NumPlayers.Add(new MultiPlayerClass { PlayerIndex = 0, AimingSensitivity = PlayerPrefs.GetFloat("Sensitivity", 4) });
        MultiSelectScript.CurrentlyLoading = true;

        ButtonManager(MainMenu, LevelSelectScreen, LevelSelectFirstButton);
        levelManager.enabled = true;
    }

    //Multiplayer button
    public void PressPlay2P()
    {
        inputSystem.enabled = false;
        inputManager.enabled = true;
        inputManager.EnableJoining();

        ButtonManager(MainMenu, MultiplayerSelect, MultiplayerFirstButton);
    }

    //Settings button
    public void PressSettings()
    {
        ButtonManager(MainMenu, Settings, SettingsFirstButton);
    }

    //Records button
    public void PressRecords()
    {
        recordManager.UpdateThings();
        ButtonManager(MainMenu, RecordsScreen, RecordsFirstButton);
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

    //Returning from Settings
    public void PressReturnToMain()
    {
        ButtonManager(Settings, MainMenu, SettingsButton);
    }

    //Returning from Multiplayer
    public void ReturnMainFromMulti()
    {
        inputManager.DisableJoining(); //Disable new players from joining

        OnDeviceLostScript[] objects = FindObjectsOfType<OnDeviceLostScript>(); //Find and remove all guest game objects
        foreach (var item in objects)
        {
            Destroy(item.gameObject);
        }

        GameManager.GM.NumPlayers.Clear();

        inputManager.enabled = false; //Disable input manager

        inputSystem.enabled = true; //Re-enable menu input player
        ButtonManager(MultiplayerSelect, MainMenu, TwoPlayerBtn);
    }

    //Returning from Level Select
    public void ReturnFromLevelSelect()
    {
        if (GameManager.GM.SingleMode)
        {
            GameManager.GM.NumPlayers.Clear();
            GameManager.GM.SingleMode = false;
            MultiSelectScript.CurrentlyLoading = false;
        } else
        {
            MultiSelectScript.CurrentlyLoading = false;

            inputManager.DisableJoining(); //Disable new players from joining

            OnDeviceLostScript[] objects = FindObjectsOfType<OnDeviceLostScript>(); //Find and remove all guest game objects
            foreach (var item in objects)
            {
                Destroy(item.gameObject);
            }

            GameManager.GM.NumPlayers.Clear();

            inputManager.enabled = false; //Disable input manager
            inputSystem.enabled = true; //Re-enable menu input player
        }
        GameManager.GM.GhostMode = true;

        ButtonManager(LevelSelectScreen, MainMenu, PlayButton);
        levelManager.enabled = false;
    }

    public void ReturnFromRecords()
    {
        ButtonManager(RecordsScreen, MainMenu, RecordButton);
    }

    public void LevelSelectToMultiplayer()
    {
        MultiSelectScript.CurrentlyLoading = false;
        ButtonManager(LevelSelectScreen, MultiplayerSelect, MultiplayerFirstButton);
        levelManager.enabled = false;
    }

    //If returning from a Course to Course Select screen, this will load
    public void CheckLevelSelect(Scene scene, LoadSceneMode mode)
    {
        if (GameManager.GM.LoadIntoLevelSelect)
        {
            GameManager.GM.LoadIntoLevelSelect = false;
            ButtonManager(MainMenu, LevelSelectScreen, LevelSelectFirstButton);
            levelManager.enabled = true;
        }
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
}
    
