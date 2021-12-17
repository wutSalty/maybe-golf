using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour
{
    public Rigidbody2D TheBall; //The ball itself
    public FlagWin TheScriptForFlagWin; //the script for the flag
    public int RotateMultiplier = 100; //Multiply how fast the ball rotates
    public int VelocityMultiplier = 15; //Multiply how fast the ball goes

    public GameObject MaskSprite;
    public GameObject InsideSprite;

    [HideInInspector]
    public bool FlagHitYet = false; //Has the ball hit the flag yet

    private Vector3 LastBallLocation; //The ball's last location
    
    //Every frame, check has the ball stopped moving yet and to rotate the ball while in motion
    private void Update()
    {
        //if (FlagHitYet == true)
        //{
        //    StartCoroutine(ManualVelocityOverride());
        //}

        float BallVelocity = TheBall.velocity.magnitude * RotateMultiplier;
        TheBall.transform.Rotate(0, 0, BallVelocity * Time.deltaTime);
    }

    //private IEnumerator ManualVelocityOverride()
    //{
    //    var t = 0f;
    //    while (t < 1)
    //    {
    //        t += Time.deltaTime / 0.5f;
    //        TheBall.velocity = TheBall.velocity * 0.30f * Time.deltaTime;
    //        yield return null;
    //    }
    //}

    //Reset the ball to it's last position if it enters death area
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            TheBall.velocity = new Vector2(0, 0);
            TheBall.transform.position = LastBallLocation;
            Debug.Log("Death");
        }
    }

    //Gets the info from Ballthings and converts into angle and speed, then launches the ball
    public void ReceiveBallInfo(float HitStrength, float HitAngle)
    {
        Debug.Log("Hitstrength: " + HitStrength + " Hitangle: " + HitAngle);

        LastBallLocation = TheBall.transform.position;

        HitStrength = HitStrength * VelocityMultiplier;

        float XDir = HitStrength * Mathf.Cos(HitAngle * Mathf.Deg2Rad);
        float YDir = HitStrength * Mathf.Sin(HitAngle * Mathf.Deg2Rad);

        TheBall.velocity = new Vector3(XDir, YDir);
    }
}
