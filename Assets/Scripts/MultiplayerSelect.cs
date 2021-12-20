using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class MultiplayerSelect : MonoBehaviour
{
    public Slider PlayerSelectSlider;
    public Text NoOfPlayers;
    public Button PlayBtn;
    public Text PlayText;

    public Text P1Connect;
    public Text P2Connect;
    public Text P3Connect;
    public Text P4Connect;

    public PlayerInputManager inputManager;

    [HideInInspector]
    public bool CurrentlyLoading = false;

    private void Start()
    {
        CurrentlyLoading = false;
        PlayBtn.interactable = false;
        PlayText.text = "Waiting for players...";
    }

    //Updates text for P1 when entering menu
    public void UpdatePlayerOneText()
    {
        if (PlayerPrefs.GetInt("InputType", 0) == 0) //Set input type text
        {
            P1Connect.text = "Connected\nClick and Drag";
        }
        else
        {
            P1Connect.text = "Connected\nKeyboard/Buttons";
        }
    }

    public void WhenAPlayerJoins(PlayerInput value)
    {
        string ControlName = "Demo";
        int ControlType = 0;

        //Checks if enough indexes are available, if not create new index
        if (GameManager.GM.NumPlayers.Count - 1 < value.playerIndex)
        {
            GameManager.GM.NumPlayers.Add(new MultiPlayerClass { ControlType = PlayerPrefs.GetInt("InputType", 0) });
        }

        //Checks device type being connected
        if (value.currentControlScheme == "Controller")
        {
            ControlName = "Controller/Buttons";
            ControlType = 2; //2 = Controller buttons
        } else
        {
            if (PlayerPrefs.GetInt("InputType", 0) == 1)
            {
                ControlName = "Keyboard/Buttons";
                ControlType = 1; //1 = Keyboard Buttons
            } else
            {
                ControlName = "Click and Drag";
                ControlType = 0; //0 = Mouse click and drag
            }
        }

        //Depending on what number player joined, change text and assign data items to array
        switch (value.playerIndex)
        {
            case 0:
                P1Connect.text = "Connected\n" + ControlName;
                GameManager.GM.NumPlayers[0].PlayerIndex = 0;
                GameManager.GM.NumPlayers[0].ControlType = ControlType;
                GameManager.GM.NumPlayers[0].inputDevice = InputUser.all[value.playerIndex].pairedDevices[0];
                Debug.Log(InputUser.all[value.playerIndex].pairedDevices[0]);
                break;

            case 1:
                P2Connect.text = "Connected\n" + ControlName;
                GameManager.GM.NumPlayers[1].PlayerIndex = 1;
                GameManager.GM.NumPlayers[1].ControlType = ControlType;
                GameManager.GM.NumPlayers[1].inputDevice = InputUser.all[value.playerIndex].pairedDevices[0];
                break;

            case 2:
                P3Connect.text = "Connected\n" + ControlName;
                GameManager.GM.NumPlayers[2].PlayerIndex = 2;
                GameManager.GM.NumPlayers[2].ControlType = ControlType;
                GameManager.GM.NumPlayers[2].inputDevice = InputUser.all[value.playerIndex].pairedDevices[0];
                break;

            case 3:
                P4Connect.text = "Connected\n" + ControlName;
                GameManager.GM.NumPlayers[3].PlayerIndex = 3;
                GameManager.GM.NumPlayers[3].ControlType = ControlType;
                GameManager.GM.NumPlayers[3].inputDevice = InputUser.all[value.playerIndex].pairedDevices[0];
                break;

            default:
                break;
        };
        
        //Once more than 1 player is present, allow game to start
        if (inputManager.playerCount > 1)
        {
            PlayBtn.interactable = true;
            PlayText.text = "Ready to Play!";
        }
    }

    public void WhenAPlayerDisconnects(PlayerInput value)
    {
        if (CurrentlyLoading == false) //Needs check in-case game is changing scenes
        {
            switch (value.playerIndex) //Depending on the player that disconnected, update stuff
            {
                case 0:
                    P1Connect.text = "Not Connected";
                    GameManager.GM.NumPlayers[0].PlayerIndex = 99;
                    break;

                case 1:
                    P2Connect.text = "Not Connected";
                    GameManager.GM.NumPlayers[1].PlayerIndex = 99;
                    break;

                case 2:
                    P3Connect.text = "Not Connected";
                    GameManager.GM.NumPlayers[2].PlayerIndex = 99;
                    break;

                case 3:
                    P4Connect.text = "Not Connected";
                    GameManager.GM.NumPlayers[3].PlayerIndex = 99;
                    break;

                default:
                    break;
            };
            
            if (inputManager.playerCount == 1) //If player left is only 1, disable play button
            {
                PlayBtn.interactable = false;
                PlayText.text = "Waiting for players...";
            }
        }
    }

    //The play button
    public void PlayReady()
    {
        GameManager.GM.SingleMode = false;
        CurrentlyLoading = true;
        SceneManager.LoadScene("SampleScene");
    }
}
