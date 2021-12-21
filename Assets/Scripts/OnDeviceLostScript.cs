using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OnDeviceLostScript : MonoBehaviour
{
    private PlayerInput playerInput;
    public UIManager uiManager;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        uiManager = FindObjectOfType<UIManager>();
    }

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

    public void OnMove(InputValue value)
    {
        uiManager.LeftMove = value.Get<Vector2>();
    }

    //If the controller has been disconnected due to running out of batteries or other, start the delay
    void OnDeviceLost()
    {
        //Destroy(this.gameObject);
        StartCoroutine(DelayDeviceLost());
    }

    //Once delay is up, delete player game object
    IEnumerator DelayDeviceLost()
    {
        yield return new WaitForSeconds(0.005f);
        Destroy(this.gameObject);
    }

    //If 'Back' is pressed, also delete the game object (but no delay cause this doesn't break things for some reason)
    void OnRemoveController()
    {
        Destroy(this.gameObject);
    }

    void OnExitMenu()
    {
        FindObjectOfType<UIManager>().ReturnMainFromMulti();
    }
}
