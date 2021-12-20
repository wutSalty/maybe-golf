using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PauseGame : MonoBehaviour
{
    public static PauseGame pM;

    public Canvas PauseMenu;
    public Button ResumeButton;

    public Text EscText;
    public Text RestartText;
    public Text PlayerPausedText;

    public EventSystem eventSystem;
    public InputSystemUIInputModule UiInputModule;

    [HideInInspector]
    public Vector2 LeftMove;

    [HideInInspector]
    public bool MenuIsOpen = false;

    private PlayerInput CurrentPlayer;
    public InputActionAsset inputActionAsset;

    void Awake()
    {
        if (pM != null && pM != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            pM = this;
        }
    }

    private void Start()
    {
        inputActionAsset = UiInputModule.actionsAsset;
    }

    //If user moves stuff, make sure things are updated
    public void Update()
    {
        if ((LeftMove != Vector2.zero) && (eventSystem.currentSelectedGameObject != null))
        {
            eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
        }

        if ((LeftMove != Vector2.zero) && (eventSystem.currentSelectedGameObject == null || eventSystem.currentSelectedGameObject.activeSelf))
        {
            eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
        }
    }

    public void OverrideForThePauseOverride()
    {
        ButtonClickOverrideCauseImLazy(false, 99);
    }

    //When the pause menu is accessed
    public void ButtonClickOverrideCauseImLazy(bool FromKey, int pIndex)
    {
        PlayerInput[] listofInputs = FindObjectsOfType<PlayerInput>(); //Get all the PlayerInputs in play
        foreach (PlayerInput item in listofInputs) //For each of them
        {
            //Change the Action Map to the required one
            if (MenuIsOpen == false)
            {
                if (item.playerIndex == pIndex) //Sets menu to the user who opened it
                {
                    item.SwitchCurrentActionMap("UI");
                    CurrentPlayer = item;
                    CurrentPlayer.uiInputModule = UiInputModule;
                } else
                {
                    item.SwitchCurrentActionMap("Not Caller Menu");
                }
            }
            else
            {
                item.SwitchCurrentActionMap("In-Game Ball");
            }
        }

        //If menu was clicked by mouse, show "by host" instead of "by player"
        if (pIndex == 99)
        {
            PlayerPausedText.text = "Paused By: Host";
        } else
        {
            PlayerPausedText.text = "Paused By: Player " + (pIndex + 1);
        }

        OpenCloseMenu(); //Do menu stuff

        if (pIndex != 99 && MenuIsOpen) //Select Resume button if not selected by mouse
        {
            switch (GameManager.GM.NumPlayers[pIndex].ControlType)
            {
                case 0: //Mouse
                    break;

                case 1: //Keyboard
                    ResumeButton.Select();
                    break;

                case 2: //Controller
                    ResumeButton.Select();
                    break;

                default:
                    ResumeButton.Select();
                    break;
            }
        }
    }

    //Opens or closes the menu
    public void OpenCloseMenu()
    {
        if (MenuIsOpen == false)
        {
            Time.timeScale = 0;
            MenuIsOpen = true;
            PauseMenu.gameObject.SetActive(true);
            eventSystem.firstSelectedGameObject = ResumeButton.gameObject;
        }
        else
        {
            Time.timeScale = 1;
            MenuIsOpen = false;
            PauseMenu.gameObject.SetActive(false);
            eventSystem.firstSelectedGameObject = null;
            eventSystem.SetSelectedGameObject(null);

            //For UI stuff that only needs to happen once
            //CurrentPlayer.uiInputModule = null;
            UiInputModule.actionsAsset = inputActionAsset;
        }
    }

    public void ControlsHaveChanged(PlayerInput playerInput)
    {
        var currentScheme = playerInput.currentControlScheme;
        Debug.Log(playerInput.currentControlScheme);

        if (currentScheme == "Keyboard")
        {
            EscText.text = "Pause (Esc)";
            RestartText.text = "Restart (R)";

        }
        else if (currentScheme == "Controller")
        {
            EscText.text = "Pause (+/Menu)";
            RestartText.text = "Restart (L+R)";
        }
    }
}
