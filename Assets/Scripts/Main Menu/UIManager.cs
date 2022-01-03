using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEditor;

public class UIManager : MonoBehaviour
{
    public GameObject MainMenu; //Main Menu object
    public GameObject MultiplayerSelect;
    public GameObject Settings; //Settings object
    public GameObject ConfirmRevertScreen; //Confirm Reject screen object

    public Button PlayButton;
    public Button TwoPlayerBtn;
    public Button SettingsButton;
    public Button QuitButton;

    public Selectable DropdownA;
    public Selectable ButtonReady;

    public EventSystem eventSystem;
    public PlayerInput inputSystem;
    public PlayerInputManager inputManager;

    public MultiplayerSelect MultiSelectScript;

    [HideInInspector]
    public Vector2 LeftMove;

    //Gets value of movement stick
    public void UpDownLeftRight(InputAction.CallbackContext value)
    {
        LeftMove = value.ReadValue<Vector2>();
    }

    //If button is pressed
    public void EnterController(InputAction.CallbackContext value)
    {
        if (eventSystem.currentSelectedGameObject != null)
        {
            eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
        }
    }

    //When the game starts, make sure all the menus are correctly active or not
    private void Awake()
    {
        ConfirmRevertScreen.SetActive(false);
        Settings.SetActive(false);
        MultiplayerSelect.SetActive(false);
        MainMenu.SetActive(true);
    }

    private void Start()
    {
        GameManager.GM.SingleMode = false;
    }

    //Any time the user uses arrow keys, make sure FirstSeleced isn't missing, set the current item to the FirstSelected item, and or set the current item to FirstSelected
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

    //Main Menu buttons
    public void PressPlay()
    {
        GameManager.GM.SingleMode = true;
        GameManager.GM.NumPlayers.Add(new MultiPlayerClass { PlayerIndex = 0 });
        MultiSelectScript.CurrentlyLoading = true;

        LoadingScreen.loadMan.BeginLoadingScene("SampleScene", true);
        //SceneManager.LoadScene("SampleScene");
    }

    public void PressPlay2P()
    {
        inputSystem.enabled = false;
        inputManager.enabled = true;
        inputManager.EnableJoining();
        //MultiSelectScript.UpdatePlayerOneText();
        ButtonManager(MainMenu, MultiplayerSelect, ButtonReady);
    }

    public void PressSettings()
    {
        ButtonManager(MainMenu, Settings, DropdownA);
    }

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
    
