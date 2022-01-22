using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class BossStatus : MonoBehaviour
{
    public static BossStatus bossStat;

    public bool GameStart;
    public bool GameOver;
    public bool ForcePause;

    private PlayerInput[] playerInputs;
    private PlayerInput PlayerOneInput;
    private MultiplayerEventSystem PlayerOneEvent;
    private PlayerInputManager inputManager;

    private GameObject BossObject;
    private PlayerHealth BossHealth;

    public int GMLevelIndex = 5;

    float Timer;

    private void Awake()
    {
        if (bossStat != null && bossStat != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            bossStat = this;
        }
    }

    private void Start()
    {
        playerInputs = FindObjectsOfType<PlayerInput>();
        inputManager = GetComponent<PlayerInputManager>();
        BossObject = FindObjectOfType<BossShooting>().gameObject;
        BossHealth = BossObject.GetComponent<PlayerHealth>();

        foreach (var item in playerInputs)
        {
            if (item.playerIndex == 0)
            {
                PlayerOneInput = item;
                PlayerOneEvent = item.gameObject.GetComponent<MultiplayerEventSystem>();
            }
        }

        int BossHP = 100 * inputManager.playerCount;
        //Get Boss and dump HP into its max health
        BossHealth.healthBar.SetMaxHealth(BossHP);
        BossHealth.CurrentHealth = BossHP;

        //Temp Begin Game, hook into loading screen
        BeginGame();
    }

    public void BeginGame()
    {
        foreach (var item in playerInputs)
        {
            item.SwitchCurrentActionMap("In-Game");
        }

        GameStart = true;

        StartCoroutine(IncrementTimer());
    }

    IEnumerator IncrementTimer()
    {
        while (!GameOver)
        {
            if (!ForcePause)
            {
                Timer += Time.deltaTime;
                //Text here
            }
            yield return null;
        }
    }

    public void GameEnded()
    {

    }
}
