using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    bool WantsToQuit()
    {
        Debug.Log("Game quitting... see you next time");
        return true;
    }

    //Check unlockables
    [ContextMenu("Force Check")]
    public void CheckUnlockables()
    {
        int LockedIndex;
        int UnlockIndex;

        if (GM.LockedBalls.Count > 0)
        {
            switch (TimesPlayed)
            {
                case 5:
                    LockedIndex = Random.Range(0, GM.LockedBalls.Count);
                    UnlockIndex = GM.LockedBalls[LockedIndex];

                    GM.UnlockedBallSkins[UnlockIndex] = true;
                    GM.LockedBalls.RemoveAt(LockedIndex);
                    break;

                case 10:
                    LockedIndex = Random.Range(0, GM.LockedBalls.Count);
                    UnlockIndex = GM.LockedBalls[LockedIndex];

                    GM.UnlockedBallSkins[UnlockIndex] = true;
                    GM.LockedBalls.RemoveAt(LockedIndex);
                    break;

                case 15:
                    LockedIndex = Random.Range(0, GM.LockedBalls.Count);
                    UnlockIndex = GM.LockedBalls[LockedIndex];

                    GM.UnlockedBallSkins[UnlockIndex] = true;
                    GM.LockedBalls.RemoveAt(LockedIndex);
                    break;

                case 20:
                    LockedIndex = Random.Range(0, GM.LockedBalls.Count);
                    UnlockIndex = GM.LockedBalls[LockedIndex];

                    GM.UnlockedBallSkins[UnlockIndex] = true;
                    GM.LockedBalls.RemoveAt(LockedIndex);
                    break;

                default:
                    break;
            }
        } else
        {
            Debug.Log("We've run out of collectables, come back next time");
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

        //Insert back into GM
        LevelData = data.LevelData;
        TimesPlayed = data.TimesPlayed;
        UnlockedBallSkins = data.UnlockedBallSkins;

        Debug.Log("Game loaded at: " + System.DateTime.Now);
    }
}
