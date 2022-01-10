using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

//Handles most of the global work for pausing the game
public class PauseGame : MonoBehaviour
{
    public static PauseGame pM;

    //public Canvas PauseMenu;
    //public Button ResumeButton;

    //public Text EscText;
    //public Text RestartText;
    //public Text PlayerPausedText;

    //public EventSystem eventSystem;

    //[HideInInspector]
    //public Vector2 LeftMove;

    [HideInInspector]
    public bool MenuIsOpen = false;

    private PlayerInput[] listofInputs;

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
        listofInputs = FindObjectsOfType<PlayerInput>(); //Get all the PlayerInputs in play
    }

    ////If user moves stuff, make sure things are updated
    //public void Update()
    //{
    //    if ((LeftMove != Vector2.zero) && (eventSystem.currentSelectedGameObject != null))
    //    {
    //        eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
    //    }

    //    if ((LeftMove != Vector2.zero) && (eventSystem.currentSelectedGameObject == null || eventSystem.currentSelectedGameObject.activeSelf))
    //    {
    //        eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
    //    }
    //}

    //If by any chance the pause menu has to be opened by the "host" user
    public void OverrideForThePauseOverride()
    {
        ButtonClickOverrideCauseImLazy(99);
    }

    //When the pause menu is accessed
    public void ButtonClickOverrideCauseImLazy(int pIndex)
    {
        foreach (PlayerInput item in listofInputs) //For each player input
        {
            //Change the Action Map to the required one
            if (MenuIsOpen == false)
            {
                if (item.playerIndex == pIndex) //Sets menu to the user who opened it
                {
                    item.SwitchCurrentActionMap("UI");
                } else
                { //Or else make sure the user can't do anything
                    item.SwitchCurrentActionMap("Not Caller Menu");
                }
            }
            else
            {
                item.SwitchCurrentActionMap("In-Game Ball");
            }
        }
        OpenCloseMenu(); //Do menu stuff
    }

    //Opens or closes the menu
    public void OpenCloseMenu()
    {
        if (MenuIsOpen == false)
        {
            Time.timeScale = 0;
            MenuIsOpen = true;
        }
        else
        {
            Time.timeScale = 1;
            MenuIsOpen = false;
        }
    }

    //public void ControlsHaveChanged(PlayerInput playerInput)
    //{
    //    var currentScheme = playerInput.currentControlScheme;
    //    Debug.Log(playerInput.currentControlScheme);

    //    if (currentScheme == "Keyboard")
    //    {
    //        EscText.text = "Pause (Esc)";
    //        RestartText.text = "Restart (R)";

    //    }
    //    else if (currentScheme == "Controller")
    //    {
    //        EscText.text = "Pause (+/Menu)";
    //        RestartText.text = "Restart (L+R)";
    //    }
    //}
}
