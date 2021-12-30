using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;

    public bool SingleMode = false;

    public List<MultiPlayerClass> NumPlayers;
    //Guide for Multiplayer Players (NumPlayers)

    //ControlType = The controller they are using; 0 = Mouse, 1 = Buttons
    //PlayerIndex = The position the player has connected; 99 = Player disconnected so please ignore
    //inputDevice = The device the player used to connect. Required for pairing when switching scenes

    public List<LevelFormat> LevelData;
    //LevelData required for saving level data

    //Upon first load, make GM the only GameManager possible
    void Awake()
    {
        if (GM != null && GM != this)
        {
            Destroy(this.gameObject);
        } else
        {
            GM = this;
        }
        DontDestroyOnLoad(this);
    }

    public void SavePlayer()
    {
        PlayerData data = new PlayerData(this);
        SaveSystem.SaveGame(data);

        Debug.Log("Game saved at: " + System.DateTime.Now);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadGame();
        LevelData = data.LevelData;

        Debug.Log("Game loaded at: " + System.DateTime.Now);
    }
}
