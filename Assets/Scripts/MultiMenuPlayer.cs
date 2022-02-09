using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

//This script just handles all the input from the multiplayer menu at this point
public class MultiMenuPlayer : MonoBehaviour
{
    private PlayerInput playerInput;
    private UIManager uiManager;
    private InputSystemUIInputModule uiModule;

    private string controlType;
    private int pIndex;

    public InputActionReference pointing;
    public InputActionReference Lclick;
    public InputActionReference Mclick;
    public InputActionReference Rclick;
    public InputActionReference scroll;
    public InputActionReference navi;
    public InputActionReference submit;
    public InputActionReference cancel;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        uiManager = FindObjectOfType<UIManager>();
        uiModule = FindObjectOfType<InputSystemUIInputModule>();
    }

    //Checks if they're Player 1. If they are, give them privleges
    private void Start()
    {
        pIndex = playerInput.playerIndex;

        if (pIndex == 0)
        {
            playerInput.uiInputModule = uiModule;
            uiModule.actionsAsset = playerInput.actions;

            uiModule.point = pointing;
            uiModule.leftClick = Lclick;
            uiModule.middleClick = Mclick;
            uiModule.rightClick = Rclick;
            uiModule.scrollWheel = scroll;
            uiModule.move = navi;
            uiModule.submit = submit;
            uiModule.cancel = cancel;

            playerInput.SwitchCurrentActionMap("MultiHost");
        }
        else
        {
            playerInput.SwitchCurrentActionMap("MultiGuest");
        }

        if (playerInput.currentControlScheme == "Controller")
        {
            controlType = "Controller";
        }
        else
        {
            controlType = "KB";
        }
    }
    
    //Passes information for UI behaviour
    public void OnMove(InputValue value)
    {
        uiManager.LeftMove = value.Get<Vector2>();
    }

    void OnSwitchControl()
    {
        if (controlType == "KB")
        {
            if (uiManager.MultiSelectScript.DropdownList[pIndex].value == 0)
            {
                uiManager.MultiSelectScript.DropdownList[pIndex].value = 1;
            }
            else
            {
                uiManager.MultiSelectScript.DropdownList[pIndex].value = 0;
            }
            
        }
    }

    //If the controller has been disconnected due to running out of batteries or other, delete the object with a delay
    void OnDeviceLost()
    {
        Destroy(this.gameObject, 0.005f);
    }

    //If 'Back' is pressed, also delete the game object (but no delay cause this doesn't break things for some reason)
    void OnRemoveController()
    {
        if (uiManager.MultiSelectScript.CurrentlyLoading)
        {
            return;
        }
        Destroy(this.gameObject);
    }

    //If the host needs to back out for whatever reason
    void OnExitMenu()
    {
        uiManager.BackingOut();
    }
}
