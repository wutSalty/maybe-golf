using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AdjustPlayerBossHUD : MonoBehaviour
{
    private PlayerInput playerInput;
    private int pIndex;

    private int XDist = 120;
    private int YDist = 35;

    public RectTransform HUDObject;
    public Text PlayerText;

    private PlayerInputManager pMan;

    private void Start()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        pIndex = playerInput.playerIndex;
        pMan = FindObjectOfType<PlayerInputManager>();
        bool Single = false;

        if (pMan.playerCount == 1)
        {
            Single = true;
        }
        switch (pIndex)
        {
            case 0:
                PlayerText.text = "Player 1: ";

                if (!Single)
                {
                    PlayerText.color = new Color(57 * 1.0f / 255, 76 * 1.0f / 255, 255 * 1.0f / 255);
                }

                HUDObject.anchorMax = new Vector2(0, 0);
                HUDObject.anchorMin = new Vector2(0, 0);
                HUDObject.anchoredPosition = new Vector2(XDist, YDist);
                break;

            case 1:
                PlayerText.text = "Player 2: ";

                if (!Single)
                {
                    PlayerText.color = new Color(255 * 1.0f / 255, 31 * 1.0f / 255, 18 * 1.0f / 255);
                }

                HUDObject.anchorMax = new Vector2(0.5f, 0);
                HUDObject.anchorMin = new Vector2(0.5f, 0);
                HUDObject.anchoredPosition = new Vector2(-XDist, YDist);
                break;

            case 2:
                PlayerText.text = "Player 3: ";

                if (!Single)
                {
                    PlayerText.color = new Color(48 * 1.0f / 255, 255 * 1.0f / 255, 42 * 1.0f / 255);
                }

                HUDObject.anchorMax = new Vector2(0.5f, 0);
                HUDObject.anchorMin = new Vector2(0.5f, 0);
                HUDObject.anchoredPosition = new Vector2(XDist, YDist);
                break;

            case 3:
                PlayerText.text = "Player 4: ";

                if (!Single)
                {
                    PlayerText.color = new Color(255 * 1.0f / 255, 20 * 1.0f / 255, 246 * 1.0f / 255);
                }

                HUDObject.anchorMax = new Vector2(1, 0);
                HUDObject.anchorMin = new Vector2(1, 0);
                HUDObject.anchoredPosition = new Vector2(-XDist, YDist);
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
