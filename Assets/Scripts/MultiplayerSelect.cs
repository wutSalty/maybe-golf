using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.EventSystems;

//Handles joining and leaving players in multiplayer menu
public class MultiplayerSelect : MonoBehaviour
{
    //The play buttons and text
    public Button PlayBtn;
    public Text PlayText;
    public GameObject dummyObject;

    //List of text showing connected players
    public List<Text> TextList;

    //List of dropdown for keyboard user
    public List<Dropdown> DropdownList;

    public List<GameObject> PlayerPanels;

    //Input managers
    public PlayerInputManager inputManager;
    public EventSystem eventSystem;
    public UIManager uiManager;
    public LevelManager levelManager;

    //Flag to signal switch scenes
    [HideInInspector]
    public bool CurrentlyLoading = false;

    //Sets text and everything ready
    private void Start()
    {
        CurrentlyLoading = false;
        PlayBtn.interactable = false;
        PlayText.text = "Waiting for players...";
    }

    //When the player joins, add them to GameManager, change text, etc etc
    public void WhenAPlayerJoins(PlayerInput value)
    {
        string ControlName = "Demo";
        int ControlType = 0;
        int pIndex = value.playerIndex;

        //Checks if enough indexes are available, if not create new index
        if (GameManager.GM.NumPlayers.Count - 1 < pIndex)
        {
            GameManager.GM.NumPlayers.Add(new MultiPlayerClass { });
        }

        //Checks device type being connected. Applies appropriate text or control type
        if (value.currentControlScheme == "Controller")
        {
            ControlName = "Controller/Buttons";
            ControlType = 2; //2 = Controller buttons

        } else //Or else it's a keyboard user and let them make a choice
        {
            ControlName = "";
            DropdownList[pIndex].gameObject.SetActive(true);
            ControlType = 0;
        }

        //Displays and stores all relevant information
        TextList[pIndex].text = "Connected\n" + ControlName;
        GameManager.GM.NumPlayers[pIndex].PlayerIndex = pIndex;
        GameManager.GM.NumPlayers[pIndex].ControlType = ControlType;
        GameManager.GM.NumPlayers[pIndex].inputDevice = InputUser.all[value.playerIndex].pairedDevices[0];
        Debug.Log("Player " + pIndex + "'s input device is: " + InputUser.all[pIndex].pairedDevices[0]);

        //Display player's card
        PlayerPanels[pIndex].SetActive(true);
   
        //Once more than 1 player is present, allow game to start
        if (inputManager.playerCount > 1)
        {
            PlayBtn.interactable = true;
            eventSystem.firstSelectedGameObject = dummyObject;
            eventSystem.SetSelectedGameObject(dummyObject);
            PlayText.text = "Ready to Play!";
        }
    }

    //When the dropdown is updated, update the relevant Control Type
    public void DropdownUpdate(int playerIndex)
    {
        GameManager.GM.NumPlayers[playerIndex].ControlType = DropdownList[playerIndex].value;
        StartCoroutine(Delay());
    }

    //Delay for non-host controller trickery
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.05f);
        if (PlayBtn.interactable == true)
        {
            eventSystem.firstSelectedGameObject = dummyObject;
            eventSystem.SetSelectedGameObject(dummyObject);
        } else
        {
            eventSystem.firstSelectedGameObject = null;
            eventSystem.SetSelectedGameObject(null);
        }
        
    }

    //When a controller disconnects or other
    public void WhenAPlayerDisconnects(PlayerInput value)
    {
        if (CurrentlyLoading == false) //Needs check in-case game is changing scenes
        {
            //Disable and reset everything
            PlayerPanels[value.playerIndex].SetActive(false);
            DropdownList[value.playerIndex].gameObject.SetActive(false);
            TextList[value.playerIndex].text = "Not Connected";
            GameManager.GM.NumPlayers[value.playerIndex].PlayerIndex = 99;
            
            if (inputManager.playerCount == 1 || value.playerIndex == 0) //If player left is only 1, disable play button
            {
                PlayBtn.interactable = false;
                eventSystem.firstSelectedGameObject = null;
                eventSystem.SetSelectedGameObject(null);
                PlayText.text = "Waiting for players...";
            }
        }
    }

    //The play button
    public void PlayReady()
    {
        GameManager.GM.SingleMode = false;
        CurrentlyLoading = true;
        inputManager.DisableJoining();

        uiManager.MultiplayerToLevelSelect();
        levelManager.enabled = true;
    }
}
