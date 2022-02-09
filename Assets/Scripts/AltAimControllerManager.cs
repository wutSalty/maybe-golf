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
    public MultiplayerEventSystem eventSystem; //the input system

    //Any UI components
    public GameObject PauseUI;
    public Selectable ResumeButton;
    public Text ControlsText;

    private Vector2 LeftMove;
    private int PlayerIndex;

    //Hooked up here so SpawnBall can set the sprites and layers
    public SpriteRenderer BallSprite;
    public SpriteMask spriteMask;
    public SpriteRenderer insideSprite;

    //When awake, grab the things we need
    private void Awake()
    {
        playerInput = gameObject.GetComponent<PlayerInput>();
    }

    //When start, set up more things
    private void Start()
    {
        PlayerIndex = playerInput.playerIndex;

        if (playerInput.currentControlScheme == "Keyboard")
        {
            ControlsText.text = "Aim - Arrow Keys LEFT/RIGHT\nPower - Arrow Keys UP/DOWN\nShoot - SPACE\nRestart Position - R\nPause Game - ESC";
            //HUDUi.sortingOrder = 19;
        } else
        {
            ControlsText.text = "Aim - Left Stick LEFT/RIGHT\nPower - Right Stick UP/DOWN\nShoot - A/Submit Button\nRestart Position - LB + RB\nPause Game - Menu Button";
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
        if (GameStatus.gameStat.GameOver || GameStatus.gameStat.ForcePause || GameStatus.gameStat.DialogueOpen)
        {
            return;
        }

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

        AudioManager.instance.PlaySound("UI_beep");
    }

    //When 'left stick' moved
    public void OnNavigate(InputValue value)
    {
        LeftMove = value.Get<Vector2>();
    }

    //When control type changed(should only work in singleplayer)
    public void OnControlsChanged(PlayerInput pInput)
    {
        if (pInput.currentControlScheme == "Keyboard")
        {
            ControlsText.text = "Aim - Arrow Keys LEFT/RIGHT\nPower - Arrow Keys UP/DOWN\nShoot - SPACE\nRestart Position - R\nPause Game - ESC";
        }
        else
        {
            ControlsText.text = "Aim - Aim - Left Stick LEFT/RIGHT\nPower - Right Stick UP/DOWN\nShoot - A/Submit Button\nRestart Position - LB + RB\nPause Game - Menu Button";
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
