using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class FunnyPlayerPause : MonoBehaviour
{
    public FunnyGameManager gameMan;
    private PlayerUpgradesScript upgradesScript;

    public EventSystem eventSys;
    public GameObject PausePanel;
    public GameObject ResumeButton;

    private PlayerInput pInput;

    private Vector2 LeftMove;
    [HideInInspector] public bool gamePaused = false;

    private Animator canvasAnim;

    private void Start()
    {
        pInput = GetComponent<PlayerInput>();
        upgradesScript = GetComponent<PlayerUpgradesScript>();
        canvasAnim = PausePanel.GetComponentInParent<Animator>();
    }

    private void Update()
    {
        if (eventSys.firstSelectedGameObject == null)
        {
            eventSys.firstSelectedGameObject = eventSys.currentSelectedGameObject;
        }

        if ((LeftMove != Vector2.zero) && (eventSys.currentSelectedGameObject != null))
        {
            eventSys.firstSelectedGameObject = eventSys.currentSelectedGameObject;
        }

        if ((LeftMove != Vector2.zero) && (eventSys.currentSelectedGameObject == null || eventSys.currentSelectedGameObject.activeSelf))
        {
            eventSys.SetSelectedGameObject(eventSys.firstSelectedGameObject);
        }
    }

    private void OnMoving(InputValue value)
    {
        LeftMove = value.Get<Vector2>();
    }

    private void OnMenu()
    {
        if (upgradesScript.shopOpened || !gameMan.GameIsActive)
        {
            return;
        }

        AudioManager.instance.PlaySound("UI_beep");
        CheckPauseGame();
    }

    public void CheckPauseGame()
    {
        if (gamePaused)
        {
            Time.timeScale = 1;
            gamePaused = false;
            pInput.SwitchCurrentActionMap("Game");
            eventSys.SetSelectedGameObject(null);
            eventSys.firstSelectedGameObject = null;
            canvasAnim.Play("Pause_Close");
        }
        else
        {
            Time.timeScale = 0;
            gamePaused = true;
            pInput.SwitchCurrentActionMap("Menu");
            canvasAnim.Play("Pause_Open");
            eventSys.firstSelectedGameObject = ResumeButton;

            if (pInput.currentControlScheme == "Controller")
            {
                eventSys.SetSelectedGameObject(ResumeButton);
            }
        }
    }

    public void ReturnToMain()
    {
        AudioManager.instance.PlaySound("UI_beep");
        LoadingScreen.loadMan.LoadingMusic("MainMenu", false, "BGM_title");
    }
}
