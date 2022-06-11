using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to hold data about player's ghost movements
[System.Serializable]
public class GhostData
{
    public float Timing; //Time between last shot
    public float HitPower; //Power of the shot
    public float HitAngle; //Angle of the shot
    public bool ResetPos; //Did the user reset their position
}
