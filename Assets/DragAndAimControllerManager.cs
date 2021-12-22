using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class DragAndAimControllerManager : MonoBehaviour
{
    private PlayerInput playerInput; //player input to switch stuff
    public MultiplayerEventSystem eventSystem; //Multiplayer event system to set active object
    public InputSystemUIInputModule inputModule; //ui input system to hijack actions

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

    public GameObject PauseUI;
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

    //When start, grab references to these stuff
    private void Start()
    {
        PlayerIndex = playerInput.playerIndex;
        spawnLocation = gameObject.transform.position;
    }

    //On every frame, check these things
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

    //When 'r' is pressed
    public void OnRestart()
    {
        //RestartScene.DoTheRestart();
        BroadcastMessage("OnRestartBall");
        //gameObject.transform.position = spawnLocation;
    }

    //When 'esc' is pressed
    public void OnMenuMouse()
    {
        PauseGame.pM.ButtonClickOverrideCauseImLazy(PlayerIndex); //Set global pause
        if (PauseGame.pM.MenuIsOpen) //Then do local menu
        {
            //Hijack input actions
            //inputModule.point = uiPoint;
            //inputModule.leftClick = uiLeftClick;
            //inputModule.middleClick = uiMiddleClick;
            //inputModule.rightClick = uiRightClick;
            //inputModule.scrollWheel = uiScrollWheel;

            //Pull up menu
            PauseUI.SetActive(true);
            eventSystem.SetSelectedGameObject(ResumeButton.gameObject);
            eventSystem.firstSelectedGameObject = ResumeButton.gameObject;
        } else
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

    //When 'esc' pressed from in menu
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

    //When arrow keys pressed
    public void OnNavigate(InputValue value)
    {
        LeftMove = value.Get<Vector2>();
    }

    //When controller disconnected, but not used cause mouse/keyboard should always be there anyways
    public void OnDeviceLost()
    {
        Debug.Log("Device has been lost. Insert Pause here");
    }
}
