using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData 
{
    public List<LevelFormat> LevelData;
    public string Version;

    public PlayerData(GameManager player)
    {
        LevelData = player.LevelData;
        Version = player.Version;
    }
}
