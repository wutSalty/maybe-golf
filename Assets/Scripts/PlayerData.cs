using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the "middle step" for saving save data. Gets all the data required from GameManager then copies it to itself
[System.Serializable]
public class PlayerData 
{
    public List<LevelFormat> LevelData;
    public bool BossLevelUnlocked;
    public bool[] UnlockedBallSkins;
    public int TimesPlayedSolo;
    public int TimesPlayedMulti;
    public int BallSkin;

    public PlayerData(GameManager player)
    {
        LevelData = player.LevelData;
        BossLevelUnlocked = player.BossLevelUnlocked;
        TimesPlayedSolo = player.TimesPlayedSolo;
        TimesPlayedMulti = player.TimesPlayedMulti;
        UnlockedBallSkins = player.UnlockedBallSkins;
        BallSkin = player.BallSkin;
    }
}
