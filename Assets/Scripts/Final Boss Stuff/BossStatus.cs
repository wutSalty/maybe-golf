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

    //Flags for keeping track of game status
    public bool GameStart;
    public bool GameOver;
    public bool ForcePause;

    //Whatever input systems
    private PlayerInput[] playerInputs;
    private PlayerInput PlayerOneInput;
    private MultiplayerEventSystem PlayerOneEvent;
    private PlayerInputManager inputManager;

    //Boss scripts
    private PlayerHealth BossHealth;
    private BossShooting bossShooting;

    //Keeping track of level
    public int GMLevelIndex = 5;

    //Internal timer
    float Timer;

    //Result screen UI elements
    public GameObject ResultsScreen;
    public Text ResultsText;
    public Button ResultsFirstButton;

    //Death screen UI elements
    public GameObject LoseScreen;
    public Button LoseFirstButton;

    //Holds status about each player to make sure they haven't died yet
    [System.Serializable]
    public class PlayerStat
    {
        public int playerIndex;
        public bool ZeroHP = false;
    }
    public List<PlayerStat> playerStatus;

    private void Awake()
    {
        if (bossStat != null && bossStat != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            bossStat = this;

            playerStatus.Add(new PlayerStat { playerIndex = 99 });
            playerStatus.Add(new PlayerStat { playerIndex = 99 });
            playerStatus.Add(new PlayerStat { playerIndex = 99 });
            playerStatus.Add(new PlayerStat { playerIndex = 99 });
        }
    }

    //When start, grab required components, apply boss HP
    private void Start()
    {
        playerInputs = FindObjectsOfType<PlayerInput>();
        inputManager = GetComponent<PlayerInputManager>();
        bossShooting = FindObjectOfType<BossShooting>();
        BossHealth = bossShooting.GetComponent<PlayerHealth>();

        foreach (var item in playerInputs)
        {
            playerStatus[item.playerIndex].playerIndex = item.playerIndex;

            if (item.playerIndex == 0)
            {
                PlayerOneInput = item;
                PlayerOneEvent = item.gameObject.GetComponent<MultiplayerEventSystem>();
            }
        }

        int BossHP = 150 * inputManager.playerCount;
        //Get Boss and dump HP into its max health
        BossHealth.healthBar.SetMaxHealth(BossHP);
        BossHealth.CurrentHealth = BossHP;
        BossHealth.MaxHealth = BossHP;

        //Temp Begin Game, hook into loading screen
        BeginGame();
    }

    //When the loading screen finishes loading, then start the game
    public void BeginGame()
    {
        foreach (var item in playerInputs)
        {
            item.SwitchCurrentActionMap("In-Game");
        }

        GameStart = true;
        bossShooting.PhaseA = true;
        StartCoroutine(IncrementTimer());
    }

    //Timer
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

    public void UpdatePlayerStatus(int pIndex)
    {
        playerStatus[pIndex].ZeroHP = true;

        GameOver = true;
        foreach (var item in playerStatus)
        {
            if (item.playerIndex != 99)
            {
                GameOver = false;
            }
        }

        if (GameOver)
        {
            PlayersAllDead();
        }
    }

    public void PlayersAllDead()
    {
        Debug.Log("Players are dead");
    }

    //Once the boss is killed, bring up the Results Screen
    public void BossDefeated()
    {
        GameOver = true;
        Debug.Log("Boss is dead");
        if (GameManager.GM.SingleMode)
        {
            //Singleplayer logic
        }
        else
        {
            //Multiplayer logic
        }

        PlayerOneInput.SwitchCurrentActionMap("Menu");

        ResultsScreen.SetActive(true); //Show results screen

        //Hijacks player 1 input
        PlayerOneEvent.playerRoot = ResultsScreen;
        if (PlayerOneInput.currentControlScheme != "Mouse")
        {
            PlayerOneEvent.SetSelectedGameObject(ResultsFirstButton.gameObject);
        }
        PlayerOneEvent.firstSelectedGameObject = ResultsFirstButton.gameObject;
    }

    //Butten when restaring the course
    public void RestartScene()
    {
        LoadingScreen.loadMan.BeginLoadingScene(SceneManager.GetActiveScene().name, false);
    }

    //Button for returning to main menu
    public void ReturnToMain()
    {
        GameManager.GM.SingleMode = false;
        GameManager.GM.GhostMode = false;
        GameManager.GM.NumPlayers.Clear();
        LoadingScreen.loadMan.BeginLoadingScene("MainMenu", false);
    }

    //Button for returning to stage select
    public void StageSelect()
    {
        GameManager.GM.LoadIntoLevelSelect = true;
        LoadingScreen.loadMan.BeginLoadingScene("MainMenu", false);
    }
}
