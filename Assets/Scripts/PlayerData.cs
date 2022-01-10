using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the "middle step" for saving save data. Gets all the data required from GameManager then copies it to itself
[System.Serializable]
public class PlayerData 
{
    public List<LevelFormat> LevelData;
    public bool[] UnlockedBallSkins;
    public int TimesPlayed;

    public PlayerData(GameManager player)
    {
        LevelData = player.LevelData;
        TimesPlayed = player.TimesPlayed;
        UnlockedBallSkins = player.UnlockedBallSkins;
    }
}
