using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

//Handles data for the current course session
public class GameStatus : MonoBehaviour
{
    public static GameStatus gameStat;

    //Holds data for each player and whether they've finished
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
    [HideInInspector]
    public bool GameOver; //Has the game ended yet
    [HideInInspector]
    public bool ForcePause = false; //Whether the dialogue menu has to appear or not
    private bool SingleMode = false; //Are we playing in singleplayer mode

    //Input systems for player one (so they can take control of the results screen)
    private PlayerInput InputOne;
    private MultiplayerEventSystem POneUISys;

    public int GMLevelIndex; //The level's index in GM

    public Text timertext; //The on-screen timer text

    //UI elements for result screen
    public Button ReturnButton;
    public GameObject ResultsCanvas;
    public Text ResultDetails;

    //UI elements for scroll collectable
    public Sprite NotCollectedSprite;
    public Sprite CollectedSprite;
    public Image UIIcon;
    public int CollectableStatus;

    //Fun facts for multiplayer games
    private int MaxHits = 0;
    private int ThePlayerWithMaxHits = 99;

    private PlayerInput[] inputs;

    public List<GhostData> RecordingGhostData;
    private float IntervalTime = 0;
    private float AccumalativeTime;

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

        //Get all player inputs and see whether they're player one
        inputs = FindObjectsOfType<PlayerInput>();
        foreach (var item in inputs)
        {
            if (item.playerIndex == 0)
            {
                InputOne = item;
                POneUISys = item.gameObject.GetComponent<MultiplayerEventSystem>();
            }
        }
    }

    //After scene loads, get things loaded
    public void BeginGame()
    {
        inputs = FindObjectsOfType<PlayerInput>();

        //If we aren't in tutorial mode, allow ball to move
        if (!GameManager.GM.TutorialMode)
        {
            foreach (var item in inputs)
            {
                item.SwitchCurrentActionMap("In-Game Ball");
            }
        }

        gameStat.StartTimer(); //And start the timer
    }

    public void StartTimer()
    {
        StartCoroutine(IncrementTimer());
    }

    //Timer will continue until the game is over (exits the loop) or a pause is called (stops counter from incrementing)
    IEnumerator IncrementTimer()
    {
        while (gameStat.GameOver == false)
        {
            if (ForcePause == false)
            {
                gameStat.Timer += Time.deltaTime;
                timertext.text = "Timer: " + gameStat.Timer.ToString("F2");
            }
            yield return null;
        }
    }

    //Receive hit info from ball and add it to the list
    public void AddGhostData(float Power, float Angle)
    {
        IntervalTime = Timer - AccumalativeTime;
        AccumalativeTime = Timer;
        RecordingGhostData.Add(new GhostData{HitAngle = Angle, HitPower = Power, Timing = IntervalTime});
    }

    //When flag is hit, pass parametres into list
    public void SubmitRecord(int pIndex, int NumHits, MoveBall ball)
    {
        gameStat.playerStatuses[pIndex].Completed = true;
        gameStat.playerStatuses[pIndex].NumHits = NumHits;
        gameStat.playerStatuses[pIndex].Time = gameStat.Timer;

        ball.UpdateTimerText(gameStat.playerStatuses[pIndex].Time);

        if (NumHits > MaxHits)
        {
            MaxHits = NumHits;
            ThePlayerWithMaxHits = pIndex;
        }

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

    //Ends the game when all players have finished
    public void EndGame()
    {
        Debug.Log("All players finished");
        if (SingleMode) //Singleplayer
        {
            var ThisTime = gameStat.playerStatuses[0].Time;
            var ThisHits = gameStat.playerStatuses[0].NumHits;

            var GMLevel = GameManager.GM.LevelData[GMLevelIndex];

            ResultDetails.text = "Great job in completing this course in " + ThisHits + " hit/s and " + ThisTime.ToString("F2") + " seconds!\n\n";
            
            //Handles records and text for hits
            if (ThisTime < GMLevel.BestTime) //Beat old time record
            {
                ResultDetails.text = ResultDetails.text + "You managed to beat your old time of " + GMLevel.BestTime.ToString("F2") + " seconds!\n\n";

                GMLevel.BestTime = ThisTime;
                
                Debug.Log("New Time Record");

            } else if (GMLevel.BestTime == 0) //First record
            {
                ResultDetails.text = ResultDetails.text + "This is your first time record in this course! ";

                GMLevel.BestTime = ThisTime;

                Debug.Log("Fresh Time Record");
            } else if (ThisTime == GMLevel.BestTime) //Tied record
            {
                ResultDetails.text = ResultDetails.text + "Seems like you tied your old best time! Think you can get those last time saves?\n\n";
            }
            else //Doesn't beat record
            {
                ResultDetails.text = ResultDetails.text + "Unfortunately you missed your record of " + GMLevel.BestTime.ToString("F2") + " seconds. Keep shaving those time saves!\n\n";
            }

            //Handles records and text for hits
            if (ThisHits < GMLevel.BestHits) //Beat old hit record
            {
                ResultDetails.text = ResultDetails.text + "You managed to beat your old hit record of " + GMLevel.BestHits + "!\n\n";

                GMLevel.BestHits = ThisHits;

                Debug.Log("New Hits Record");

            } else if (GMLevel.BestHits == 0) //Completely new hit record
            {
                ResultDetails.text = ResultDetails.text + "It's also your first hit record too! Think you can go back and beat these?";

                GMLevel.BestHits = ThisHits;

                Debug.Log("Fresh Hits Record");

            }  else if (GMLevel.BestHits == ThisHits) //Tied old hit record
            {
                if (ThisHits == 1) //Tied old record *and* reached the min number of hits
                {
                    ResultDetails.text = ResultDetails.text + "Seems like you've peaked when it comes to taking these shots! Perhaps work on those time records now.\n\n";
                } else
                {
                    ResultDetails.text = ResultDetails.text + "Looks like you've managed to tie your old hit record! Think you could go back and tightening those bounces?\n\n";
                }
            } else
            {
                ResultDetails.text = ResultDetails.text + "Unfortunately you missed your record of " + GMLevel.BestHits + " hit/s. Try working on tigher bounces!\n\n";
            }
            GameManager.GM.TimesPlayedSolo += 1;

        } else //Multiplayer
        {
            ResultDetails.text = "Great job! All players have cleared the course in less than " + gameStat.Timer.ToString("F2") + " seconds! By the way Player " + (ThePlayerWithMaxHits + 1) + ", well done with getting it in " + MaxHits + ". Keep working on it!" + "\n\nPlayer 1, feel free to choose whether to Restart Course, Return to Course Select, or just Quit to Main Menu.";
            GameManager.GM.TimesPlayedMulti += 1;
        }

        if (InputOne.gameObject.TryGetComponent(out DragAndAimControllerManager manager)) //Changes input mode for Mouse
        {
            manager.SetToUI();
        }
        InputOne.SwitchCurrentActionMap("UI");

        ResultsCanvas.SetActive(true); //Show results screen

        //Hijacks player 1 input
        POneUISys.playerRoot = ResultsCanvas;
        if (InputOne.currentControlScheme != "Mouse")
        {
            POneUISys.SetSelectedGameObject(ReturnButton.gameObject);
        }
        POneUISys.firstSelectedGameObject = ReturnButton.gameObject;

        //Checks unlockables then saves the game
        if (CollectableStatus == 1)
        {
            GameManager.GM.LevelData[GMLevelIndex].CollectableGet = 2;
        }
        
        GameManager.GM.CheckUnlockables();
        GameManager.GM.SavePlayer();
    }

    //Butten when restaring the course
    public void RestartScene()
    {
        LoadingScreen.loadMan.BeginLoadingScene(SceneManager.GetActiveScene().name, true);
    }

    //Button for returning to main menu
    public void ReturnToMain()
    {
        GameManager.GM.SingleMode = false;
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
