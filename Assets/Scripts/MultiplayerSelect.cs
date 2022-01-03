using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.EventSystems;

public class MultiplayerSelect : MonoBehaviour
{
    //The play buttons and text
    public Button PlayBtn;
    public Text PlayText;

    //List of text showing connected players
    public List<Text> TextList;

    //List of dropdown for keyboard user
    public List<Dropdown> DropdownList;

    //Input managers
    public PlayerInputManager inputManager;
    public EventSystem eventSystem;

    //Flag to signal switch scenes
    [HideInInspector]
    public bool CurrentlyLoading = false;

    private void Start()
    {
        CurrentlyLoading = false;
        PlayBtn.interactable = false;
        PlayText.text = "Waiting for players...";
    }

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
        } else
        {
            DropdownList[pIndex].gameObject.SetActive(true);
            ControlType = 0;
        }

        //Displays and stores all relevant information
        TextList[pIndex].text = "Connected\n" + ControlName;
        GameManager.GM.NumPlayers[pIndex].PlayerIndex = pIndex;
        GameManager.GM.NumPlayers[pIndex].ControlType = ControlType;
        GameManager.GM.NumPlayers[pIndex].inputDevice = InputUser.all[value.playerIndex].pairedDevices[0];
        Debug.Log("Player " + pIndex + "'s input device is: " + InputUser.all[pIndex].pairedDevices[0]);
   
        //Once more than 1 player is present, allow game to start
        if (inputManager.playerCount > 1 && value.playerIndex > 0)
        {
            PlayBtn.interactable = true;
            eventSystem.firstSelectedGameObject = PlayBtn.gameObject;
            PlayText.text = "Ready to Play!";
        }
    }

    //When the dropdown is updated, update the relevant Control Type
    public void DropdownUpdate(int playerIndex)
    {
        GameManager.GM.NumPlayers[playerIndex].ControlType = DropdownList[playerIndex].value;
    }

    public void WhenAPlayerDisconnects(PlayerInput value)
    {
        if (CurrentlyLoading == false) //Needs check in-case game is changing scenes
        {
            //Disable and reset everything
            DropdownList[value.playerIndex].gameObject.SetActive(false);
            TextList[value.playerIndex].text = "Not Connected";
            GameManager.GM.NumPlayers[value.playerIndex].PlayerIndex = 99;
            
            if (inputManager.playerCount == 1 || value.playerIndex == 0) //If player left is only 1, disable play button
            {
                PlayBtn.interactable = false;
                eventSystem.firstSelectedGameObject = null;
                PlayText.text = "Waiting for players...";
            }
        }
    }

    //The play button
    public void PlayReady()
    {
        GameManager.GM.SingleMode = false;
        CurrentlyLoading = true;
        LoadingScreen.loadMan.BeginLoadingScene("SampleScene", true);
    }
}
