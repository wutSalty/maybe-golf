using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ControllerDisconnectPause : MonoBehaviour
{
    public static ControllerDisconnectPause ControlDC;

    public Canvas DisconnectPanel;
    public Text ControllerTemplate;

    public bool CurrentlyDC = false;

    [System.Serializable]
    public class CurrentDC
    {
        public int PlayerIndex;
        public string ControlName = "dummy";
    };

    public List<CurrentDC> CurrentDead;

    //Make this the only instance possible
    void Awake()
    {
        if (ControlDC != null && ControlDC != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            ControlDC = this;

            ControlDC.CurrentDead.Add(new CurrentDC { PlayerIndex = 99 });
            ControlDC.CurrentDead.Add(new CurrentDC { PlayerIndex = 99 });
            ControlDC.CurrentDead.Add(new CurrentDC { PlayerIndex = 99 });
            ControlDC.CurrentDead.Add(new CurrentDC { PlayerIndex = 99 });
        }

        //DontDestroyOnLoad(this);
    }

    //When a controller has disconnected, change the Action Map and display the Warning Screen 
    public void EmergencyPause()
    {
        PlayerInput[] listofInputs = FindObjectsOfType<PlayerInput>(); //Get all the PlayerInputs in play
        foreach (PlayerInput item in listofInputs) //For each of them
        {
            //Change the Action Map to the required one
            if (PauseGame.pM.MenuIsOpen == false)
            {
                item.SwitchCurrentActionMap("Pause Menu");
            }
            else
            {
                item.SwitchCurrentActionMap("In-Game Ball");
            }
        }

        if (PauseGame.pM.MenuIsOpen == false)
        {
            Time.timeScale = 0;
            PauseGame.pM.MenuIsOpen = true;
            DisconnectPanel.gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            PauseGame.pM.MenuIsOpen = false;
            DisconnectPanel.gameObject.SetActive(false);
        }
    }

    //When controller disconnected, make this call with it's playerindex
    public void ControllerDisconnected(int playerIndex)
    {
        ControlDC.CurrentDead[playerIndex].PlayerIndex = playerIndex;
        if (ControlDC.CurrentlyDC == false)
        {
            ControlDC.CurrentlyDC = true;
            ControllerTemplate.text = "Player " + (playerIndex + 1);
            EmergencyPause();
        } else
        {
            ControllerTemplate.text = ControllerTemplate.text + ", Player " + (playerIndex + 1);
        }
    }

    //When controller re-connected, hide screen
    public void ControllerConnected(int playerIndex)
    {
        ControlDC.CurrentDead[playerIndex].PlayerIndex = 99; //When controller reconnects, set internal index to 99 (now connected)
        bool NothingLeft = true;
        foreach (var item in ControlDC.CurrentDead) //Check if there's any controllers still not connected
        {
            if (item.PlayerIndex != 99)
            {
                NothingLeft = false; //If there's anything still not connected, set to false
            }
        }

        if (NothingLeft) //If there's nothing left, resume the game
        {
            EmergencyPause();
        } else
        {
            foreach (var item in ControlDC.CurrentDead) //If stuff still exists, update the text
            {
                if (item.PlayerIndex != 99)
                {
                    ControllerTemplate.text = "Player " + (item.PlayerIndex + 1) + ", ";
                }
            }
        }
    }
}
