using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewInputAim : MonoBehaviour
{
    public Rigidbody2D BallPhysics; //The ball's physics cause velocity
    public GameObject ArrowOutline; //Arrow outline object
    public GameObject ArrowMask; //Arrow mask object

    public float RotMultiplier = 450f; //Multiplier for how fast the arrow should spin
    public float MaskScale = 5f; //Multiplier for how fast the mask should scale

    public MoveBall ScriptToMoveTheBall; //The script that applies the velocity

    public EscMenu PauseMenu; //Pause menu
    private bool PlayOK = true; //Whether it's OK to be in play or not

    private bool InMotion = false; //Is the ball moving

    private float ScaleX;

    private float AimingVal = 0;
    private float PowerVal = 0;

    //Input System Magic
    public void Aiming(InputAction.CallbackContext value)
    {
        AimingVal = value.ReadValue<float>();
    }

    public void Power(InputAction.CallbackContext value)
    {
        PowerVal = value.ReadValue<float>();
    }

    public void Shoot(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            ScriptToMoveTheBall.ReceiveBallInfo(ScaleX, ArrowOutline.transform.eulerAngles.z);
            TurnThingsOff();
            InMotion = true;
        }
    }

    //Rest of the Script
    private void Update()
    {
        //When the ball has stopped moving, enable things again
        if (InMotion == true && BallPhysics.velocity.magnitude < 0.005f && ScriptToMoveTheBall.FlagHitYet == false)
        {
            InMotion = false;
            TurnThingsOn();
        }

        if (InMotion == false && PlayOK == true) //As long as the ball isn't already moving or the game is paused
        {
            ArrowOutline.transform.Rotate(0, 0, -AimingVal * RotMultiplier * Time.deltaTime);

            ScaleX = ArrowMask.transform.localScale.x + (PowerVal * MaskScale * Time.deltaTime);
            ScaleX = Mathf.Clamp(ScaleX, 1, 3);
            ArrowMask.transform.localScale = new Vector3(ScaleX, ArrowMask.transform.localScale.y, ArrowMask.transform.localScale.z);
        }

        //Pause needs to be checked at the end of update to prevent ball from launching when closing menu
        if (PauseGame.pM.MenuIsOpen)
        {
            PlayOK = false;
        }
        else
        {
            PlayOK = true;
        }
    }

    private void TurnThingsOff()
    {
        ArrowOutline.SetActive(false);
    }

    private void TurnThingsOn()
    {
        ArrowOutline.SetActive(true);
        ArrowMask.transform.localScale = new Vector3(1f, 0.7f, 0);
    }
}
