using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;

    public bool SingleMode = false;
    public List<MultiPlayerClass> NumPlayers;

    //Guide for Multiplayer Players
    //Number of records = Number of players
    //ControlType = The controller they are using; 0 = Mouse, 1 = Buttons
    //PlayerIndex = The position the player has connected; 99 = Player disconnected so please ignore
    //inputDevice = The device the player used to connect

    //Upon first load, make GM the only GameManager possible
    void Awake()
    {
        if (GM != null && GM != this)
        {
            Destroy(this.gameObject);
        } else
        {
            GM = this;
            GM.NumPlayers.Add(new MultiPlayerClass { ControlType = PlayerPrefs.GetInt("InputType", 0) });
            GM.NumPlayers[0].PlayerIndex = 0;
        }
        DontDestroyOnLoad(this);
    }
}
