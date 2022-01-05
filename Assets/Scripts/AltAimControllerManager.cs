using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class AltAimControllerManager : MonoBehaviour
{
    private PlayerInput playerInput; //player input to switch stuff
    public MultiplayerEventSystem eventSystem;

    public GameObject PauseUI;
    public Canvas HUDUi;
    public Selectable ResumeButton;
    public Text ControlsText;

    private Vector2 LeftMove;
    private int PlayerIndex;

    public SpriteRenderer BallSprite;
    public SpriteMask spriteMask;
    public SpriteRenderer insideSprite;

    public Text pauseText;

    private Vector3 spawnLocation;

    //When awake, grab the things we need
    private void Awake()
    {
        playerInput = gameObject.GetComponent<PlayerInput>();
    }

    //When start, set up more things
    private void Start()
    {
        PlayerIndex = playerInput.playerIndex;
        spawnLocation = gameObject.transform.position;

        if (playerInput.currentControlScheme == "Keyboard")
        {
            ControlsText.text = "Aim - Arrow Keys Up/Down\nPower - Arrow Keys Left/Right\nShoot - Enter\nRestart Position - R\nPause Game - ESC";
            //HUDUi.sortingOrder = 19;
        } else
        {
            ControlsText.text = "Aim - Left Stick Left/Right\nPower - Right Stick Up/Down\nShoot - A/Submit Button\nRestart Position - LB + RB\nPause Game - Menu Button";
        }
        
    }

    public void Update()
    {
        if ((LeftMove != Vector2.zero) && (eventSystem.currentSelectedGameObject != null))
        {
            eventSystem.firstSelectedGameObject = eventSystem.currentSelectedGameObject;
        }

        if ((LeftMove != Vector2.zero) && (eventSystem.currentSelectedGameObject == null || eventSystem.currentSelectedGameObject.activeSelf))
        {
            eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
        }
    }

    //When L+R pressed
    public void OnRestart()
    {
        BroadcastMessage("OnRestartBall");
    }

    //When 'Menu' pressed
    public void OnMenu()
    {
        PauseGame.pM.ButtonClickOverrideCauseImLazy(PlayerIndex);
        if (PauseGame.pM.MenuIsOpen && ControllerDisconnectPause.ControlDC.CurrentlyDC == false)
        {
            PauseUI.SetActive(true);
            eventSystem.SetSelectedGameObject(ResumeButton.gameObject);
            eventSystem.firstSelectedGameObject = ResumeButton.gameObject;
        }
        else if (ControllerDisconnectPause.ControlDC.CurrentlyDC == false)
        {
            PauseUI.SetActive(false);
            eventSystem.SetSelectedGameObject(null);
        }
    }

    //When 'left stick' moved
    public void OnNavigate(InputValue value)
    {
        LeftMove = value.Get<Vector2>();
    }

    //When control type changed(should only work in singleplayer)
    public void OnControlsChanged()
    {
        if (playerInput.currentControlScheme == "Keyboard")
        {
            ControlsText.text = "Aim - Arrow Keys Up/Down\nPower - Arrow Keys Left/Right\nShoot - Enter\nRestart Position - R\nPause Game - ESC";
        }
        else
        {
            ControlsText.text = "Aim - Left Stick Left/Right\nPower - Right Stick Up/Down\nShoot - A/Submit Button\nRestart Position - LB + RB\nPause Game - Menu Button";
        }
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
