using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;

    public bool TwoPlayerMode = false;

    void Awake()
    {
        if (GM != null)
            Destroy(GM);
        else
            GM = this;

        DontDestroyOnLoad(this);
    }
}
