using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Holds data for the courses
[System.Serializable]
public class LevelFormat
{
    public string LevelName = "dummy"; //The name saved in Unity
    public string ExternalName = ""; //The name that should show up everywhere else
    public int LevelInt = -99; //The order it's supposed to show in
    public float BestTime = 0; //Time record
    public int BestHits = 0; //Hit record
    public int CollectableGet = 0; //Whether the user has collected the course's collectable. 0 = No, 1 = Just got, 2 = Already get
    public List<GhostData> ghostData;
}
