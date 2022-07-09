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

    ////If by any chance the pause menu has to be opened by the "host" user
    //public void OverrideForThePauseOverride()
    //{
    //    ButtonClickOverrideCauseImLazy(99);
    //}

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
                    item.GetComponent<MultiplayerEventSystem>().enabled = false;
                }
            }
            else
            {
                item.SwitchCurrentActionMap("In-Game Ball");
                item.GetComponent<MultiplayerEventSystem>().enabled = true;
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
            print("game paused");
        }
        else
        {
            Time.timeScale = 1;
            MenuIsOpen = false;
            print("game unpaused");
        }
    }
}
