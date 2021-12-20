using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AltAimControllerManager : MonoBehaviour
{
    private PlayerInput playerInput; //player input to switch stuff

    public Text PauseText; //the text for the pause button
    public Text RestartText; //text for restart button
    public GameObject UIManager; //the UIManager
    //private RestartScene restartScene; //script for restarting scene
    //private EscMenu escMenu; //script for escape menu

    public int PlayerIndex;

    //When awake, grab the things we need
    private void Awake()
    {
        playerInput = gameObject.GetComponent<PlayerInput>();
        //restartScene = UIManager.GetComponent<RestartScene>();
        //escMenu = UIManager.GetComponent<EscMenu>();

        //OnControlsChanged(); !!!Not very priority but try and fix because of prefabs. Need to get reference to text when spawn
    }

    //When start, grab playerindex (we need this for disconnect purposes)
    private void Start()
    {
        PlayerIndex = playerInput.playerIndex;
    }

    //When L+R pressed
    public void OnRestart()
    {
        RestartScene.DoTheRestart();
    }

    //When 'Menu' pressed
    public void OnMenu()
    {
        PauseGame.pM.ButtonClickOverrideCauseImLazy(false, playerInput.playerIndex);
    }

    //When 'left stick' moved
    public void OnNavigate(InputValue value)
    {
        PauseGame.pM.LeftMove = value.Get<Vector2>();
    }

    //When control type changed (should only work in singleplayer)
    public void OnControlsChanged()
    {
        PauseGame.pM.ControlsHaveChanged(playerInput);
    }

    //When controller has disconnected, make a call
    public void OnDeviceLost()
    {
        ControllerDisconnectPause.ControlDC.ControllerDisconnected(PlayerIndex);
    }

    //When controller re-connected, also make a call
    public void OnDeviceRegained()
    {
        ControllerDisconnectPause.ControlDC.ControllerConnected(PlayerIndex);
    }

}
