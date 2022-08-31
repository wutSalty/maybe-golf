using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunnyMinimapHandling : MonoBehaviour
{
    public FunnyGameManager gameMan;
    private FunnyPlayerPause playerPause;
    private PlayerUpgradesScript playerUpgrades;

    public Animator mapAnim;
    public bool MiniMapOpen { get; private set; } = false;

    private void Start()
    {
        playerPause = GetComponent<FunnyPlayerPause>();
        playerUpgrades = GetComponent<PlayerUpgradesScript>();
    }

    private void OnMapping()
    {
        if (playerPause.gamePaused || !gameMan.GameIsActive || playerUpgrades.shopOpened)
        {
            return;
        }

        CheckMapStatus();
    }

    private void CheckMapStatus()
    {
        if (MiniMapOpen)
        {
            mapAnim.Play("Map_Close");
        }
        else
        {
            mapAnim.Play("Map_Open");
        }

        MiniMapOpen = !MiniMapOpen;
    }
}
