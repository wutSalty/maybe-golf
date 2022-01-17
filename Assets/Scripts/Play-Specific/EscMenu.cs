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
    private PlayerInput playerInput;
    public Text CurrentPlayerText;

    private void Start()
    {
        if (gameObject.TryGetComponent(out PlayerInput manager))
        {
            playerInput = manager;
            CurrentPlayerText.text = "Paused By: Player " + (playerInput.playerIndex + 1);
        }
    }

    //Returns to main menu
    public void ReturnToMain()
    {
        foreach (var item in FindObjectsOfType<MoveBall>())
        {
            item.EmergancyEscape();
        }
        Time.timeScale = 1;
        //PauseGame.pM.MenuIsOpen = false;
        GameStatus.gameStat.ForcePause = true;
        GameManager.GM.NumPlayers.Clear();
        GameManager.GM.SingleMode = false;
        GameManager.GM.GhostMode = false;
        LoadingScreen.loadMan.BeginLoadingScene("MainMenu", false);
    }

    public void RestartScene()
    {
        foreach (var item in FindObjectsOfType<MoveBall>())
        {
            item.EmergancyEscape();
        }
        Time.timeScale = 1;
        GameStatus.gameStat.ForcePause = true;
        LoadingScreen.loadMan.BeginLoadingScene(SceneManager.GetActiveScene().name, true);
    }
}