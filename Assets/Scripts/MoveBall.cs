using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Script that receives information to move the ball
public class MoveBall : MonoBehaviour
{
    public Rigidbody2D TheBall; //The ball itself
    public FlagWin TheScriptForFlagWin; //the script for the flag
    public int RotateMultiplier = 100; //Multiply how fast the ball rotates
    public int VelocityMultiplier = 5; //Multiply how fast the ball goes

    public GameObject MaskSprite;
    public GameObject InsideSprite;

    public Text NumHitsText;
    public Text TimeTakenText;

    private Vector3 LastBallLocation; //The ball's last location

    [HideInInspector]
    public int NumHits;

    [HideInInspector]
    public bool FlagHitYet = false; //Has the ball hit the flag yet

    [HideInInspector]
    public int playerIndex;

    [HideInInspector]
    public bool CurrentlyDead;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //Every frame, check has the ball stopped moving yet and to rotate the ball while in motion
    private void Update()
    {
        float BallVelocity = TheBall.velocity.magnitude * RotateMultiplier;
        TheBall.transform.Rotate(0, 0, BallVelocity * Time.deltaTime);
    }

    //Handles the ball going into areas it might be in
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6) //When the ball hits the water or illegal area
        {
            CurrentlyDead = true;
            TheBall.velocity = new Vector2(0, 0);
            StartCoroutine(DeathFade());
        }

        if (collision.tag == "Flag") //When the ball hits the flag
        {
            Debug.Log("Player " + (playerIndex + 1) + " cleared");

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

    //Handles when ball has hit flag
    public void BallHasWon()
    {
        gameObject.SendMessage("TurnThingsOff");
        FlagHitYet = true;
        TheBall.velocity = Vector2.zero;
        gameObject.layer = 8;
        GameStatus.gameStat.SubmitRecord(playerIndex, NumHits, this);
    }

    //Called when the scene is changing or restarting
    public void EmergancyEscape()
    {
        FlagHitYet = true;
        TheBall.velocity = Vector2.zero;
        gameObject.layer = 8;
    }

    //Updates the time taken text on player HUD
    public void UpdateTimerText(float text)
    {
        Debug.Log(text);
        TimeTakenText.text = "Time Taken: " + text.ToString("F2");
    }

    IEnumerator DeathFade()
    {
        float targetValue = 0;
        float duration = 1;
        float startValue = 1;
        float time = 0;
        float alpha;

        while (time < duration)
        {
            alpha = Mathf.Lerp(startValue, targetValue, time / duration);

            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);

            time += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, targetValue);

        TheBall.transform.position = LastBallLocation;

        targetValue = 1;
        startValue = 0;
        time = 0;

        while (time < duration)
        {
            alpha = Mathf.Lerp(startValue, targetValue, time / duration);

            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);

            time += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, targetValue);
        CurrentlyDead = false;
    }
}
