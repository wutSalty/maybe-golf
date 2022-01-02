using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class AdjustPlayerHUD : MonoBehaviour
{
    private int pIndex;

    public RectTransform HUDObject;
    public Text playerNumText;
    public Text shotHit;

    private int Xdist = 90;
    private int Ydist = 60;

    private void Start()
    {
        pIndex = gameObject.GetComponent<PlayerInput>().playerIndex;

        switch (pIndex)
        {
            case 0: //top left
                playerNumText.text = "Player 1";
                shotHit.text = "Shots taken: 0";

                HUDObject.anchorMax = new Vector2(0, 1);
                HUDObject.anchorMin = new Vector2(0, 1);
                HUDObject.anchoredPosition = new Vector2(Xdist, -Ydist);
                break;

            case 1: //top right
                playerNumText.text = "Player 2";
                shotHit.text = "Shots taken: 0";

                HUDObject.anchorMax = new Vector2(1, 1);
                HUDObject.anchorMin = new Vector2(1, 1);
                HUDObject.anchoredPosition = new Vector2(-Xdist, -Ydist);
                break;
               
            case 2: //bottom left
                playerNumText.text = "Player 3";
                shotHit.text = "Shots taken: 0";

                HUDObject.anchorMax = new Vector2(0, 0);
                HUDObject.anchorMin = new Vector2(0, 0);
                HUDObject.anchoredPosition = new Vector2(Xdist, Ydist);
                break;

            case 3: //bottom right
                playerNumText.text = "Player 4";
                shotHit.text = "Shots taken: 0";

                HUDObject.anchorMax = new Vector2(1, 0);
                HUDObject.anchorMin = new Vector2(1, 0);
                HUDObject.anchoredPosition = new Vector2(-Xdist, Ydist);
                break;

            default:
                break;
        }
    }

    //Top-left: 0,1 & 0,1
    //Top-right: 1,1 & 1,1
    //Bottom-left: 0,0 & 0,0
    //Bottom-right: 1,0 & 1,0
}
