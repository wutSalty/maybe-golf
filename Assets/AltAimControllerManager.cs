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
    public InputSystemUIInputModule inputModule;

    public InputActionAsset inputAsset;

    //Pointing and clicking for in-game HUD
    public InputActionReference InGamePoint;
    public InputActionReference InGameLeftClick;
    public InputActionReference InGameMiddleClick;
    public InputActionReference InGameRightClick;
    public InputActionReference InGameScrollWheel;

    //Pointing and clicking for menu
    public InputActionReference uiPoint;
    public InputActionReference uiLeftClick;
    public InputActionReference uiMiddleClick;
    public InputActionReference uiRightClick;
    public InputActionReference uiScrollWheel;
    public InputActionReference uiNavigate;
    public InputActionReference uiSubmit;
    public InputActionReference uiCancel;

    public GameObject PauseUI;
    public Canvas HUDUi;
    public Selectable ResumeButton;

    private Vector2 LeftMove;
    private int PlayerIndex;

    public SpriteRenderer BallSprite;
    public SpriteMask spriteMask;
    public SpriteRenderer insideSprite;

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
            HUDUi.sortingOrder = 19;
        }

        //Screams into the void. Bug in Prefabs where UI Input Module clears itself upon instantiation. This should fix that
        //playerInput.actions = inputAsset;

        //inputModule.actionsAsset = inputAsset;

        //inputModule.point = InGamePoint;
        //inputModule.leftClick = InGameLeftClick;
        //inputModule.middleClick = InGameMiddleClick;
        //inputModule.rightClick = InGameRightClick;
        //inputModule.scrollWheel = InGameScrollWheel;
        //inputModule.move = uiNavigate;
        //inputModule.submit = uiSubmit;
        //inputModule.cancel = uiCancel;

        //StartCoroutine(FixInput());
    }

    IEnumerator FixInput()
    {
        playerInput.uiInputModule.enabled = false;
        yield return new WaitForSeconds(0.005f);
        playerInput.uiInputModule.enabled = true;

        inputModule.point = InGamePoint;
        inputModule.leftClick = InGameLeftClick;
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
        //RestartScene.DoTheRestart();
        BroadcastMessage("OnRestartBall");
        //gameObject.transform.position = spawnLocation;
    }

    //When 'Menu' pressed
    public void OnMenu()
    {
        PauseGame.pM.ButtonClickOverrideCauseImLazy(PlayerIndex);
        if (PauseGame.pM.MenuIsOpen)
        {
            //inputModule.point = uiPoint;
            //inputModule.leftClick = uiLeftClick;
            //inputModule.middleClick = uiMiddleClick;
            //inputModule.rightClick = uiRightClick;
            //inputModule.scrollWheel = uiScrollWheel;

            PauseUI.SetActive(true);
            eventSystem.SetSelectedGameObject(ResumeButton.gameObject);
            eventSystem.firstSelectedGameObject = ResumeButton.gameObject;
        }
        else
        {
            //inputModule.point = InGamePoint;
            //inputModule.leftClick = InGameLeftClick;
            //inputModule.middleClick = InGameMiddleClick;
            //inputModule.rightClick = InGameRightClick;
            //inputModule.scrollWheel = InGameScrollWheel;

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
