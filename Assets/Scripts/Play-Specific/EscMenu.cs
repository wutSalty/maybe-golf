using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

//Used to handle the global pause menu, now handles local pause menu
public class EscMenu : MonoBehaviour
{
    public PlayerInput playerInput;
    public Text CurrentPlayerText;

    private void Start()
    {
        CurrentPlayerText.text = "Paused By: Player " + (playerInput.playerIndex + 1);
    }

    //Returns to main menu
    public void ReturnToMain()
    {
        foreach (var item in FindObjectsOfType<MoveBall>())
        {
            item.EmergancyEscape();
        }
        Time.timeScale = 1;
        GameStatus.gameStat.ForcePause = true;
        GameManager.GM.NumPlayers.Clear();
        GameManager.GM.SingleMode = false;
        GameManager.GM.GhostMode = false;

        AudioManager.instance.PlaySound("UI_beep");
        LoadingScreen.loadMan.LoadingMusic("MainMenu", false, "BGM_title");
    }

    public void RestartScene()
    {
        foreach (var item in FindObjectsOfType<MoveBall>())
        {
            item.EmergancyEscape();
        }
        Time.timeScale = 1;
        GameStatus.gameStat.ForcePause = true;

        AudioManager.instance.PlaySound("UI_beep");
        LoadingScreen.loadMan.LoadingMusic(SceneManager.GetActiveScene().name, true, AudioManager.instance.CurrentlyPlayingBGM);
    }
}