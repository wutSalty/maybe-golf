using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue FirstPlay;
    public Dialogue FirstShoot;
    public Dialogue FirstGoal;
    public Dialogue FirstDeath;

    public MoveBall MoveBallScript;
    public PlayerInput playerInput;

    private bool FirstMove = false;
    private bool HitFlag = false;
    private bool WaterHit = false;

    private void Start()
    {
        DialogueManager.dMan.StartDialogue(FirstPlay, playerInput);
    }

    private void Update()
    {
        if (FirstMove == false && MoveBallScript.NumHits == 1)
        {
            FirstMove = true;
            TriggerDialogue(FirstShoot);
        }

        if (MoveBallScript.FlagHitYet == true && HitFlag == false)
        {
            HitFlag = true;
            TriggerDialogue(FirstGoal);
        }
    }

    public void TriggerDialogue(Dialogue dialogue)
    {
        DialogueManager.dMan.StartDialogue(dialogue, playerInput);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 && WaterHit == false)
        {
            WaterHit = true;
            TriggerDialogue(FirstDeath);
        }
    }
}
