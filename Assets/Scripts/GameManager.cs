using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;

    public Animator NotiAnimator;

    public bool SingleMode = false;
    public List<MultiPlayerClass> NumPlayers;
    public Sprite[] BallSkins;
    //Guide for Multiplayer Players (NumPlayers)

    //ControlType = The controller they are using; 0 = Mouse, 1 = Buttons
    //PlayerIndex = The position the player has connected; 99 = Player disconnected so please ignore
    //inputDevice = The device the player used to connect. Required for pairing when switching scenes

    public List<LevelFormat> LevelData;
    public bool[] UnlockedBallSkins;
    public int TimesPlayed;

    public string Version = "Pre-Alpha v0.0.0";
    //LevelData required for saving level data

    public List<int> LockedBalls;

    public bool LoadIntoLevelSelect = false;

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

    bool WantsToQuit()
    {
        Debug.Log("Game quitting... see you next time");
        return true;
    }

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

    //Check unlockables
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
