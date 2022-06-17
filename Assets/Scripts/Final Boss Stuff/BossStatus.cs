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

    public Animator ResultsAnimator;

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
    private Animator BossAnimator;

    //Keeping track of level
    public int GMLevelIndex = 5;

    //Internal timer
    float Timer;

    //Result screen UI elements
    public GameObject ResultsScreen;
    public Text ResultsTitle;
    public Text ResultsText;
    public Button ResultsFirstButton;

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
        BossAnimator = bossShooting.GetComponentInParent<Animator>();

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

        BeginGame();
    }

    private void Update()
    {
        if (ForcePause)
        {
            BossAnimator.enabled = false;
        } else
        {
            BossAnimator.enabled = true;
        }
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
            if (item.playerIndex != 99 && !item.ZeroHP)
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
        GameOver = true;
        bossShooting.StopShooting();
        Debug.Log("Players are dead");

        float BossHPRemaining = (BossHealth.CurrentHealth * 1.0f / BossHealth.MaxHealth) * 100;
        Debug.Log(BossHPRemaining);

        ResultsTitle.text = "Boss Course Failed";
        ResultsText.text = "The boss remains victorious. " + Mathf.RoundToInt(BossHPRemaining) + "% of it's HP is left.\n\n";

        if (GameManager.GM.SingleMode)
        {
            //Singleplayer logic
            ResultsText.text += "Why not try again to get your revenge! Or feel free to return to the Level Select and Main Menu.";

            GameManager.GM.TimesPlayedSolo += 1;
        }
        else
        {
            //Multiplayer logic
            ResultsText.text += "Why not try again to get your revenge! Player One, feel free to make the calls. Or feel free to return to the Level Select and Main Menu.";

            GameManager.GM.TimesPlayedMulti += 1;
        }

        PlayerOneInput.SwitchCurrentActionMap("Menu");

        ResultsScreen.SetActive(true); //Show results screen
        ResultsAnimator.SetTrigger("ShowResults");

        //Hijacks player 1 input
        PlayerOneEvent.playerRoot = ResultsScreen;
        if (PlayerOneInput.currentControlScheme != "Mouse")
        {
            PlayerOneEvent.SetSelectedGameObject(ResultsFirstButton.gameObject);
        }
        PlayerOneEvent.firstSelectedGameObject = ResultsFirstButton.gameObject;

        GameManager.GM.CheckUnlockables();
        GameManager.GM.SavePlayer();
    }

    //Once the boss is killed, bring up the Results Screen
    public void BossDefeated()
    {
        GameOver = true;
        Debug.Log("Boss is dead");

        ResultsTitle.text = "Boss Defeated";
        ResultsText.text = "Congratulations on defeating the Boss! You managed to do it in " + Timer.ToString("F2") + " seconds!\n\n";

        if (GameManager.GM.SingleMode)
        {
            //Singleplayer logic
            ResultsText.text += "If you'd like to shave some more time to get a better score, feel free to try again! If not, you can always return to Level Select or Main Menu.";

            if (GameManager.GM.LevelData[GMLevelIndex].BestTime == 0) //First clear
            {
                GameManager.GM.LevelData[GMLevelIndex].BestTime = Timer;
                GameManager.GM.LevelData[5].CollectableGet = 2;
            }
            else if (GameManager.GM.LevelData[GMLevelIndex].BestTime > Timer) //Beat best time
            {
                GameManager.GM.LevelData[GMLevelIndex].BestTime = Timer;
            }
            else if (GameManager.GM.LevelData[GMLevelIndex].BestTime <= Timer) //Slower than best time
            {
                //Nothing lol
            }

            GameManager.GM.TimesPlayedSolo += 1;
        }
        else
        {
            //Multiplayer logic
            ResultsText.text += "If you'd like to shave some more time to get a better score, feel free to try again! If not, you can always return to Level Select or Main Menu. Feel free to make the call, Player One.";

            GameManager.GM.TimesPlayedMulti += 1;
        }

        PlayerOneInput.SwitchCurrentActionMap("Menu");

        ResultsScreen.SetActive(true); //Show results screen
        ResultsAnimator.SetTrigger("ShowResults");

        //Hijacks player 1 input
        PlayerOneEvent.playerRoot = ResultsScreen;
        if (PlayerOneInput.currentControlScheme != "Mouse")
        {
            PlayerOneEvent.SetSelectedGameObject(ResultsFirstButton.gameObject);
        }
        PlayerOneEvent.firstSelectedGameObject = ResultsFirstButton.gameObject;

        //Disable other player's inputs
        foreach (var item in playerInputs)
        {
            if (item.playerIndex != 0)
            {
                item.GetComponent<MultiplayerEventSystem>().enabled = false;
            }
        }

        GameManager.GM.CheckUnlockables();
        GameManager.GM.SavePlayer();
    }

    //Butten when restaring the course
    public void RestartScene()
    {
        //LoadingScreen.loadMan.BeginLoadingScene(SceneManager.GetActiveScene().name, false);
        AudioManager.instance.PlaySound("UI_beep");
        LoadingScreen.loadMan.LoadingMusic(SceneManager.GetActiveScene().name, false, "BGM_boss");
    }

    //Button for returning to main menu
    public void ReturnToMain()
    {
        GameManager.GM.SingleMode = false;
        GameManager.GM.GhostMode = false;
        GameManager.GM.NumPlayers.Clear();
        AudioManager.instance.PlaySound("UI_beep");
        //LoadingScreen.loadMan.BeginLoadingScene("MainMenu", false);
        LoadingScreen.loadMan.LoadingMusic("MainMenu", false, "BGM_title");
    }

    //Button for returning to stage select
    public void StageSelect()
    {
        GameManager.GM.LoadIntoLevelSelect = true;
        AudioManager.instance.PlaySound("UI_beep");
        //LoadingScreen.loadMan.BeginLoadingScene("MainMenu", false);
        LoadingScreen.loadMan.LoadingMusic("MainMenu", false, "BGM_title");
    }
}
