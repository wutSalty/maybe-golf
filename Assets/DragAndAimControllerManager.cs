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

    //When start, grab the things we need
    private void Awake()
    {
        playerInput = gameObject.GetComponent<PlayerInput>();
        //restartScene = UIManager.GetComponent<RestartScene>();
        //escMenu = UIManager.GetComponent<EscMenu>();
    }

    public void OnRestart()
    {
        RestartScene.DoTheRestart();
    }

    public void OnMenuMouse()
    {
        PauseGame.pM.ButtonClickOverrideCauseImLazy(true);
    }

    public void OnMenu()
    {
        PauseGame.pM.ButtonClickOverrideCauseImLazy(false);
    }

    public void OnMove(InputValue value)
    {
        PauseGame.pM.LeftMove = value.Get<Vector2>();
    }
}
