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

    //When start, grab the things we need
    private void Awake()
    {
        playerInput = gameObject.GetComponent<PlayerInput>();
        //restartScene = UIManager.GetComponent<RestartScene>();
        //escMenu = UIManager.GetComponent<EscMenu>();

        //OnControlsChanged(); !!!Not very priority but try and fix because of prefabs. Need to get reference to text when spawn
    }

    public void OnRestart()
    {
        RestartScene.DoTheRestart();
    }

    public void OnMenu()
    {
        PauseGame.pM.ButtonClickOverrideCauseImLazy(false);
    }

    public void OnMove(InputValue value)
    {
        PauseGame.pM.LeftMove = value.Get<Vector2>();
    }

    public void OnControlsChanged()
    {
        PauseGame.pM.ControlsHaveChanged(playerInput);
    }

    public void OnDeviceLost()
    {
        Debug.Log("Device has been lost");
    }
}
