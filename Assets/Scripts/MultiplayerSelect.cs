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

    public GameObject emptyPlayerObject;

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
    public InputSystemUIInputModule uiModule;
    public PlayerInput defaultInput;

    //Flag to signal switch scenes
    [HideInInspector]
    public bool CurrentlyLoading = false;

    //Sets text and everything ready
    private void Awake()
    {
        CurrentlyLoading = false;
        PlayBtn.interactable = false;
        PlayText.text = "Waiting for players...";

        print("awake disable play");
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
            DropdownList[pIndex].value = 0;
            ControlType = 0;
        }

        //Displays and stores all relevant information
        TextList[pIndex].text = "Connected\n" + ControlName;
        GameManager.GM.NumPlayers[pIndex].PlayerIndex = pIndex;
        GameManager.GM.NumPlayers[pIndex].ControlType = ControlType;
        GameManager.GM.NumPlayers[pIndex].inputDevice = value.GetDevice<InputDevice>();
        GameManager.GM.NumPlayers[pIndex].deviceName = value.GetDevice<InputDevice>().name;

        print("Player " + pIndex + "'s input device is: " + value.GetDevice<InputDevice>().name);

        //Display player's card
        PlayerPanels[pIndex].SetActive(true);
   
        //Once more than 1 player is present, allow game to start
        if (inputManager.playerCount > 1)
        {
            PlayBtn.interactable = true;
            eventSystem.firstSelectedGameObject = dummyObject;
            eventSystem.SetSelectedGameObject(dummyObject);
            PlayText.text = "Ready to Play!";

            print("ping actual play btn");
        }
    }

    //When the dropdown is updated, update the relevant Control Type
    public void DropdownUpdate(int playerIndex)
    {
        GameManager.GM.NumPlayers[playerIndex].ControlType = DropdownList[playerIndex].value;
        //StartCoroutine(Delay());
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
                //eventSystem.firstSelectedGameObject = null;
                eventSystem.SetSelectedGameObject(null);
                PlayText.text = "Waiting for players...";

                print("ping disabled play btn");
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

    public void StraightIntoLevelSelect()
    {
        uiManager.inputSystem.enabled = false;

        uiManager.inputManager.enabled = true;

        foreach (var item in GameManager.GM.NumPlayers)
        {
            string ControlName = "Demo";
            int pIndex = item.PlayerIndex;

            //Checks device type being connected. Applies appropriate text or control type
            if (item.ControlType == 2)
            {
                ControlName = "Controller/Buttons";

                PlayerInput.Instantiate(emptyPlayerObject, pIndex, "Controller", -1, item.inputDevice);

            }
            else //Or else it's a keyboard user and let them make a choice
            {
                ControlName = "";
                DropdownList[pIndex].gameObject.SetActive(true);
                DropdownList[pIndex].value = item.ControlType;

                PlayerInput.Instantiate(emptyPlayerObject, pIndex, "Mouse Keyboard", -1, item.inputDevice);
            }

            //Displays all relevant information
            TextList[pIndex].text = "Connected\n" + ControlName;

            //Display player's card
            PlayerPanels[pIndex].SetActive(true);
        }

        PlayBtn.interactable = true;
        //eventSystem.firstSelectedGameObject = dummyObject;
        //eventSystem.SetSelectedGameObject(dummyObject);
        PlayText.text = "Ready to Play!";

        CurrentlyLoading = true;
        inputManager.DisableJoining();

        print("ping override play btn");
    }

    public InputActionReference pointing;
    public InputActionReference Lclick;
    public InputActionReference Mclick;
    public InputActionReference Rclick;
    public InputActionReference scroll;
    public InputActionReference navi;
    public InputActionReference submit;
    public InputActionReference cancel;

    public void ResetActions()
    {
        defaultInput.uiInputModule = uiModule;
        uiModule.actionsAsset = defaultInput.actions;

        uiModule.point = pointing;
        uiModule.leftClick = Lclick;
        uiModule.middleClick = Mclick;
        uiModule.rightClick = Rclick;
        uiModule.scrollWheel = scroll;
        uiModule.move = navi;
        uiModule.submit = submit;
        uiModule.cancel = cancel;
    }
}
