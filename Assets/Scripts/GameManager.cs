using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;

    public List<MultiPlayerClass> NumPlayers;

    //Guide for Multiplayer Players
    //Number of records = Number of players
    //Int in each record = Controller type

    void Awake()
    {
        if (GM != null && GM != this)
            Destroy(this.gameObject);
        else
            GM = this;

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        GM.NumPlayers.Add(new MultiPlayerClass { ControlType = PlayerPrefs.GetInt("InputType", 0)});
    }
}
