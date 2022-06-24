using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class PlayerPause : MonoBehaviour
{
    PlayerInput playerInput;
    MultiplayerEventSystem eventSystem;
    Vector2 LeftMove;
    int pIndex;

    public Animator PauseAnimator;

    public GameObject PauseUI;
    public Selectable ResumeButton;
    public Text PausedByText;
    public Text ControlsText;
    [TextArea(5, 10)]
    public string[] WhatShouldControlsSay;

    void Start()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        pIndex = playerInput.playerIndex;
        eventSystem = GetComponentInParent<MultiplayerEventSystem>();
        PausedByText.text = "Paused By: Player " + (pIndex + 1);

        OnControlsChanged();
    }

    private void Update()
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

    private void OnNavigate(InputValue value)
    {
        LeftMove = value.Get<Vector2>();
    }

    public void OnMenu()
    {
        //If currently game over or force pause
        if (BossStatus.bossStat.GameOver || BossStatus.bossStat.ForcePause || PauseAnimator.GetCurrentAnimatorStateInfo(0).IsName("BossPauseOpen") || PauseAnimator.GetCurrentAnimatorStateInfo(0).IsName("BossPauseClose"))
        {
            return;
        }

        BossPauseGame.bossPause.SetPause(pIndex);

        if (BossPauseGame.bossPause.MenuIsOpen && BossControllerDisconnect.BossControlDC.CurrentlyDC == false)
        {
            PauseUI.SetActive(true);
            PauseAnimator.SetTrigger("OpenMenu");

            eventSystem.SetSelectedGameObject(ResumeButton.gameObject);
            eventSystem.firstSelectedGameObject = ResumeButton.gameObject;
        }
        else if (BossControllerDisconnect.BossControlDC.CurrentlyDC == false)
        {
            eventSystem.SetSelectedGameObject(null);

            PauseAnimator.SetTrigger("CloseMenu");
            //PauseUI.SetActive(false);

        }
        AudioManager.instance.PlaySound("UI_beep");
    }

    public void ApplyObject()
    {
        eventSystem.SetSelectedGameObject(ResumeButton.gameObject);
        eventSystem.firstSelectedGameObject = ResumeButton.gameObject;
    }

    void OnControlsChanged()
    {
        if (playerInput == null)
        {
            return;
        }

        string TheText = "";
        if (playerInput.currentControlScheme == "Keyboard")
        {
            TheText = WhatShouldControlsSay[0];
        }
        else if (playerInput.currentControlScheme == "Controller")
        {
            TheText = WhatShouldControlsSay[1];
        }
        else if (playerInput.currentControlScheme == "Mouse")
        {
            TheText = WhatShouldControlsSay[2];
        }
        ControlsText.text = TheText;
    }

    public void ReturnToMain()
    {
        //Find a way to pause everything without breaking anything. Or use unscaled time.
        AudioManager.instance.PlaySound("UI_beep");

        BossStatus.bossStat.ForcePause = true;
        GameManager.GM.NumPlayers.Clear();
        GameManager.GM.SingleMode = false;
        GameManager.GM.GhostMode = false;
        LoadingScreen.loadMan.LoadingMusic("MainMenu", false, "BGM_title");
    }

    public void RestartScene()
    {
        AudioManager.instance.PlaySound("UI_beep");

        BossStatus.bossStat.ForcePause = true;
        LoadingScreen.loadMan.LoadingMusic(SceneManager.GetActiveScene().name, false, AudioManager.instance.CurrentlyPlayingBGM);
    }
}
