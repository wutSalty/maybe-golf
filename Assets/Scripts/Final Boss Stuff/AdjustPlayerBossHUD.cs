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
    private int YDist = 50;

    public RectTransform HUDObject;
    public Text PlayerText;

    private void Start()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        pIndex = playerInput.playerIndex;

        switch (pIndex)
        {
            case 0:
                PlayerText.text = "Player 1: ";

                HUDObject.anchorMax = new Vector2(0, 0);
                HUDObject.anchorMin = new Vector2(0, 0);
                HUDObject.anchoredPosition = new Vector2(XDist, YDist);
                break;

            case 1:
                PlayerText.text = "Player 2: ";

                HUDObject.anchorMax = new Vector2(0.5f, 0);
                HUDObject.anchorMin = new Vector2(0.5f, 0);
                HUDObject.anchoredPosition = new Vector2(-XDist, YDist);
                break;

            case 2:
                PlayerText.text = "Player 3: ";

                HUDObject.anchorMax = new Vector2(0.5f, 0);
                HUDObject.anchorMin = new Vector2(0.5f, 0);
                HUDObject.anchoredPosition = new Vector2(XDist, YDist);
                break;

            case 3:
                PlayerText.text = "Player 4: ";

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
