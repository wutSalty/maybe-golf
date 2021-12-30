using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStatus : MonoBehaviour
{
    public static GameStatus gameStat;

    [System.Serializable]
    public class PlayerStatus
    {
        public bool Completed = false;
        public float Time;
        public int NumHits;
        public int playerIndex;
    }

    public List<PlayerStatus> playerStatuses;
    private float Timer;
    private bool GameOver;

    public Text timertext;

    void Awake()
    {
        if (gameStat != null && gameStat != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            gameStat = this;

            gameStat.playerStatuses.Add(new PlayerStatus { playerIndex = 99 });
            gameStat.playerStatuses.Add(new PlayerStatus { playerIndex = 99 });
            gameStat.playerStatuses.Add(new PlayerStatus { playerIndex = 99 });
            gameStat.playerStatuses.Add(new PlayerStatus { playerIndex = 99 });
        }
    }

    private void Start()
    {
        foreach (var item in GameManager.GM.NumPlayers)
        {
            if (item.PlayerIndex != 99)
            {
                gameStat.playerStatuses[item.PlayerIndex].playerIndex = item.PlayerIndex;
            }
        }
    }

    private void Update()
    {
        if (gameStat.GameOver == false)
        {
            gameStat.Timer += Time.deltaTime;
            timertext.text = "Timer: " + gameStat.Timer.ToString("F2");
        }
    }

    public void SubmitRecord(int pIndex, int NumHits)
    {
        gameStat.playerStatuses[pIndex].Completed = true;
        gameStat.playerStatuses[pIndex].NumHits = NumHits;
        gameStat.playerStatuses[pIndex].Time = gameStat.Timer;

        gameStat.CheckStatus();
    }

    public void CheckStatus()
    {
        gameStat.GameOver = true;
        foreach (var item in gameStat.playerStatuses)
        {
            if (item.playerIndex != 99 && item.Completed == false)
            {
                gameStat.GameOver = false;
            }
        }
        if (gameStat.GameOver)
        {
            Debug.Log("All players finished");
        }
    }
}
