using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;

//Handles save data, any data that needs to travel through scenes, other things that are handy to have just one copy of, etc etc
public class GameManager : MonoBehaviour
{
    public static GameManager GM;

    [Header("Animation")]
    //Animation for notification bars
    public Animator NotiAnimator; 
    public Animator BossUnlockNoti;
    public Animator fcNoti;
    public Animator SavedNoti;

    [Header("Current Mode")]
    public bool SingleMode = false; //Flag for playing in singleplayer
    public bool GhostMode = false; //Whether playing vs ghosts
    public bool LoadIntoLevelSelect = false; //Flag for whether user needs to go straight into Level Select
    public bool TutorialMode = false; //Flag for whether the game is currently playing the tutorial

    [Header("Current Players")]
    public List<MultiPlayerClass> NumPlayers; //Holds data for every player playing or connected

    [Header("Level Info")]
    [Tooltip("This is Saved")] public List<LevelFormat> LevelData; //Holds data about every level (This data is saved)
    [Tooltip("This is Saved")] public bool BossLevelUnlocked; //Flag for whether the user has unlocked the Boss Level yet (Also saved)

    [Header("Fun Stats")]
    [Tooltip("This is Saved")] public int TimesPlayedSolo; //Number of times the user has cleared a course by themselves
    [Tooltip("This is Saved")] public int TimesPlayedMulti; //Number of times user has cleared a course in multiplayer
    [Tooltip("This is Saved")] public int BallSkin = 0; //Migrate ball skins from playerPrefs to internal cause cheaters

    [Header("Internal Info")]
    public string appID = "FE08EA34-BBE9-4400-A36F-DEB5009E7986";
    public string appBuild = "22w31a";

    [Header("Ball Data")]
    //Holds the sprites and status of unlockable balls
    public Sprite[] BallSkins;
    [Tooltip("This is Saved")] public bool[] UnlockedBallSkins;
    public List<int> LockedBalls;

    [Header("Save Completion")]
    [Tooltip("This is Saved")] public bool FullCleared = false; //Has the user completed everything in the game
    [Tooltip("This is Saved")] public float SparkleColour = 0; //The Hue value for the colour of sparkles

    [Header("Save Status")]
    public int ErrorStatus; //Has data loaded with an error?
    public bool FirstLoaded = true; //Is this the first time the game's at title screen?

    [Header("Random")]
    public string DownloadedText = "";

    public Material mat;

    public bool SilentSave = false; //Flag to save game without audio/anim cue

    public AppdataClass jsonHere;

    //Upon first load, make GM the only GameManager possible
    void Awake()
    {
        if (GM != null && GM != this)
        {
            Destroy(this.gameObject);
        } else
        {
            GM = this;

            LoadPlayer();
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        Application.wantsToQuit += WantsToQuit;
        CheckLocked();
        CheckTempFolder();
    }

    private void Update()
    {
        mat.SetFloat("_unscaledTime", Time.unscaledTime);
    }

    //Just makes a message when the application is quitting
    bool WantsToQuit()
    {
        Debug.Log("Game quitting... see you next time");
        return true;
    }

    //Checks the status of balls still locked
    public void CheckLocked()
    {
        GM.LockedBalls.Clear();

        int index = 0;
        foreach (var item in GM.UnlockedBallSkins)
        {
            if (item == false)
            {
                GM.LockedBalls.Add(index);
            }
            index += 1;
        }
    }

    public void CheckTempFolder()
    {
        UpdateManager updateManager = FindObjectOfType<UpdateManager>();
        string appName = "\\" + Application.productName;
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + appName + updateManager.saveTo;

        print(path);

        DirectoryInfo di = new DirectoryInfo(path);
        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }
        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete();
        }
    }

    //After every course, check whether it should unlock something
    [ContextMenu("Force Check")]
    public void CheckUnlockables()
    {
        int LockedIndex;
        int UnlockIndex;

        //Checks whether a new skin should be unlocked
        if ((GM.LockedBalls.Count > 0) && ((TimesPlayedSolo + TimesPlayedMulti) % 5 == 0))
        {
            LockedIndex = Random.Range(0, GM.LockedBalls.Count);
            UnlockIndex = GM.LockedBalls[LockedIndex];

            GM.UnlockedBallSkins[UnlockIndex] = true;
            
            CheckLocked();

            NotiAnimator.SetTrigger("ShowNoti");
            AudioManager.instance.PlaySound("UI_noti");
        } else
        {
            Debug.Log("No collectables yet");
        }

        //Checks whether all other levels have been played through yet before unlocking boss level
        if (!BossLevelUnlocked)
        {
            BossLevelUnlocked = true;
            foreach (var item in LevelData)
            {
                if (item.BestTime == 0 && item.LevelInt != 5)
                {
                    BossLevelUnlocked = false;
                }
            }

            if (BossLevelUnlocked)
            {
                BossUnlockNoti.SetTrigger("ShowNoti");
                AudioManager.instance.PlaySound("UI_noti");
            }
        }

        //Checks whether all levels have been cleared and all scrolls have been collected
        if (!FullCleared)
        {
            FullCleared = true;
            foreach (var item in LevelData)
            {
                if (item.BestTime == 0)
                {
                    FullCleared = false;
                }

                if (item.CollectableGet != 2 && item.LevelInt != 5)
                {
                    FullCleared = false;
                }
            }

            if (FullCleared)
            {
                fcNoti.SetTrigger("Noti");
                AudioManager.instance.PlaySound("UI_noti");
            }
        }
    }

    //Save manager stuffs
    [ContextMenu("Force Save")]
    public void SavePlayer()
    {
        //Package savedata from GM into PlayerData data
        PlayerData data = new PlayerData(this);

        //Send data to SaveSystem
        SaveSystem.SaveGame(data);

        if (!SilentSave)
        {
            SavedNoti.SetTrigger("GameSaved");
        }
        SilentSave = false;

        string LastSaved = "Game saved at: " + System.DateTime.Now;
        Debug.Log(LastSaved);
    }

    [ContextMenu("Force Load")]
    public void LoadPlayer()
    {
        //Get savedata from SaveSystem
        PlayerData data = SaveSystem.LoadGame();

        if (data == null)
        {
            switch (SaveSystem.LoadStatus)
            {
                case 1: //Hash Error
                    ErrorStatus = 1;
                    break;

                case 2: //File not exists
                    SilentSave = true;
                    SavePlayer(); //Assume fresh save file
                    break;

                default: //Unknown error
                    ErrorStatus = 99;
                    break;
            }
            
        } else
        {
            ErrorStatus = 0;

            //Insert back into GM
            BossLevelUnlocked = data.BossLevelUnlocked;
            TimesPlayedSolo = data.TimesPlayedSolo;
            TimesPlayedMulti = data.TimesPlayedMulti;
            BallSkin = data.BallSkin;
            FullCleared = data.FullCleared;
            SparkleColour = data.SparkleColour;

            for (int i = 0; i < data.LevelData.Count; i++)
            {
                LevelData[i] = data.LevelData[i];
            }

            for (int i = 0; i < data.UnlockedBallSkins.Length; i++)
            {
                UnlockedBallSkins[i] = data.UnlockedBallSkins[i];
            }

            Debug.Log("Game loaded at: " + System.DateTime.Now);
        }
    }

    //Deletes save file. Only occurs through editor
    [ContextMenu("Force Delete Save")]
    void ForceDeleteSave()
    {
        Debug.Log("File itself deleted");

        string path = Application.persistentDataPath + "/playerData.golf";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
