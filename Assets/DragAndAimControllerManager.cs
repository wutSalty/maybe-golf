using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DragAndAimControllerManager : MonoBehaviour
{
    private PlayerInput playerInput; //player input to switch stuff
    public Text PauseText; //the text for the pause button
    public Text RestartText; //text for restart button

    public GameObject UIManager; //the UIManager
    //private RestartScene restartScene; //script for restarting scene
    //private EscMenu escMenu; //script for escape menu

    //When awake, grab the things we need
    private void Awake()
    {
        playerInput = gameObject.GetComponent<PlayerInput>();
        //restartScene = UIManager.GetComponent<RestartScene>();
        //escMenu = UIManager.GetComponent<EscMenu>();
    }

    //When 'r' is pressed
    public void OnRestart()
    {
        RestartScene.DoTheRestart();
    }

    //When 'esc' is pressed
    public void OnMenuMouse()
    {
        PauseGame.pM.ButtonClickOverrideCauseImLazy(true, playerInput.playerIndex);
    }

    //When arrow keys pressed
    public void OnNavigate(InputValue value)
    {
        PauseGame.pM.LeftMove = value.Get<Vector2>();
    }

    //When controller disconnected, but not used cause mouse/keyboard should always be there anyways
    public void OnDeviceLost()
    {
        Debug.Log("Device has been lost. Insert Pause here");
    }
}
