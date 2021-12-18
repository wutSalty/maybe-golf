using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseGame : MonoBehaviour
{
    public static PauseGame pM;

    public Canvas PauseMenu;
    public Button ResumeButton;

    public Text EscText;
    public Text RestartText;

    public EventSystem eventSystem;

    [HideInInspector]
    public Vector2 LeftMove;

    [HideInInspector]
    public bool MenuIsOpen = false;

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

        //DontDestroyOnLoad(this);
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

    //When the pause menu is accessed
    public void ButtonClickOverrideCauseImLazy(bool FromKey)
    {
        bool ControllerYes = false;
        PlayerInput[] listofInputs = FindObjectsOfType<PlayerInput>(); //Get all the PlayerInputs in play
        foreach (PlayerInput item in listofInputs) //For each of them
        {
            //Change the Action Map to the required one
            if (MenuIsOpen == false)
            {
                item.SwitchCurrentActionMap("Pause Menu");
            }
            else
            {
                item.SwitchCurrentActionMap("In-Game Ball");
            }

            //If any of them are using button controls, set ControllerYes to true
            if (item.currentControlScheme == "Keyboard" || item.currentControlScheme == "Controller" || FromKey)
            {
                ControllerYes = true;
            }
        }

        OpenCloseMenu(); //Do menu stuff

        //If accessed from a button, make sure the first option is selected
        if (ControllerYes)
        {
            if (MenuIsOpen == true)
            {
                ResumeButton.Select();
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
            eventSystem.SetSelectedGameObject(null);
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
