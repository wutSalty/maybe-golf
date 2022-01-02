using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class GameStatus : MonoBehaviour
{
    public static GameStatus gameStat;

    [System.Serializable]
    public class PlayerStatus
    {
        public bool Completed = false;
        public float Time;
        public int NumHits;
        public int playerIndex;
    }

    public List<PlayerStatus> playerStatuses; //List of players
    private float Timer; //The time
    private bool GameOver; //Has the game ended yet
    private bool SingleMode = false; //Are we playing in singleplayer mode
    private PlayerInput InputOne;
    private MultiplayerEventSystem POneUISys;

    public int GMLevelIndex; //The level's index in GM
    public Text timertext;
    public Button ReturnButton;

    public GameObject ResultsCanvas;

    //Make sure there's only one GameStat
    void Awake()
    {
        if (gameStat != null && gameStat != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            gameStat = this;

            gameStat.playerStatuses.Add(new PlayerStatus { playerIndex = 99 });
            gameStat.playerStatuses.Add(new PlayerStatus { playerIndex = 99 });
            gameStat.playerStatuses.Add(new PlayerStatus { playerIndex = 99 });
            gameStat.playerStatuses.Add(new PlayerStatus { playerIndex = 99 });
        }
    }

    //Get required data from GM
    private void Start()
    {
        foreach (var item in GameManager.GM.NumPlayers)
        {
            if (item.PlayerIndex != 99)
            {
                gameStat.playerStatuses[item.PlayerIndex].playerIndex = item.PlayerIndex;
            }
        }
        SingleMode = GameManager.GM.SingleMode;

        PlayerInput[] inputs = FindObjectsOfType<PlayerInput>();
        foreach (var item in inputs)
        {
            if (item.playerIndex == 0)
            {
                InputOne = item;
                POneUISys = item.gameObject.GetComponent<MultiplayerEventSystem>();
            }
        }
    }

    //Increment timer every frame
    private void Update()
    {
        if (gameStat.GameOver == false)
        {
            gameStat.Timer += Time.deltaTime;
            timertext.text = "Timer: " + gameStat.Timer.ToString("F2");
        }
    }

    //When flag is hit, pass parametres into list
    public void SubmitRecord(int pIndex, int NumHits, MoveBall ball)
    {
        gameStat.playerStatuses[pIndex].Completed = true;
        gameStat.playerStatuses[pIndex].NumHits = NumHits;
        gameStat.playerStatuses[pIndex].Time = gameStat.Timer;

        ball.UpdateTimerText(gameStat.playerStatuses[pIndex].Time);

        gameStat.CheckStatus();
    }

    //Check whether every player has completed yet or not
    public void CheckStatus()
    {
        gameStat.GameOver = true;
        foreach (var item in gameStat.playerStatuses)
        {
            if (item.playerIndex != 99 && item.Completed == false)
            {
                gameStat.GameOver = false;
            }
        }

        if (gameStat.GameOver)
        {
            EndGame();
        }
    }

    //Start game ending things
    public void EndGame()
    {
        Debug.Log("All players finished");
        if (SingleMode)
        {
            var ThisTime = gameStat.playerStatuses[0].Time;
            var ThisHits = gameStat.playerStatuses[0].NumHits;

            var GMLevel = GameManager.GM.LevelData[GMLevelIndex];

            if (ThisTime < GMLevel.BestTime || GMLevel.BestTime == 0)
            {
                GMLevel.BestTime = ThisTime;
                Debug.Log("New Time Record");
            }

            if (ThisHits < GMLevel.BestHits || GMLevel.BestHits == 0)
            {
                GMLevel.BestHits = ThisHits;
                Debug.Log("New Hits Record");
            }

            GameManager.GM.SavePlayer();

        } else
        {
            //maybe add some random stats for multiplayer or something

        }

        if (InputOne.gameObject.TryGetComponent(out DragAndAimControllerManager manager))
        {
            manager.SetToUI();
        }

        InputOne.SwitchCurrentActionMap("UI");

        ResultsCanvas.SetActive(true);

        POneUISys.playerRoot = ResultsCanvas;
        //ReturnButton.Select();

        POneUISys.firstSelectedGameObject = ReturnButton.gameObject;

        if (InputOne.currentControlScheme != "Mouse")
        {
            POneUISys.SetSelectedGameObject(ReturnButton.gameObject);
        }
    }

    public void RestartScene()
    {
        Debug.Log("Button has been pressed");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMain()
    {
        Debug.Log("Button has been pressed");
        GameManager.GM.NumPlayers.Clear();
        SceneManager.LoadScene("MainMenu");
    }

    public void StageSelect()
    {
        Debug.Log("this is still broken lol, come back later");
    }
}
