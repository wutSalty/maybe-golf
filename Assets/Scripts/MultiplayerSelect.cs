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

    public Text P1Connect;
    public Text P2Connect;
    public Text P3Connect;
    public Text P4Connect;

    private bool CurrentlyLoading = false;

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
        string ControlName;
        if(value.playerIndex == 0) //If Player 1
        {
            GameManager.GM.NumPlayers.Add(new MultiPlayerClass { ControlType = PlayerPrefs.GetInt("InputType", 0) }); //Create new array
            if (PlayerPrefs.GetInt("InputType", 0) == 0) //Set input type text
            {
                ControlName = "Mouse/Drag";
            } else
            {
                ControlName = "Keyboard/Buttons";
            }
        } else //If more than player 1, check if entry doesn't already exist
        {
            if (GameManager.GM.NumPlayers.Count - 1 < value.playerIndex) //If not, add new entry
            {
                GameManager.GM.NumPlayers.Add(new MultiPlayerClass { ControlType = 1 });
            } //If it already exists, skip this step

            //GameManager.GM.NumPlayers.Add(new MultiPlayerClass { ControlType = 1 });
            ControlName = "Controller/Buttons";
        }

        switch (value.playerIndex)
        {
            case 0:
                P1Connect.text = "Connected\n" + ControlName;
                GameManager.GM.NumPlayers[0].PlayerIndex = 0;
                GameManager.GM.NumPlayers[0].inputDevice = InputUser.all[value.playerIndex].pairedDevices[0];
                break;

            case 1:
                P2Connect.text = "Connected\n" + ControlName;
                GameManager.GM.NumPlayers[1].PlayerIndex = 1;
                GameManager.GM.NumPlayers[1].inputDevice = InputUser.all[value.playerIndex].pairedDevices[0];
                break;

            case 2:
                P3Connect.text = "Connected\n" + ControlName;
                GameManager.GM.NumPlayers[2].PlayerIndex = 2;
                GameManager.GM.NumPlayers[2].inputDevice = InputUser.all[value.playerIndex].pairedDevices[0];
                break;

            case 3:
                P4Connect.text = "Connected\n" + ControlName;
                GameManager.GM.NumPlayers[3].PlayerIndex = 3;
                GameManager.GM.NumPlayers[3].inputDevice = InputUser.all[value.playerIndex].pairedDevices[0];
                break;

            default:
                break;
        };
        //Debug.Log(InputUser.all[value.playerIndex].pairedDevices[0]);
    }

    public void WhenAPlayerDisconnects(PlayerInput value)
    {
        if (CurrentlyLoading == false)
        {
            switch (value.playerIndex)
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
        }
    }

    public void PlayReady()
    {
        CurrentlyLoading = true;
        SceneManager.LoadScene("SampleScene");
    }
}
