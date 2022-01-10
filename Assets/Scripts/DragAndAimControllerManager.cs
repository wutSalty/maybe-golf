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

    //UI stuff
    public GameObject PauseUI;
    public Selectable ResumeButton;

    private Vector2 LeftMove;
    private int PlayerIndex;

    //Allows Spawn Ball to access sprites
    public SpriteRenderer BallSprite;
    public SpriteMask spriteMask;
    public SpriteRenderer insideSprite;

    //When awake, grab the things we need
    private void Awake()
    {
        playerInput = gameObject.GetComponent<PlayerInput>();
    }

    //When start, grab references to these stuff
    private void Start()
    {
        PlayerIndex = playerInput.playerIndex;
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
        BroadcastMessage("OnRestartBall");
    }

    //When 'esc' is pressed
    public void OnMenuMouse()
    {
        if (GameStatus.gameStat.GameOver)
        {
            return;
        }

        PauseGame.pM.ButtonClickOverrideCauseImLazy(PlayerIndex); //Set global pause
        if (PauseGame.pM.MenuIsOpen && ControllerDisconnectPause.ControlDC.CurrentlyDC == false) //Then do local menu
        {
            //Hijack input actions
            SetToUI();

            //Pull up menu
            PauseUI.SetActive(true);
            eventSystem.SetSelectedGameObject(ResumeButton.gameObject);
            eventSystem.firstSelectedGameObject = ResumeButton.gameObject;
        } else if (ControllerDisconnectPause.ControlDC.CurrentlyDC == false)
        {
            SetToInGame();

            PauseUI.SetActive(false);
            eventSystem.SetSelectedGameObject(null);
        }
    }

    //When 'esc' pressed from in menu
    public void OnMenu()
    {
        if (GameStatus.gameStat.GameOver || GameStatus.gameStat.ForcePause)
        {
            return;
        }

        PauseGame.pM.ButtonClickOverrideCauseImLazy(PlayerIndex);
        if (PauseGame.pM.MenuIsOpen && ControllerDisconnectPause.ControlDC.CurrentlyDC == false)
        {
            SetToUI();

            PauseUI.SetActive(true);
            eventSystem.firstSelectedGameObject = ResumeButton.gameObject;
        }
        else if (ControllerDisconnectPause.ControlDC.CurrentlyDC == false)
        {
            SetToInGame();

            PauseUI.SetActive(false);
            eventSystem.SetSelectedGameObject(null);
        }
    }

    //Changes input stuff
    public void SetToUI()
    {
        inputModule.point = uiPoint;
        inputModule.leftClick = uiLeftClick;
        inputModule.middleClick = uiMiddleClick;
        inputModule.rightClick = uiRightClick;
        inputModule.scrollWheel = uiScrollWheel;
    }

    public void SetToInGame()
    {
        inputModule.point = InGamePoint;
        inputModule.leftClick = InGameLeftClick;
        inputModule.middleClick = InGameMiddleClick;
        inputModule.rightClick = InGameRightClick;
        inputModule.scrollWheel = InGameScrollWheel;
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
