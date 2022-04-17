using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMouseMovement : MonoBehaviour
{
    public SpriteRenderer ObjectBounds;
    public PlayerShootProjectile shootProjectile;

    //Vector2 ScreenBounds;
    Vector2 mousepos;
    float YPos;
    float ObjectHeight;

    float LastYPos;

    PlayerInput playerInput;
    int pIndex;

    PlayerHealth pHealth;

    string CurrentControls;
    float MovingValue;

    private void Start()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        pIndex = playerInput.playerIndex;
        //ScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        ObjectHeight = ObjectBounds.bounds.extents.y;
        CurrentControls = playerInput.currentControlScheme;
        pHealth = GetComponent<PlayerHealth>();
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
        if ((BossControllerDisconnect.BossControlDC.CurrentlyDC || BossPauseGame.bossPause.MenuIsOpen || BossStatus.bossStat.GameOver || BossStatus.bossStat.ForcePause || pHealth.PlayerDead) && MovingValue != 0)
        {
            MovingValue = 0;
            return;
        }

        if (CurrentControls == "Mouse" && !BossControllerDisconnect.BossControlDC.CurrentlyDC && !BossStatus.bossStat.GameOver && !BossStatus.bossStat.ForcePause && BossStatus.bossStat.GameStart && !BossPauseGame.bossPause.MenuIsOpen && !pHealth.PlayerDead)
        {
            LastYPos = YPos;

            mousepos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            YPos = Mathf.Clamp(mousepos.y, 5 * -1 + ObjectHeight, 5 - ObjectHeight);

            float diff = (YPos - LastYPos) * Time.deltaTime * 7;
            //if (Mathf.Abs(diff) > 0.2f)
            //{
            //    float Pos = gameObject.transform.position.y + (diff * 10 * Time.deltaTime);
            //    YPos = Mathf.Clamp(Pos, 5 * -1 + ObjectHeight, 5 - ObjectHeight);
            //}

            if (diff > 0)
            {
                diff = Mathf.Clamp(diff, 0, 1);

            } else if (diff < 0)
            {
                diff = Mathf.Clamp(diff, -1, 0);
            }

            //float Pos = gameObject.transform.position.y + (diff * 10 * Time.deltaTime);
            float Pos = gameObject.transform.position.y + (diff);
            YPos = Mathf.Clamp(Pos, 5 * -1 + ObjectHeight, 5 - ObjectHeight);
            gameObject.transform.position = new Vector3(transform.position.x, YPos, 0);
        }
        else
        {
            float Pos = gameObject.transform.position.y + (MovingValue * 10 * Time.deltaTime);
            YPos = Mathf.Clamp(Pos, 5 * -1 + ObjectHeight, 5 - ObjectHeight);
            gameObject.transform.position = new Vector3(transform.position.x, YPos, 0);
        }
    }
}
