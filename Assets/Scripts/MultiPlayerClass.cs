using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Another simple class to handle multiple players for GameManager
[System.Serializable]
public class MultiPlayerClass
{
    public int ControlType;
    public int PlayerIndex;
    public float AimingSensitivity = 4;
    public InputDevice inputDevice;
}
