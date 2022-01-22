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
    public GameObject VsText;

    //UI elements for scroll collectable
    public Sprite NotCollectedSprite;
    public Sprite CollectedSprite;
    public Image UIIcon;
    public GameObject IconPanel;
    public int CollectableStatus;

    //Fun facts for multiplayer games
    private int MaxHits = 0;
    private int ThePlayerWithMaxHits = 99;

    private PlayerInput[] inputs;

    //Ghost data recordings
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

        if (!SingleMode)
        {
            UIIcon.gameObject.SetActive(false);
            IconPanel.SetActive(false);
        }

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

        GhostBallMove ghostBallMove = FindObjectOfType<GhostBallMove>();
        //If ghosts are enabled, start the ghost
        if (GameManager.GM.GhostMode && GameManager.GM.SingleMode && ghostBallMove != null)
        {
            ghostBallMove.StartReplay();
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
    public void AddGhostData(float Power, float Angle, bool Restart)
    {
        IntervalTime = Timer - AccumalativeTime;
        AccumalativeTime = Timer;
        RecordingGhostData.Add(new GhostData{HitAngle = Angle, HitPower = Power, Timing = IntervalTime, ResetPos = Restart});
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
            var OldTime = GMLevel.BestTime;
            var OldHits = GMLevel.BestHits;

            ResultDetails.text = "Great job in completing this course in " + ThisHits + " hit/s and " + ThisTime.ToString("F2") + " seconds!\n\n";
            
            //Handles records and text for hits
            if (OldTime == 0 && OldHits == 0) //First play
            {
                ResultDetails.text = ResultDetails.text + "Congrats on your first time and hit record in this course! Think you can go back and break new records?\n\nGhost Data has been created for this course.";

                GMLevel.BestTime = ThisTime;
                GMLevel.BestHits = ThisHits;
                GMLevel.ghostData = RecordingGhostData;
            }
            else if (ThisTime < OldTime && ThisHits > OldHits) //Beat time, lost hits
            {
                ResultDetails.text = ResultDetails.text + "You managed to beat your old time of " + OldTime.ToString("F2") + " seconds!\n\nUnfortunately you missed your current hit record of " + OldHits + ". Try taking some more challenging bounces.\n\nGhost Data has been updated for this course.";

                GMLevel.BestTime = ThisTime;
                GMLevel.ghostData = RecordingGhostData;
            }
            else if (ThisTime < OldTime && ThisHits == OldHits) //Beat time, tied hits
            {
                if (OldHits == 1) //Tied hits and record is 1
                {
                    ResultDetails.text = ResultDetails.text + "You managed to beat your old time of " + OldTime.ToString("F2") + " seconds!\n\nAnd it seems like you've peaked when it comes to taking those shots! Maybe keep grinding for even lower times!\n\nGhost Data has been updated for this course.";

                    GMLevel.BestTime = ThisTime;
                    GMLevel.ghostData = RecordingGhostData;
                }
                else //Tied but not 1
                {
                    ResultDetails.text = ResultDetails.text + "You managed to beat your old time of " + OldTime.ToString("F2") + " seconds!\n\nSeems like you also tied your current hit record of " + OldHits + ". Think you can tighten up some of those bounces?\n\nGhost Data has been updated for this course.";

                    GMLevel.BestTime = ThisTime;
                    GMLevel.ghostData = RecordingGhostData;
                }   
            }
            else if (ThisTime < OldTime && ThisHits < OldHits) //Beat both
            {
                ResultDetails.text = ResultDetails.text + "You managed to beat your old time record of " + OldTime.ToString("F2") + " seconds!\n\nYou also managed to beat your old hit record of " + OldHits + " too! Think you could go again to make more crucial plays?\n\nGhost Data has been updated for this course.";

                GMLevel.BestTime = ThisTime;
                GMLevel.BestHits = ThisHits;
                GMLevel.ghostData = RecordingGhostData;
            }
            else if (ThisTime == OldTime && ThisHits > OldHits) //Tie and lose
            {
                ResultDetails.text = ResultDetails.text + "Somehow you've managed to tie your old record of " +OldTime.ToString("F2")+" seconds exactly to the dot! \n\nThough you missed your old hit record of " + OldHits + ". \n\nHow about finding some more challenging angles to take?";
            }
            else if (ThisTime == OldTime && ThisHits == OldHits) //Tie and tie
            {
                if (OldHits == 1) //if record is 1
                {
                    ResultDetails.text = ResultDetails.text + "Somehow you've managed to tie your current record of " + OldTime.ToString("F2") + " seconds exactly to the dot!\n\nYou've seemed to have peaked when it comes to taking those shots too! Why not try working on those time records now.";
                }
                else //Tied but not 1
                {
                    ResultDetails.text = ResultDetails.text + "Somehow you've managed to tie your current record of " + OldTime.ToString("F2") + " seconds exactly to the dot! \n\nAnd you tied your current hit record of " + OldHits + " too! How about about going again to beat either of them?\n\nGhost Data has been updated for this course.";

                    GMLevel.ghostData = RecordingGhostData;
                }                
            }
            else if (ThisTime == OldTime && ThisHits < OldHits) //Tie and beat
            {
                ResultDetails.text = ResultDetails.text + "Somehow you've managed to tie your current record of " + OldTime.ToString("F2") + " seconds exactly to the dot!\n\nAnd even better, you beat your old hit record of " + OldHits + " too! How about going again to shave some time to match those bounces.\n\nGhost Data has been updated for this course.";

                GMLevel.BestHits = ThisHits;
                GMLevel.ghostData = RecordingGhostData;
            }
            else if (ThisTime > OldTime && ThisHits > OldHits) //lose and lose
            {
                ResultDetails.text = ResultDetails.text + "It seems you've missed your old time record of " + OldTime.ToString("F2") + " seconds and missed your old hit record of " + OldHits + ". Why not take another shot at cracking some records!";
            }
            else if (ThisTime > OldTime && ThisHits == OldHits) //lose and tied
            {
                if (OldHits == 1) //if 1
                {
                    ResultDetails.text = ResultDetails.text + "It seems you've missed your old time record of " + OldTime.ToString("F2") + " seconds.\n\nBut you've managed to peak when it comes to taking those shots! All that's left to do is keep working on those times!";
                }
                else //Not one
                {
                    ResultDetails.text = ResultDetails.text + "It seems you've missed your old time record of " + OldTime.ToString("F2") + " seconds.\n\nBut you managed to tie your current hit record of " + OldHits + "! How about taking another go at those records.";
                }
            }
            else if (ThisTime > OldTime && ThisHits < OldHits) //lose and win
            {
                ResultDetails.text = ResultDetails.text + "It seems you've missed your old time record of " + OldTime.ToString("F2") + " seconds.\n\nBut you managed to beat your old hit record of " + OldHits + "! Now try shaving down those seconds!";
            }

            GameManager.GM.TimesPlayedSolo += 1;

        } else //Multiplayer
        {
            ResultDetails.text = "Great job! All players have cleared the course in less than " + gameStat.Timer.ToString("F2") + " seconds! By the way Player " + (ThePlayerWithMaxHits + 1) + ", well done with getting it in " + MaxHits + ". Keep working on it!" + "\n\nPlayer 1, feel free to choose whether to Restart Course, Return to Course Select, or just Quit to Main Menu.";
            GameManager.GM.TimesPlayedMulti += 1;
        }

        if (GameManager.GM.GhostMode)
        {
            VsText.SetActive(true);
        } else
        {
            VsText.SetActive(false);
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
