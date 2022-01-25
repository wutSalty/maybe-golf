using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class BossControllerDisconnect : MonoBehaviour
{
    public static BossControllerDisconnect BossControlDC;

    //UI elements
    public Canvas DisconnectPanel;
    public Text ControllerTemplate;
    public GameObject ReturnBtn;

    //Flag for other components to check whether game is playing or not
    public bool CurrentlyDC = false;

    [System.Serializable]
    public class CurrentDC
    {
        public int PlayerIndex;
        public string ControlName = "dummy";
    };

    public List<CurrentDC> CurrentDead; //List of all disconnected controllers

    private PlayerInput[] listofInputs; //List of all players currently connected

    //Make this the only instance possible
    void Awake()
    {
        if (BossControlDC != null && BossControlDC != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            BossControlDC = this;

            BossControlDC.CurrentDead.Add(new CurrentDC { PlayerIndex = 99 });
            BossControlDC.CurrentDead.Add(new CurrentDC { PlayerIndex = 99 });
            BossControlDC.CurrentDead.Add(new CurrentDC { PlayerIndex = 99 });
            BossControlDC.CurrentDead.Add(new CurrentDC { PlayerIndex = 99 });
        }
    }

    private void Start()
    {
        listofInputs = FindObjectsOfType<PlayerInput>(); //Get all the PlayerInputs in play
    }

    //When a controller has disconnected, change the Action Map and display the Warning Screen 
    public void EmergencyPause()
    {
        foreach (PlayerInput item in listofInputs) //For each of them
        {
            //Change the Action Map to the required one
            if (BossControlDC.CurrentlyDC == false)
            {
                item.SwitchCurrentActionMap("Menu");
                item.gameObject.GetComponent<MultiplayerEventSystem>().playerRoot = DisconnectPanel.gameObject;
                item.gameObject.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(null);
                item.gameObject.GetComponent<MultiplayerEventSystem>().firstSelectedGameObject = ReturnBtn;
            }
            else
            {
                item.SwitchCurrentActionMap("In-Game");
                item.gameObject.GetComponent<MultiplayerEventSystem>().playerRoot = item.gameObject;
                item.gameObject.GetComponent<MultiplayerEventSystem>().firstSelectedGameObject = null;
            }
        }

        if (BossControlDC.CurrentlyDC == false)
        {
            Time.timeScale = 0;
            BossControlDC.CurrentlyDC = true;
            DisconnectPanel.gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            BossControlDC.CurrentlyDC = false;
            DisconnectPanel.gameObject.SetActive(false);
        }
    }

    //When controller disconnected, make this call with it's playerindex
    public void ControllerDisconnected(int playerIndex)
    {
        BossControlDC.CurrentDead[playerIndex].PlayerIndex = playerIndex;
        if (BossControlDC.CurrentlyDC == false)
        {
            ControllerTemplate.text = "Player " + (playerIndex + 1);
            EmergencyPause();
        }
        else
        {
            string NewText = "";
            foreach (var item in BossControlDC.CurrentDead) //If stuff still exists, update the text
            {
                if (item.PlayerIndex != 99)
                {
                    NewText = NewText + "Player " + (item.PlayerIndex + 1) + ", ";
                }
            }

            NewText = NewText.Remove(NewText.Length - 2, 2); //Cuts off the extra comma

            ControllerTemplate.text = NewText;
        }
    }

    //When controller re-connected, hide screen
    public void ControllerConnected(int playerIndex)
    {
        BossControlDC.CurrentDead[playerIndex].PlayerIndex = 99; //When controller reconnects, set internal index to 99 (now connected)
        bool NothingLeft = true;

        foreach (var item in BossControlDC.CurrentDead) //Check if there's any controllers still not connected
        {
            if (item.PlayerIndex != 99)
            {
                NothingLeft = false; //If there's anything still not connected, set to false
            }
        }

        if (NothingLeft) //If there's nothing left, resume the game
        {
            EmergencyPause();
        }
        else
        {
            string NewText = "";
            foreach (var item in BossControlDC.CurrentDead) //If stuff still exists, update the text
            {
                if (item.PlayerIndex != 99)
                {
                    NewText = NewText + "Player " + (item.PlayerIndex + 1) + ", ";
                }
            }

            NewText = NewText.Remove(NewText.Length - 2, 2); //Cuts off the extra comma

            ControllerTemplate.text = NewText;
        }
    }

    public void ReturnToMain()
    {
        BossStatus.bossStat.ForcePause = true;
        GameManager.GM.NumPlayers.Clear();
        GameManager.GM.SingleMode = false;
        GameManager.GM.GhostMode = false;
        LoadingScreen.loadMan.BeginLoadingScene("MainMenu", false);
    }
}
