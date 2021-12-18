using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class EscMenu : MonoBehaviour
{
    //Migrated script to PauseGame

    //public Canvas PauseMenu;
    //public Button ResumeButton;

    //public EventSystem eventSystem;

    //[HideInInspector]
    //public Vector2 LeftMove;

    //[HideInInspector]
    //public static bool MenuIsOpen = false;

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

    ////When the pause menu is accessed
    //public void ButtonClickOverrideCauseImLazy(bool FromKey)
    //{
    //    bool ControllerYes = false;
    //    PlayerInput[] listofInputs = FindObjectsOfType<PlayerInput>(); //Get all the PlayerInputs in play
    //    foreach (PlayerInput item in listofInputs) //For each of them
    //    {
    //        //Change the Action Map to the required one
    //        if (MenuIsOpen == false)
    //        {
    //            item.SwitchCurrentActionMap("Pause Menu");
    //        }
    //        else
    //        {
    //            item.SwitchCurrentActionMap("In-Game Ball");
    //        }

    //        //If any of them are using button controls, set ControllerYes to true
    //        if (item.currentControlScheme == "Keyboard" || item.currentControlScheme == "Controller" || FromKey)
    //        {
    //            ControllerYes = true;
    //        }
    //    }

    //    OpenCloseMenu(); //Do menu stuff

    //    //If accessed from a button, make sure the first option is selected
    //    if (ControllerYes)
    //    {
    //        if (MenuIsOpen == true)
    //        {
    //            ResumeButton.Select();
    //        }
    //    }
    //}

    ////Opens or closes the menu
    //public void OpenCloseMenu()
    //{
    //    if (MenuIsOpen == false)
    //    {
    //        Time.timeScale = 0;
    //        MenuIsOpen = true;
    //        PauseMenu.gameObject.SetActive(true);
    //        eventSystem.firstSelectedGameObject = ResumeButton.gameObject;
    //    }
    //    else
    //    {
    //        Time.timeScale = 1;
    //        MenuIsOpen = false;
    //        PauseMenu.gameObject.SetActive(false);
    //        eventSystem.SetSelectedGameObject(null);
    //    }
    //}

    //Returns to main menu
    public void ReturnToMain()
    {
        Time.timeScale = 1;
        PauseGame.pM.MenuIsOpen = false;

        //Reset NumPlayers from GM
        if (GameManager.GM.NumPlayers.Count > 1)
        {
            GameManager.GM.NumPlayers.RemoveRange(1, GameManager.GM.NumPlayers.Count - 1);
        }

        SceneManager.LoadScene("MainMenu");
    }
}
