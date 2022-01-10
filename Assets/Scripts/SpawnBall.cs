using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Script to spawn the ball and any multiplayer players with skins
public class SpawnBall : MonoBehaviour
{
    public GameObject SpawnLocation; //Location to spawn the ball (uses empty gameobject)
    public GameObject BallDragPrefab; //The "template" for mouse input
    public GameObject BallButtonPrefab; //The "template" for button input

    private PlayerInput CurrentBall;

    //Numbers required to set sprite mask layers
    public int[] DefaultSpriteMasks;

    //Different sprites for the balls in multiplayer
    public Sprite[] MultiSprites;

    void Awake()
    {
        GameManager gameMan = GameManager.GM;

        if (gameMan.SingleMode == true || gameMan.NumPlayers.Count == 1) //If game is entering with only 1 player, grab their preference
        {
            var InputType = PlayerPrefs.GetInt("InputType", 0);
            var SkinType = PlayerPrefs.GetInt("BallSkin", 0);
            GameObject ABall;

            switch (InputType)
            {
                case 0: //Mouse and drag
                    ABall = Instantiate(BallDragPrefab, SpawnLocation.transform.position, Quaternion.identity);
                    var BallControllerA = ABall.GetComponent<DragAndAimControllerManager>();
                    BallControllerA.BallSprite.sprite = gameMan.BallSkins[SkinType];
                    break;

                case 1: //Button and controllers
                    ABall = Instantiate(BallButtonPrefab, SpawnLocation.transform.position, Quaternion.identity);
                    var BallControllerB = ABall.GetComponent<AltAimControllerManager>();
                    BallControllerB.BallSprite.sprite = gameMan.BallSkins[SkinType];
                    break;

                default:
                    break;
            }
        }
        else //Or else, spawn a ball for every valid user
        {
            foreach (var item in gameMan.NumPlayers) //For every record registered in GameManager
            {
                if (item.PlayerIndex != 99) //If their index is not 99 (currently disconnected)
                {
                    switch (item.ControlType) //Depending on their control type, spawn in different ball. And initialise properties
                    {
                        case 0: //Mouse and drag
                            CurrentBall = PlayerInput.Instantiate(BallDragPrefab, item.PlayerIndex, null, -1, item.inputDevice);

                            var ControllerManagerA = CurrentBall.GetComponent<DragAndAimControllerManager>();

                            ControllerManagerA.BallSprite.sprite = MultiSprites[item.PlayerIndex];
                            ControllerManagerA.gameObject.transform.position = SpawnLocation.transform.position;
                            ControllerManagerA.spriteMask.frontSortingOrder = DefaultSpriteMasks[item.PlayerIndex] + 5;
                            ControllerManagerA.spriteMask.backSortingOrder = DefaultSpriteMasks[item.PlayerIndex] - 5;
                            ControllerManagerA.insideSprite.sortingOrder = DefaultSpriteMasks[item.PlayerIndex];
                            break;

                        case 1: //Keyboard
                            CurrentBall = PlayerInput.Instantiate(BallButtonPrefab, item.PlayerIndex, "Keyboard", -1, item.inputDevice);
                            CurrentBall.neverAutoSwitchControlSchemes = true;

                            var ControllerManagerB = CurrentBall.GetComponent<AltAimControllerManager>();

                            ControllerManagerB.BallSprite.sprite = MultiSprites[item.PlayerIndex];
                            ControllerManagerB.gameObject.transform.position = SpawnLocation.transform.position;
                            ControllerManagerB.spriteMask.frontSortingOrder = DefaultSpriteMasks[item.PlayerIndex] + 5;
                            ControllerManagerB.spriteMask.backSortingOrder = DefaultSpriteMasks[item.PlayerIndex] - 5;
                            ControllerManagerB.insideSprite.sortingOrder = DefaultSpriteMasks[item.PlayerIndex];
                            break;

                        case 2: //Controller
                            CurrentBall = PlayerInput.Instantiate(BallButtonPrefab, item.PlayerIndex, "Controller", -1, item.inputDevice);
                            CurrentBall.neverAutoSwitchControlSchemes = true;

                            var ControllerManagerC = CurrentBall.GetComponent<AltAimControllerManager>();

                            ControllerManagerC.BallSprite.sprite = MultiSprites[item.PlayerIndex];
                            ControllerManagerC.gameObject.transform.position = SpawnLocation.transform.position;
                            ControllerManagerC.spriteMask.frontSortingOrder = DefaultSpriteMasks[item.PlayerIndex] + 5;
                            ControllerManagerC.spriteMask.backSortingOrder = DefaultSpriteMasks[item.PlayerIndex] - 5;
                            ControllerManagerC.insideSprite.sortingOrder = DefaultSpriteMasks[item.PlayerIndex];
                            break;

                        default:
                            break;
                    };
                }
            }
        }
    }
}
