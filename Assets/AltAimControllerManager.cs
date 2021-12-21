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
    public Selectable ResumeButton;

    private Vector2 LeftMove;
    private int PlayerIndex;

    public SpriteRenderer BallSprite;
    public SpriteMask spriteMask;
    public SpriteRenderer insideSprite;

    //When awake, grab the things we need
    private void Awake()
    {
        playerInput = gameObject.GetComponent<PlayerInput>();
    }

    //When start, grab playerindex (we need this for disconnect purposes)
    private void Start()
    {
        PlayerIndex = playerInput.playerIndex;
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
        RestartScene.DoTheRestart();
    }

    //When 'Menu' pressed
    public void OnMenu()
    {
        PauseGame.pM.ButtonClickOverrideCauseImLazy(true, PlayerIndex);
        if (PauseGame.pM.MenuIsOpen)
        {
            PauseUI.SetActive(true);
            eventSystem.SetSelectedGameObject(ResumeButton.gameObject);
            eventSystem.firstSelectedGameObject = ResumeButton.gameObject;
        }
        else
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
