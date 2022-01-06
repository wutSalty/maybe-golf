using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveBall : MonoBehaviour
{
    public Rigidbody2D TheBall; //The ball itself
    public FlagWin TheScriptForFlagWin; //the script for the flag
    public int RotateMultiplier = 100; //Multiply how fast the ball rotates
    public int VelocityMultiplier = 15; //Multiply how fast the ball goes

    public GameObject MaskSprite;
    public GameObject InsideSprite;

    public Text NumHitsText;
    public Text TimeTakenText;

    private Vector3 LastBallLocation; //The ball's last location
    private int NumHits;

    [HideInInspector]
    public bool FlagHitYet = false; //Has the ball hit the flag yet

    [HideInInspector]
    public int playerIndex;

    //Every frame, check has the ball stopped moving yet and to rotate the ball while in motion
    private void Update()
    {
        float BallVelocity = TheBall.velocity.magnitude * RotateMultiplier;
        TheBall.transform.Rotate(0, 0, BallVelocity * Time.deltaTime);
    }

    //Handles the ball going into areas it might be in
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            TheBall.velocity = new Vector2(0, 0);
            TheBall.transform.position = LastBallLocation;
            Debug.Log("Ball has hit death");
        }

        if (collision.tag == "Flag")
        {
            Debug.Log("Player " + (playerIndex + 1) + "cleared");

            BallHasWon();
        }
    }

    //Gets the info from Ballthings and converts into angle and speed, then launches the ball
    public void ReceiveBallInfo(float HitStrength, float HitAngle)
    {
        NumHits += 1;
        NumHitsText.text = "Shots taken: " + NumHits;
        if (NumHits >= 1)
        {
            gameObject.layer = 7;
        }

        Debug.Log("Hitstrength: " + HitStrength + " Hitangle: " + HitAngle);

        LastBallLocation = TheBall.transform.position;

        HitStrength = HitStrength * VelocityMultiplier;

        float XDir = HitStrength * Mathf.Cos(HitAngle * Mathf.Deg2Rad);
        float YDir = HitStrength * Mathf.Sin(HitAngle * Mathf.Deg2Rad);

        TheBall.velocity = new Vector3(XDir, YDir);
    }

    //Get call from flag. Once hit flag, stop ball and pass info to game status
    public void BallHasWon()
    {
        FlagHitYet = true;
        TheBall.velocity = Vector2.zero;
        gameObject.layer = 8;
        GameStatus.gameStat.SubmitRecord(playerIndex, NumHits, this);
    }

    public void EmergancyEscape()
    {
        FlagHitYet = true;
        TheBall.velocity = Vector2.zero;
        gameObject.layer = 8;
    }

    public void UpdateTimerText(float text)
    {
        Debug.Log(text);
        TimeTakenText.text = "Time Taken: " + text.ToString("F2");
    }
}
