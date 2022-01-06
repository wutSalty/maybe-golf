using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
