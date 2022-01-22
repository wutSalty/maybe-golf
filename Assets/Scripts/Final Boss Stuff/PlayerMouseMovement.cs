using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMouseMovement : MonoBehaviour
{
    public SpriteRenderer ObjectBounds;
    public PlayerShootProjectile shootProjectile;

    Vector2 ScreenBounds;
    Vector2 mousepos;
    float YPos;
    float ObjectHeight;
    PlayerInput playerInput;
    int pIndex;

    string CurrentControls;
    float MovingValue;

    private void Start()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        pIndex = playerInput.playerIndex;
        ScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        ObjectHeight = ObjectBounds.bounds.extents.y;
        CurrentControls = playerInput.currentControlScheme;
    }

    void OnControlsChanged()
    {
        if (playerInput != null)
        {
            CurrentControls = playerInput.currentControlScheme;
        }
    }

    void OnDeviceLost()
    {
        BossControllerDisconnect.BossControlDC.ControllerDisconnected(pIndex);
    }

    void OnDeviceRegained()
    {
        BossControllerDisconnect.BossControlDC.ControllerConnected(pIndex);
    }

    void OnFlying(InputValue value)
    {
        MovingValue = value.Get<float>();
    }

    void Update()
    {
        if (CurrentControls == "Mouse" && !BossStatus.bossStat.GameOver && !BossStatus.bossStat.ForcePause && BossStatus.bossStat.GameStart && !BossPauseGame.bossPause.MenuIsOpen)
        {
            mousepos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            YPos = Mathf.Clamp(mousepos.y, ScreenBounds.y * -1 + ObjectHeight, ScreenBounds.y - ObjectHeight);
            gameObject.transform.position = new Vector3(transform.position.x, YPos, 0);
        }
        else
        {
            float Pos = gameObject.transform.position.y + (MovingValue * 10 * Time.deltaTime);
            YPos = Mathf.Clamp(Pos, ScreenBounds.y * -1 + ObjectHeight, ScreenBounds.y - ObjectHeight);
            gameObject.transform.position = new Vector3(transform.position.x, YPos, 0);
        }
    }

    public void DirtyQuit()
    {
        LoadingScreen.loadMan.BeginLoadingScene("MainMenu", false);
    }
}
