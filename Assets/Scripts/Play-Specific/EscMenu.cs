using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class EscMenu : MonoBehaviour
{
    private PlayerInput playerInput;
    public Text CurrentPlayerText;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

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
        //PauseGame.pM.MenuIsOpen = false;
        GameStatus.gameStat.GameOver = true;
        GameManager.GM.NumPlayers.Clear();
        LoadingScreen.loadMan.BeginLoadingScene("MainMenu", false);
    }
}