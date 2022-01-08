using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    GameManager gameMan;
    LoadingScreen loading;
    List<LevelFormat> LevelList;

    private void Start()
    {
        loading = LoadingScreen.loadMan;
        gameMan = GameManager.GM;

        LevelList = gameMan.LevelData;
    }

    public void LoadLevel(int LevelInt)
    {
        loading.BeginLoadingScene(LevelList[LevelInt].LevelName, true);
    }
}
