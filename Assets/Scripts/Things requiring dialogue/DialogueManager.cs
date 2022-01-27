using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager dMan;

    public Text NameField; //NPC Name
    public Text DialogueField; //NPC Dialogue box
    public GameObject ContinueButton; //The continue button
    public Animator animator; //Animator to animate box

    [HideInInspector]
    public Queue<string> sentences; //The list of dialogue that needs to be said

    //Various inputs to manipulate during play
    [HideInInspector]
    public PlayerInput playerInput;
    [HideInInspector]
    public MultiplayerEventSystem multiplayerEvent;
    [HideInInspector]
    public InputSystemUIInputModule inputSystem;

    //Required when hijacking results screen
    [HideInInspector]
    public GameObject LastSelectedObject;

    //Holds coroutine so only this routine can be stopped
    [HideInInspector]
    public Coroutine TypeSentenceRoutine;

    //Everything down here is to handle buttons, controllers, and keyboards
    [HideInInspector]
    public string ControlScheme;

    [HideInInspector]
    public string ControllerA = "To get started, use the LEFT STICK and RIGHT STICK to aim, and SUBMIT to shoot.";
    [HideInInspector]
    public string KeyboardA = "To get started, use the ARROW KEYS to aim, and ENTER to shoot.";

    [HideInInspector]
    public string ControllerB = "If you ever need to restart your position back to the start, press LB + RB. And use MENU to pause the game.";
    [HideInInspector]
    public string KeyboardB = "If you ever need to restart your position back to the start, press R. And use ESC to pause the game.";

    public bool DialogueOpen;

    //When awake, grab necessary components
    private void Awake()
    {
        if (dMan != null && dMan != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            dMan = this;
        }
        dMan.sentences = new Queue<string>();

        multiplayerEvent = GetComponentInParent<MultiplayerEventSystem>();
        inputSystem = GetComponentInParent<InputSystemUIInputModule>();
        playerInput = GetComponentInParent<PlayerInput>();
    }

    //Every frame, check whether the player has changed controls (required for tutorial text)
    private void Update()
    {
        if (playerInput.currentControlScheme != ControlScheme)
        {
            ControlScheme = playerInput.currentControlScheme;
        }
    }

    //Dialogue gets passed into here. Pauses the timer and sets up the dialogue box
    public void StartDialogue(Dialogue dialogue)
    {
        GameStatus.gameStat.DialogueOpen = true;
        DialogueOpen = true;

        dMan.sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        NameField.text = dialogue.name;

        StartCoroutine(HahaDelay());
        animator.SetBool("IsOpen", true);

        DisplayNextSentence();
    }

    //For every string of sentences, gets passed into here. It also checks if any strings need replacing then begings displaying them
    public void DisplayNextSentence()
    {
        //Debug.Log("DisplayNextSentence has been pressed");
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();

        if (sentence.Contains("<CheckA>"))
        {
            if (ControlScheme == "Keyboard") //Keyboard
            {
                sentence = KeyboardA;
            } else //Controller
            {
                sentence = ControllerA;
            }
        } else if (sentence.Contains("<CheckB>"))
        {
            if (ControlScheme == "Keyboard") //Keyboard
            {
                sentence = KeyboardB;
            }
            else //Controller
            {
                sentence = ControllerB;
            }
        }

        if (TypeSentenceRoutine != null)
        {
            StopCoroutine(TypeSentenceRoutine);
        }

        TypeSentenceRoutine = StartCoroutine(TypeSentence(sentence));
    }

    //Types out each letter of the dialogue one at a time
    IEnumerator TypeSentence(string sentence)
    {
        DialogueField.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            DialogueField.text += letter;
            yield return new WaitForSeconds(0.002f);
        }
    }
    
    //If there's no more dialogue, return input to where ever
    public void EndDialogue()
    {
        animator.SetBool("IsOpen", false);

        if (GameStatus.gameStat.GameOver)
        {
            playerInput.SwitchCurrentActionMap("UI");
            multiplayerEvent.playerRoot = GameObject.FindGameObjectWithTag("Results Screen");

            StartCoroutine(HindsightIsFun());

            multiplayerEvent.firstSelectedGameObject = LastSelectedObject;
            multiplayerEvent.SetSelectedGameObject(LastSelectedObject);
        } else
        {
            if (playerInput.gameObject.TryGetComponent(out DragAndAimControllerManager manager))
            {
                manager.SetToUI();
            }

            playerInput.SwitchCurrentActionMap("In-Game Ball");
            multiplayerEvent.playerRoot = playerInput.gameObject;

            multiplayerEvent.SetSelectedGameObject(null);
            multiplayerEvent.firstSelectedGameObject = null;
        }
        ContinueButton.SetActive(false);
        DialogueOpen = false;
        GameStatus.gameStat.DialogueOpen = false;
    }

    //When dialogue box is needed, steal input to button can be clicked
    IEnumerator HahaDelay()
    {
        yield return new WaitForSeconds(0.02f);

        if (GameStatus.gameStat.GameOver)
        {
            playerInput.SwitchCurrentActionMap("UI");
            multiplayerEvent.playerRoot = this.gameObject;

            LastSelectedObject = multiplayerEvent.firstSelectedGameObject;

            multiplayerEvent.SetSelectedGameObject(null);
            multiplayerEvent.firstSelectedGameObject = null;

            multiplayerEvent.firstSelectedGameObject = ContinueButton;
            multiplayerEvent.SetSelectedGameObject(ContinueButton);
        }
        else
        {
            //Oldtrial
            if (playerInput.gameObject.TryGetComponent(out DragAndAimControllerManager manager))
            {
                manager.SetToUI();
            }

            playerInput.SwitchCurrentActionMap("UI");

            multiplayerEvent.playerRoot = this.gameObject;

            multiplayerEvent.SetSelectedGameObject(null);
            multiplayerEvent.firstSelectedGameObject = null;

            multiplayerEvent.SetSelectedGameObject(ContinueButton);
            multiplayerEvent.firstSelectedGameObject = ContinueButton;

            multiplayerEvent.SetSelectedGameObject(ContinueButton);
        }

        ContinueButton.SetActive(true);
        inputSystem.enabled = true;
    }

    //Just a delay for the closing dialogue
    IEnumerator HindsightIsFun()
    {
        yield return new WaitForSeconds(0.02f);
    }
}
