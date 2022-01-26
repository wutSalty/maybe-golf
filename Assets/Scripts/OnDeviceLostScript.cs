using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

//This script just handles all the input from the multiplayer menu at this point
public class OnDeviceLostScript : MonoBehaviour
{
    private PlayerInput playerInput;
    private UIManager uiManager;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        uiManager = FindObjectOfType<UIManager>();
    }

    //Checks if they're Player 1. If they are, give them privleges
    private void Start()
    {
        if (playerInput.playerIndex == 0)
        {
            playerInput.SwitchCurrentActionMap("MultiHost");
        }
        else
        {
            playerInput.SwitchCurrentActionMap("MultiGuest");
        }
    }
    
    //Passes information for UI behaviour
    public void OnMove(InputValue value)
    {
        uiManager.LeftMove = value.Get<Vector2>();
    }

    //If the controller has been disconnected due to running out of batteries or other, delete the object with a delay
    void OnDeviceLost()
    {
        Destroy(this.gameObject, 0.005f);
    }

    //If 'Back' is pressed, also delete the game object (but no delay cause this doesn't break things for some reason)
    void OnRemoveController()
    {
        Destroy(this.gameObject);
    }

    //If the host needs to back out for whatever reason
    void OnExitMenu()
    {
        uiManager.BackingOut();
    }
}
