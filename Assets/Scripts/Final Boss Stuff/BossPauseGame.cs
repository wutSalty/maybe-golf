using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class BossPauseGame : MonoBehaviour
{
    public static BossPauseGame bossPause;

    public bool MenuIsOpen = false;

    private PlayerInput[] playerInputs;

    [System.Serializable]
    public class PausedAudio
    {
        public AudioSource source;
        public bool IsPaused;
    }

    public List<PausedAudio> paused;

    void Awake()
    {
        if (bossPause != null && bossPause != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            bossPause = this;
        }
    }

    private void Start()
    {
        playerInputs = FindObjectsOfType<PlayerInput>();

        foreach (var item in AudioManager.instance.sounds)
        {
            if (item.audioPurpose == 2)
            {
                paused.Add(new PausedAudio { source = item.source, IsPaused = false });
            }
        }
    }

    public void SetPause(int playerIndex)
    {
        foreach (var item in playerInputs)
        {
            if (MenuIsOpen == false)
            {
                if (item.playerIndex == playerIndex)
                {
                    item.SwitchCurrentActionMap("Menu");
                }
                else
                {
                    item.SwitchCurrentActionMap("Not Caller Menu");
                    item.GetComponent<MultiplayerEventSystem>().enabled = false;
                }
            }
            else
            {
                item.SwitchCurrentActionMap("In-Game");
                item.GetComponent<MultiplayerEventSystem>().enabled = true;
            }
        }

        if (MenuIsOpen == false)
        {
            //Time.timeScale = 0;
            MenuIsOpen = true;
            foreach (var item in paused)
            {
                if (item.source.isPlaying)
                {
                    item.source.Pause();
                    item.IsPaused = true;
                }
            }
        }
        else
        {
            //Time.timeScale = 1;
            MenuIsOpen = false;
            foreach (var item in paused)
            {
                if (item.IsPaused)
                {
                    item.source.UnPause();
                    item.IsPaused = false;
                }
            }
        }
    }
}
