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

    public Text NameField;
    public Text DialogueField;
    public GameObject ContinueButton;
    public GameObject ReturnToSelectButton;
    public Animator animator;

    [HideInInspector]
    public Queue<string> sentences;

    [HideInInspector]
    public PlayerInput playerInput;

    [HideInInspector]
    public EventSystem eventSystem;

    [HideInInspector]
    public MultiplayerEventSystem multiplayerEvent;

    [HideInInspector]
    public InputSystemUIInputModule inputSystem;

    [HideInInspector]
    public GameObject LastSelectedObject;

    [HideInInspector]
    public Coroutine Thingy;

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
        eventSystem = GameObject.FindGameObjectWithTag("Respawn").GetComponent<EventSystem>();
        multiplayerEvent = GetComponentInParent<MultiplayerEventSystem>();
        inputSystem = GetComponentInParent<InputSystemUIInputModule>();
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("InputType", 0) == 0)
        {
            eventSystem.enabled = false;
        }
    }

    public void StartDialogue(Dialogue dialogue, PlayerInput pInput)
    {
        playerInput = pInput;
        
        GameStatus.gameStat.ForcePause = true;

        dMan.sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        NameField.text = dialogue.name;
        animator.SetBool("IsOpen", true);

        StartCoroutine(HahaDelay());
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        Debug.Log("DisplayNextSentence has been pressed");
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();

        if (Thingy != null)
        {
            StopCoroutine(Thingy);
        }

        Thingy = StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        DialogueField.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            DialogueField.text += letter;
            yield return new WaitForSeconds(0.002f);
        }
    }

    public void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
        eventSystem.SetSelectedGameObject(null);

        if (GameStatus.gameStat.GameOver)
        {
            playerInput.SwitchCurrentActionMap("UI");
            multiplayerEvent.playerRoot = GameObject.FindGameObjectWithTag("Results Screen");
            
            //eventSystem.firstSelectedGameObject = ReturnToSelectButton;
            //eventSystem.SetSelectedGameObject(ReturnToSelectButton);

            eventSystem.enabled = false;

            multiplayerEvent.firstSelectedGameObject = LastSelectedObject;
            multiplayerEvent.SetSelectedGameObject(LastSelectedObject);
        } else
        {
            playerInput.SwitchCurrentActionMap("In-Game Ball");
            eventSystem.SetSelectedGameObject(null);
            
            //multiplayerEvent.SetSelectedGameObject(null);
        }
        GameStatus.gameStat.ForcePause = false;
    }

    IEnumerator HahaDelay()
    {
        yield return new WaitForSeconds(0.02f);

        if (GameStatus.gameStat.GameOver)
        {
            eventSystem.enabled = false;

            playerInput.SwitchCurrentActionMap("UI");
            multiplayerEvent.playerRoot = this.gameObject;

            LastSelectedObject = multiplayerEvent.firstSelectedGameObject;

            multiplayerEvent.SetSelectedGameObject(null);
            multiplayerEvent.firstSelectedGameObject = null;

            //eventSystem.SetSelectedGameObject(null);

            multiplayerEvent.firstSelectedGameObject = ContinueButton;
            //eventSystem.firstSelectedGameObject = ContinueButton;

            multiplayerEvent.SetSelectedGameObject(ContinueButton);
            //eventSystem.SetSelectedGameObject(ContinueButton);
        }
        else
        {
            inputSystem.enabled = false;

            playerInput.SwitchCurrentActionMap("Not Caller Menu");

            eventSystem.SetSelectedGameObject(null);
            eventSystem.firstSelectedGameObject = null;

            eventSystem.SetSelectedGameObject(ContinueButton);
            eventSystem.firstSelectedGameObject = ContinueButton;
            
            //multiplayerEvent.SetSelectedGameObject(ContinueButton);
        }
        inputSystem.enabled = true;
    }
}
