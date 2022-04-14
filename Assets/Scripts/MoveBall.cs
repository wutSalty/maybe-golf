using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

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

    public AudioSource[] audios; //0=water, 1=flag, 2=golf hit, 3=enemy

    public ParticleSystem flagParticle;
    public ParticleSystem waterParticle;

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

    public GameObject fcParticle;
    private GameObject particleObject;
    private bool SpecialBall;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        foreach (var item in audios)
        {
            item.volume = PlayerPrefs.GetFloat("InGame", 5) / 10;
        }

        if (GameManager.GM.FullCleared)
        {
            particleObject = Instantiate(fcParticle, TheBall.transform, false);
            SpecialBall = true;
        }
    }

    //Every frame, check has the ball stopped moving yet and to rotate the ball while in motion
    private void Update()
    {
        float BallVelocity = TheBall.velocity.magnitude * RotateMultiplier;
        TheBall.transform.Rotate(0, 0, BallVelocity * Time.deltaTime);

        if (SpecialBall)
        {
            particleObject.transform.localRotation = Quaternion.identity;
        }
    }

    //Handles the ball going into areas it might be in
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6) //When the ball hits the water or illegal area
        {
            waterParticle.transform.localPosition = TheBall.transform.localPosition;

            if (collision.CompareTag("TheVoid"))
            {

            }
            else if (collision.CompareTag("Lava"))
            {
                var main = waterParticle.main;
                main.startColor = new Color(245 / 255f, 104 / 255f, 22/ 225f, 255 / 255f);
                audios[0].Play();
                waterParticle.Play();
            }
            else
            {
                var main = waterParticle.main;
                main.startColor = new Color(0, 136 / 255f, 255 / 255f, 255 / 255f);
                audios[0].Play();
                waterParticle.Play();
            }

            CurrentlyDead = true;
            TheBall.velocity = new Vector2(0, 0);
            StartCoroutine(DeathFade());
        }
        else if (collision.CompareTag("Enemy"))
        {
            //audio
            //particle
            audios[3].Play();

            CurrentlyDead = true;
            TheBall.velocity = new Vector2(0, 0);
            StartCoroutine(DeathFade());
        }

        if (collision.CompareTag("Flag")) //When the ball hits the flag
        {
            Debug.Log("Player " + (playerIndex + 1) + " cleared");

            BallHasWon();
        }
    }

    //Gets the info from Ballthings and converts into angle and speed, then launches the ball
    public void ReceiveBallInfo(float HitStrength, float HitAngle)
    {
        audios[2].Play();

        NumHits += 1;
        NumHitsText.text = "Shots taken: " + NumHits;
        if (NumHits >= 1)
        {
            gameObject.layer = 7;
        }

        Debug.Log("Hitstrength: " + HitStrength + " Hitangle: " + HitAngle);

        //If the game is singleplayer and or this is the only player, then add list to ghost data
        if (GameManager.GM.SingleMode && playerIndex == 0)
        {
            GameStatus.gameStat.AddGhostData(HitStrength, HitAngle, false);
        }

        LastBallLocation = TheBall.transform.position;

        HitStrength = HitStrength * VelocityMultiplier;

        float XDir = HitStrength * Mathf.Cos(HitAngle * Mathf.Deg2Rad);
        float YDir = HitStrength * Mathf.Sin(HitAngle * Mathf.Deg2Rad);

        TheBall.velocity = new Vector3(XDir, YDir);
    }

    //Handles when ball has hit flag
    public void BallHasWon()
    {
        audios[1].Play();
        flagParticle.transform.localPosition = TheBall.transform.localPosition;
        flagParticle.Play();

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
