using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Basic class for dialogue information. Could be expanded on but this works for now
[System.Serializable]
public class Dialogue
{
    public string name;

    [TextArea(3, 10)]
    public string[] sentences;
}
