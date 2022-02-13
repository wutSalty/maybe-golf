using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class BossPauseGame : MonoBehaviour
{
    public static BossPauseGame bossPause;

    public bool MenuIsOpen = false;

    private PlayerInput[] playerInputs;

    void Awake()
    {
        if (bossPause != null && bossPause != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            bossPause = this;
        }
    }

    private void Start()
    {
        playerInputs = FindObjectsOfType<PlayerInput>();
    }

    public void SetPause(int playerIndex)
    {
        foreach (var item in playerInputs)
        {
            if (MenuIsOpen == false)
            {
                if (item.playerIndex == playerIndex)
                {
                    item.SwitchCurrentActionMap("Menu");
                }
                else
                {
                    item.SwitchCurrentActionMap("Not Caller Menu");
                    item.GetComponent<MultiplayerEventSystem>().enabled = false;
                }
            }
            else
            {
                item.SwitchCurrentActionMap("In-Game");
                item.GetComponent<MultiplayerEventSystem>().enabled = true;
            }
        }

        if (MenuIsOpen == false)
        {
            Time.timeScale = 0;
            MenuIsOpen = true;
        }
        else
        {
            Time.timeScale = 1;
            MenuIsOpen = false;
        }
    }
}
