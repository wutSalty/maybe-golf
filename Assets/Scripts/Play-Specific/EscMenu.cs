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
        Time.timeScale = 1;
        PauseGame.pM.MenuIsOpen = false;

        //Reset NumPlayers from GM
        GameManager.GM.NumPlayers.Clear();

        SceneManager.LoadScene("MainMenu");
    }
}