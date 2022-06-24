using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AltAim : MonoBehaviour
{
    public Rigidbody2D BallPhysics; //The ball's physics cause velocity
    public GameObject ArrowOutline; //Arrow outline object
    public GameObject ArrowMask; //Arrow mask object

    public float RotMultiplier = 450f; //Multiplier for how fast the arrow should spin
    public float MaskScale = 5f; //Multiplier for how fast the mask should scale

    public MoveBall ScriptToMoveTheBall; //The script that applies the velocity

    public Slider sensSlider;

    private bool PlayOK = true; //Whether it's OK to be in play or not

    private bool InMotion = false; //Is the ball moving

    private float ScaleX;

    private float AimingVal = 0;
    private float PowerVal = 0;
    private float AimingSensitivity = 4;

    [HideInInspector]
    public int playerIndex;

    private void Start()
    {
        playerIndex = gameObject.GetComponentInParent<PlayerInput>().playerIndex;
        ScriptToMoveTheBall.playerIndex = playerIndex;

        AimingSensitivity = GameManager.GM.NumPlayers[playerIndex].AimingSensitivity;
        sensSlider.value = AimingSensitivity;
    }

    //Input System Magic
    public void OnAiming(InputValue value)
    {
        AimingVal = value.Get<float>();
    }

    public void OnPower(InputValue value)
    {
        PowerVal = value.Get<float>();
    }

    public void OnShoot(InputValue value)
    {
        if (InMotion == false)
        {
            ScriptToMoveTheBall.ReceiveBallInfo(ScaleX, ArrowOutline.transform.eulerAngles.z);
            TurnThingsOff();
        }
    }

    //Rest of the Script
    private void Update()
    {
        if (InMotion == false && PlayOK == true) //As long as the ball isn't already moving or the game is paused
        {
            ArrowOutline.transform.Rotate(0, 0, -AimingVal * RotMultiplier * (AimingSensitivity / 4) * Time.deltaTime);

            ScaleX = ArrowMask.transform.localScale.x + (PowerVal * MaskScale * Time.deltaTime);
            ScaleX = Mathf.Clamp(ScaleX, 1, 3);
            ArrowMask.transform.localScale = new Vector3(ScaleX, ArrowMask.transform.localScale.y, ArrowMask.transform.localScale.z);
        }

        //Pause needs to be checked at the end of update to prevent ball from launching when closing menu
        if (PauseGame.pM.MenuIsOpen || ControllerDisconnectPause.ControlDC.CurrentlyDC)
        {
            PlayOK = false;
        }
        else
        {
            PlayOK = true;
        }
    }

    private void LateUpdate()
    {
        //When the ball has stopped moving, enable things again
        if (InMotion && BallPhysics.velocity.magnitude < 0.005f && !ScriptToMoveTheBall.FlagHitYet && !ScriptToMoveTheBall.CurrentlyDead && !PauseGame.pM.MenuIsOpen && !GameStatus.gameStat.ForcePause)
        {
            TurnThingsOn();
        }
    }

    //When the ball is moving
    public void TurnThingsOff()
    {
        InMotion = true;
        ArrowOutline.SetActive(false);
    }

    //When the ball has stopped moving
    private void TurnThingsOn()
    {
        InMotion = false;
        ArrowOutline.SetActive(true);
        ArrowMask.transform.localScale = new Vector3(1f, 0.7f, 0);
    }

    //When ball is asked to restart position
    void OnRestartBall()
    {
        if (ScriptToMoveTheBall.FlagHitYet == false)
        {
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
            BallPhysics.velocity = Vector2.zero;
            ArrowOutline.transform.rotation = new Quaternion(0, 0, 0, 0);
            ArrowMask.transform.localScale = new Vector3(1f, 0.7f, 0);
            gameObject.layer = 8;

            GameStatus.gameStat.AddGhostData(0, 0, true);
        }
    }

    //Updates the sensitivity of aiming locally and (if by yourself) globally
    public void UpdateSensitivity(float value)
    {
        AimingSensitivity = value;
        GameManager.GM.NumPlayers[playerIndex].AimingSensitivity = value;

        if (GameManager.GM.SingleMode)
        {
            PlayerPrefs.SetFloat("Sensitivity", value);
            PlayerPrefs.Save();
        }
    }
}
