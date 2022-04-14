using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBallMove : MonoBehaviour
{
    public List<GhostData> ghostData; //Holds all the instruction for the ball to move

    //Replicate the movement of an actual ball
    public int VelocityMultiplier = 10;
    public int RotateMultiplier = 100;

    //Ball elements that are manipulated
    private Rigidbody2D BallRigidbody;
    private Vector3 LastBallLocation;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        //Get required components
        BallRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //Get ghostData and check whether it's ok to play
        ghostData = GameManager.GM.LevelData[GameStatus.gameStat.GMLevelIndex].ghostData;
        if (ghostData.Count == 0)
        {
            Destroy(gameObject); //If data does not exist, delete itself
        }
        else
        {
            //Wait for the go ahead from GameStat
        }
    }

    public void StartReplay()
    {
        StartCoroutine(IterateSteps());
    }

    private void Update()
    {
        float BallVelocity = BallRigidbody.velocity.magnitude * RotateMultiplier;
        BallRigidbody.transform.Rotate(0, 0, BallVelocity * Time.deltaTime);
    }

    IEnumerator IterateSteps()
    {
        foreach (var item in ghostData)
        {
            yield return new WaitForSeconds(item.Timing);
            if (item.ResetPos)
            {
                RestartPos();
            } else
            {
                ReceiveBallInfo(item.HitPower, item.HitAngle);
            }
        }
    }

    public void ReceiveBallInfo(float HitStrength, float HitAngle)
    {
        //NumHits += 1;
        //NumHitsText.text = "Shots taken: " + NumHits;
        //if (NumHits >= 1)
        //{
        //    gameObject.layer = 7;
        //}

        //Debug.Log("Hitstrength: " + HitStrength + " Hitangle: " + HitAngle);

        LastBallLocation = transform.position;

        HitStrength = HitStrength * VelocityMultiplier;

        float XDir = HitStrength * Mathf.Cos(HitAngle * Mathf.Deg2Rad);
        float YDir = HitStrength * Mathf.Sin(HitAngle * Mathf.Deg2Rad);

        BallRigidbody.velocity = new Vector3(XDir, YDir);
    }

    public void RestartPos()
    {
        BallRigidbody.velocity = Vector2.zero;
        BallRigidbody.gameObject.transform.localPosition = Vector3.zero;
        BallRigidbody.gameObject.transform.localRotation = Quaternion.identity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 || collision.CompareTag("Enemy")) //When the ball hits the water or illegal area
        {
            //CurrentlyDead = true;
            BallRigidbody.velocity = new Vector2(0, 0);
            StartCoroutine(DeathFade());
        }

        if (collision.CompareTag("Flag")) //When the ball hits the flag
        {
            Debug.Log("Ghost cleared");

            BallHasWon();
        }
    }

    public void BallHasWon()
    {
        //gameObject.SendMessage("TurnThingsOff");
        //FlagHitYet = true;
        BallRigidbody.velocity = Vector2.zero;
        gameObject.layer = 8;
        //GameStatus.gameStat.SubmitRecord(playerIndex, NumHits, this);
    }

    IEnumerator DeathFade()
    {
        float targetValue = 0;
        float duration = 1;
        float startValue = 0.6980392f;
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

        BallRigidbody.transform.position = LastBallLocation;

        targetValue = 0.6980392f;
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
        //CurrentlyDead = false;
    }
}
