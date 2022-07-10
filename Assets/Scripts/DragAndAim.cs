using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//The primary method to aiming. Mouse acts like a "sling shot"
public class DragAndAim : MonoBehaviour
{ 
    public Collider2D ClickableObject; //The object we need to click
    public GameObject TheObjectWeWantToMove; //The object we want to move
    public GameObject TheObjectShowingDirection; //An object to show direction of movement
    public GameObject TheActualArrow; //It's an arrow
    public GameObject TheMask; //Mask to show arrow strength
    public GameObject TheBall; //The script to pass info to move the ball

    GameObject targetObject; //Top layer collider object
    Vector3 offset; //Offset of object to mouse
    float XDist; //Distance of A to B in X
    float YDist; //Distance of A to B in Y
    float AngleOfAim; //The Angle the ball is intended to go towards
    float MaskScaleX; //The size of the spirte mask. Also the power of the shot

    public Rigidbody2D BallPhysics;
    public MoveBall BallMoveScript;
    private bool InMotion = false;

    private Vector3 mousePosition;

    //When mouse is clicked, check if it's the item we want
    public void OnMouseLeftStarted(InputValue value)
    {
        if (Physics2D.OverlapPoint(mousePosition) && PauseGame.pM.MenuIsOpen == false && ControllerDisconnectPause.ControlDC.CurrentlyDC == false)
        {
            Collider2D[] results = Physics2D.OverlapPointAll(mousePosition);
            Collider2D highestCollider = GetHighestObject(results);
            targetObject = highestCollider.transform.gameObject;

            offset = targetObject.transform.position - mousePosition;
        }
    }

    //When mouse is released and it is the object we want, shoot the ball
    public void OnMouseLeftCancelled()
    {
        if (ClickableObject.gameObject == targetObject)
        {
            targetObject = null;
            if (MaskScaleX <= 0.65f)
            {
                Debug.Log("Less than required");
            }
            else if (MaskScaleX > 0.65f)
            {
                BallMoveScript.ReceiveBallInfo(MaskScaleX, AngleOfAim);
                
                TurnThingsOff();
            }
        }
    }

    //Set components to easier accessed identifiers
    private void Start()
    {
        BallMoveScript.playerIndex = gameObject.GetComponentInParent<PlayerInput>().playerIndex;

        if (!GameManager.GM.SingleMode)
        {
            ClickableObject.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 61;
        }
    }

    //Every frame, check the position of the mouse and the movement of the ball
    private void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        //If the selected object is the one we want, adjust direction and power
        if (ClickableObject.gameObject == targetObject)
        {
            TheObjectWeWantToMove.transform.position = mousePosition + offset;

            XDist = TheObjectWeWantToMove.transform.position.x - ClickableObject.transform.position.x;
            YDist = TheObjectWeWantToMove.transform.position.y - ClickableObject.transform.position.y;

            Vector3 CombDir = new Vector3(ClickableObject.transform.position.x - XDist, ClickableObject.transform.position.y - YDist, TheObjectShowingDirection.transform.position.z);

            TheObjectShowingDirection.transform.position = CombDir;

            AngleOfAim = Mathf.Atan2(-YDist, -XDist) * Mathf.Rad2Deg;
            TheActualArrow.transform.rotation = Quaternion.Euler(0, 0, AngleOfAim);

            MaskScaleX = Vector3.Distance(TheObjectWeWantToMove.transform.position, ClickableObject.transform.position) * 1.35f;
            MaskScaleX = Mathf.Clamp(MaskScaleX, 0, 2.2f);
            TheMask.transform.localScale = new Vector3(MaskScaleX, TheMask.transform.localScale.y);
        }
    }

    private void LateUpdate()
    {
        if (InMotion == true && BallPhysics.velocity.magnitude < 0.005f && !BallMoveScript.FlagHitYet && !BallMoveScript.CurrentlyDead && !PauseGame.pM.MenuIsOpen && !GameStatus.gameStat.ForcePause)
        {
            TurnOnThings();
        }
    }

    //Returns top object in sorting order
    Collider2D GetHighestObject(Collider2D[] results)
    {
        int highestValue = 0;
        Collider2D highestObject = results[0];

        foreach (Collider2D col in results)
        {
            Renderer ren = col.gameObject.GetComponent<Renderer>();
            if (ren && ren.sortingOrder > highestValue)
            {
                highestValue = ren.sortingOrder;
                highestObject = col;
            }
        }
        return highestObject;
    }

    //When the ball stops moving, do this
    private void TurnOnThings()
    {
        InMotion = false;

        ClickableObject.gameObject.SetActive(true);
        TheMask.transform.localScale = new Vector3(0.55f, 0.7f, 0);
        TheObjectShowingDirection.transform.localPosition = new Vector3(0, 0, 0);
        TheObjectWeWantToMove.transform.localPosition = new Vector3(0, 0, 0);
        TheActualArrow.SetActive(true);
    }

    //When the ball starts moving, do this
    public void TurnThingsOff()
    {
        InMotion = true;
        ClickableObject.gameObject.SetActive(false);
        TheActualArrow.SetActive(false);
    }

    //This call is made from respective Controller Managers. Restarts position of ball
    void OnRestartBall()
    {
        if (BallMoveScript.FlagHitYet == false && !BallMoveScript.CurrentlyDead)
        {
            BallMoveScript.transform.localPosition = Vector3.zero;
            BallMoveScript.transform.rotation = new Quaternion(0, 0, 0, 0);
            BallPhysics.velocity = Vector2.zero;
            TheActualArrow.transform.rotation = new Quaternion(0, 0, 0, 0);
            TheMask.transform.localScale = new Vector3(0.65f, 0.7f, 0);
            BallMoveScript.gameObject.layer = 8;

            GameStatus.gameStat.AddGhostData(0, 0, true);
        }
    }
}
