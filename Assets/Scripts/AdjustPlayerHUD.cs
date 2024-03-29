using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

//Gets the player index and changes where on the screen the Player's information is located
public class AdjustPlayerHUD : MonoBehaviour
{
    private int pIndex;

    public RectTransform HUDObject;
    public Text playerNumText;
    public Text shotHit;

    private int Xdist = 100;
    private int Ydist = 50;

    private PlayerInputManager pMan;

    private void Start()
    {
        pIndex = gameObject.GetComponentInParent<PlayerInput>().playerIndex;
        pMan = FindObjectOfType<PlayerInputManager>();
        bool Single = false;

        if (pMan.playerCount == 1)
        {
            Single = true;
        }

        switch (pIndex)
        {
            case 0: //top left
                playerNumText.text = "Player 1";
                shotHit.text = "Shots taken: 0";

                if (!Single)
                {
                    playerNumText.color = new Color(57 * 1.0f / 255, 76 * 1.0f / 255, 255 * 1.0f / 255);
                }

                HUDObject.anchorMax = new Vector2(0, 1);
                HUDObject.anchorMin = new Vector2(0, 1);
                HUDObject.anchoredPosition = new Vector2(Xdist, -Ydist);
                break;

            case 1: //top right
                playerNumText.text = "Player 2";
                shotHit.text = "Shots taken: 0";

                if (!Single)
                {
                    playerNumText.color = new Color(255 * 1.0f / 255, 69 * 1.0f / 255, 59 * 1.0f / 255);
                }

                HUDObject.anchorMax = new Vector2(1, 1);
                HUDObject.anchorMin = new Vector2(1, 1);
                HUDObject.anchoredPosition = new Vector2(-Xdist, -Ydist);
                break;
               
            case 2: //bottom left
                playerNumText.text = "Player 3";
                shotHit.text = "Shots taken: 0";

                if (!Single)
                {
                    playerNumText.color = new Color(48 * 1.0f / 255, 255 * 1.0f / 255, 42 * 1.0f / 255);
                }

                HUDObject.anchorMax = new Vector2(0, 0);
                HUDObject.anchorMin = new Vector2(0, 0);
                HUDObject.anchoredPosition = new Vector2(Xdist, Ydist);
                break;

            case 3: //bottom right
                playerNumText.text = "Player 4";
                shotHit.text = "Shots taken: 0";

                if (!Single)
                {
                    playerNumText.color = new Color(255 * 1.0f / 255, 124 * 1.0f / 255, 250 * 1.0f / 255);
                }

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
