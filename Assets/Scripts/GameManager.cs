using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

//Handles save data, any data that needs to travel through scenes, other things that are handy to have just one copy of, etc etc
public class GameManager : MonoBehaviour
{
    public static GameManager GM;

    public Animator NotiAnimator; //Animation for notification bar

    public bool SingleMode = false; //Flag for playing in singleplayer
    public bool LoadIntoLevelSelect = false; //Flag for whether user needs to go straight into Level Select
    public bool TutorialMode = false; //Flag for whether the game is currently playing the tutorial

    public List<MultiPlayerClass> NumPlayers; //Holds data for every player playing or connected

    public List<LevelFormat> LevelData; //Holds data about every level (This data is saved)
    
    public int TimesPlayed; //Number of times the user has cleared a course (This data is saved)

    public string Version = "Pre-Alpha v0.0.0";

    //Holds the sprites and status of unlockable balls
    public Sprite[] BallSkins;
    public bool[] UnlockedBallSkins;
    public List<int> LockedBalls;

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

    //After every course, check whether it should unlock a skin
    [ContextMenu("Force Check")]
    public void CheckUnlockables()
    {
        int LockedIndex;
        int UnlockIndex;

        if ((GM.LockedBalls.Count > 0) && (TimesPlayed % 5 == 0))
        {
            LockedIndex = Random.Range(0, GM.LockedBalls.Count);
            UnlockIndex = GM.LockedBalls[LockedIndex];

            GM.UnlockedBallSkins[UnlockIndex] = true;
            
            CheckLocked();

            NotiAnimator.SetTrigger("ShowNoti");
        } else
        {
            Debug.Log("No collectables yet");
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

        Debug.Log("Game saved at: " + System.DateTime.Now);
    }

    [ContextMenu("Force Load")]
    public void LoadPlayer()
    {
        //Get savedata from SaveSystem
        PlayerData data = SaveSystem.LoadGame();

        if (data == null)
        {
            Debug.Log("Instead of not having data, how about I create some for you.");
            SavePlayer();
            Debug.Log("No need to thank me.");
        } else
        {
            //Insert back into GM
            LevelData = data.LevelData;
            TimesPlayed = data.TimesPlayed;
            UnlockedBallSkins = data.UnlockedBallSkins;

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
