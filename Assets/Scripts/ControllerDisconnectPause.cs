using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

//Handles controller disconnects during a game
public class ControllerDisconnectPause : MonoBehaviour
{
    public static ControllerDisconnectPause ControlDC;

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
            if (ControlDC.CurrentlyDC == false)
            {
                item.SwitchCurrentActionMap("UI");
                item.gameObject.GetComponent<MultiplayerEventSystem>().playerRoot = DisconnectPanel.gameObject;
                item.gameObject.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(null);
                item.gameObject.GetComponent<MultiplayerEventSystem>().firstSelectedGameObject = ReturnBtn;
                if (item.gameObject.TryGetComponent(out DragAndAimControllerManager manager))
                {
                    manager.SetToUI();
                }
            }
            else
            {
                item.SwitchCurrentActionMap("In-Game Ball");
                item.gameObject.GetComponent<MultiplayerEventSystem>().playerRoot = item.gameObject;
                item.gameObject.GetComponent<MultiplayerEventSystem>().firstSelectedGameObject = null;
                if (item.gameObject.TryGetComponent(out DragAndAimControllerManager manager))
                {
                    manager.SetToInGame();
                }
            }
        }

        if (ControlDC.CurrentlyDC == false)
        {
            Time.timeScale = 0;
            ControlDC.CurrentlyDC = true;
            DisconnectPanel.gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            ControlDC.CurrentlyDC = false;
            DisconnectPanel.gameObject.SetActive(false);
        }
    }

    //When controller disconnected, make this call with it's playerindex
    public void ControllerDisconnected(int playerIndex)
    {
        ControlDC.CurrentDead[playerIndex].PlayerIndex = playerIndex;
        if (ControlDC.CurrentlyDC == false)
        {
            ControllerTemplate.text = "Player " + (playerIndex + 1);
            EmergencyPause();
        } else
        {
            string NewText = "";
            foreach (var item in ControlDC.CurrentDead) //If stuff still exists, update the text
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
            string NewText = "";
            foreach (var item in ControlDC.CurrentDead) //If stuff still exists, update the text
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
}
