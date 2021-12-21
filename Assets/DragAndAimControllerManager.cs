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

    //When 'r' is pressed
    public void OnRestart()
    {
        RestartScene.DoTheRestart();
    }

    //When 'esc' is pressed
    public void OnMenuMouse()
    {
        PauseGame.pM.ButtonClickOverrideCauseImLazy(true, PlayerIndex);
        if (PauseGame.pM.MenuIsOpen)
        {
            PauseUI.SetActive(true);
            eventSystem.SetSelectedGameObject(ResumeButton.gameObject);
            eventSystem.firstSelectedGameObject = ResumeButton.gameObject;
        } else
        {
            PauseUI.SetActive(false);
            eventSystem.SetSelectedGameObject(null);
        }
    }

    //When 'esc' pressed from in menu
    public void OnMenu()
    {
        PauseGame.pM.ButtonClickOverrideCauseImLazy(false, PlayerIndex);
        if (PauseGame.pM.MenuIsOpen)
        {
            PauseUI.SetActive(true);
        }
        else
        {
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
