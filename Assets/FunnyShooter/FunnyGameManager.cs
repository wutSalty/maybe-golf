using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class FunnyGameManager : MonoBehaviour
{
    public FunnyPlayerHealth playerHealth;
    public PlayerUpgradesScript upgradeScript;
    public RandomSpawning randomSpawning;
    public PlayerInput pInput;

    public bool GameIsActive { get; private set; } = true;

    public GameObject ResultsCanvas;
    public Text WinnerText;
    public Text TimeLastedText;
    public Text WellDoneText;
    public GameObject DefaultButton;

    public EventSystem eventSys;

    private float FinalTime;

    private Animator canvasAnim;

    private void Start()
    {
        canvasAnim = ResultsCanvas.GetComponentInParent<Animator>();
    }

    public void CheckTime()
    {
        GameIsActive = false;
        upgradeScript.ForceCloseShop();
        FinalTime = randomSpawning.currentTime;

        pInput.SwitchCurrentActionMap("Menu");

        StartCoroutine(TimingAfterPlayerDies());
    }

    private IEnumerator TimingAfterPlayerDies()
    {
        yield return new WaitForSeconds(3f);

        DoEverythingElse();
    }

    private void DoEverythingElse()
    {
        GameIsActive = false;
        Time.timeScale = 0;

        if (FinalTime >= 30 * 60f)
        {
            PlayerHasWon();
        }
        else
        {
            PlayerHasLost();
        }

        canvasAnim.Play("Result_Open");
        eventSys.firstSelectedGameObject = DefaultButton;
        eventSys.SetSelectedGameObject(DefaultButton);
    }

    private void PlayerHasWon()
    {
        WinnerText.text = "Bonus Stage Cleared";
        TimeLastedText.text = "You survived for 30 minutes!";
        WellDoneText.text = "Thank you for playing this extra bonus mode!";
    }

    private void PlayerHasLost()
    {
        int minutes = Mathf.FloorToInt(FinalTime / 60);
        int seconds = Mathf.FloorToInt(FinalTime - minutes * 60);

        WinnerText.text = "Bonus Stage Failed";
        TimeLastedText.text = "You lasted for " + minutes.ToString() + " minutes and " + seconds.ToString() + " seconds.";
        WellDoneText.text = "Why not try again and make it to 30 minutes.";
    }

    public void ReturnToMain()
    {
        AudioManager.instance.PlaySound("UI_beep");
        LoadingScreen.loadMan.LoadingMusic("MainMenu", false, "BGM_title");
    }

    public void RestartGame()
    {
        AudioManager.instance.PlaySound("UI_beep");
        LoadingScreen.loadMan.LoadingMusic("FunnyShooter", false, "BGM_boss");
    }
}
