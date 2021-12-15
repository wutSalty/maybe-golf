using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerSelect : MonoBehaviour
{
    public Slider PlayerSelectSlider;
    public Text NoOfPlayers;

    public void SliderChange()
    {
        switch (PlayerSelectSlider.value)
        {
            case 2:
                NoOfPlayers.text = "2 Players";
                break;

            case 3:
                NoOfPlayers.text = "3 Players";
                break;

            case 4:
                NoOfPlayers.text = "4 Players";
                break;

            default:
                break;
        }
    }

    public void PlayReady()
    {
        for (int i = 1; i < (PlayerSelectSlider.value); i++)
        {
            GameManager.GM.NumPlayers.Add(new MultiPlayerClass { ControlType = 1});
        }
    }
}
