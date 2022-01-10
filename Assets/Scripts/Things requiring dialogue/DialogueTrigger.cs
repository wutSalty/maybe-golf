using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

//Checks when to start dialogue
public class DialogueTrigger : MonoBehaviour
{
    public Dialogue FirstPlay; //Dialogue when scene first loaded
    public Dialogue FirstShoot; //Dialogue after first shoot
    public Dialogue FirstGoal; //Dialogue after goal is hit
    public Dialogue FirstDeath; //Dialogue if water touched

    private MoveBall MoveBallScript;

    //Flags to prevent text appearing more than once
    private bool FirstMove = false;
    private bool HitFlag = false;
    private bool WaterHit = false;

    //For first load
    private void Start()
    {
        MoveBallScript = GetComponent<MoveBall>();
        TriggerDialogue(FirstPlay);
    }

    private void Update()
    {
        //Checks whether the ball has been shot before
        if (FirstMove == false && MoveBallScript.NumHits == 1)
        {
            FirstMove = true;
            TriggerDialogue(FirstShoot);
        }

        if (MoveBallScript.FlagHitYet == true && HitFlag == false && GameStatus.gameStat.GameOver)
        {
            HitFlag = true;
            TriggerDialogue(FirstGoal);
        }
    }

    //Checks whether user died
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 && WaterHit == false)
        {
            WaterHit = true;
            TriggerDialogue(FirstDeath);
        }
    }

    public void TriggerDialogue(Dialogue dialogue)
    {
        DialogueManager.dMan.StartDialogue(dialogue);
    }
}
