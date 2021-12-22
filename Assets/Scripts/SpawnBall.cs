using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnBall : MonoBehaviour
{
    public GameObject SpawnLocation;
    public GameObject BallDragPrefab;
    public GameObject BallButtonPrefab;

    private PlayerInput CurrentBall;

    public int[] DefaultSpriteMasks;

    public Sprite[] MultiSprites;

    public GameObject[] BallButtonPrefabs;

    void Awake()
    {
        GameManager gameMan = GameManager.GM;

        if (gameMan.SingleMode == true || gameMan.NumPlayers.Count == 1) //If game is entering with only 1 player, grab their preference
        {
            var InputType = PlayerPrefs.GetInt("InputType", 0);

            switch (InputType)
            {
                case 0:
                    Instantiate(BallDragPrefab, SpawnLocation.transform.position, Quaternion.identity);
                    break;

                case 1:
                    Instantiate(BallButtonPrefab, SpawnLocation.transform.position, Quaternion.identity);
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
                        case 0:
                            CurrentBall = PlayerInput.Instantiate(BallDragPrefab, item.PlayerIndex, null, -1, item.inputDevice);

                            var ControllerManagerA = CurrentBall.GetComponent<DragAndAimControllerManager>();

                            ControllerManagerA.BallSprite.sprite = MultiSprites[item.PlayerIndex];
                            ControllerManagerA.gameObject.transform.position = SpawnLocation.transform.position;
                            ControllerManagerA.spriteMask.frontSortingOrder = DefaultSpriteMasks[item.PlayerIndex] + 5;
                            ControllerManagerA.spriteMask.backSortingOrder = DefaultSpriteMasks[item.PlayerIndex] - 5;
                            ControllerManagerA.insideSprite.sortingOrder = DefaultSpriteMasks[item.PlayerIndex];
                            break;

                        case 1:
                            CurrentBall = PlayerInput.Instantiate(BallButtonPrefabs[item.PlayerIndex], item.PlayerIndex, "Keyboard", -1, item.inputDevice);

                            var ControllerManagerB = CurrentBall.GetComponent<AltAimControllerManager>();

                            ControllerManagerB.BallSprite.sprite = MultiSprites[item.PlayerIndex];
                            ControllerManagerB.gameObject.transform.position = SpawnLocation.transform.position;
                            ControllerManagerB.spriteMask.frontSortingOrder = DefaultSpriteMasks[item.PlayerIndex] + 5;
                            ControllerManagerB.spriteMask.backSortingOrder = DefaultSpriteMasks[item.PlayerIndex] - 5;
                            ControllerManagerB.insideSprite.sortingOrder = DefaultSpriteMasks[item.PlayerIndex];
                            break;

                        case 2:
                            CurrentBall = PlayerInput.Instantiate(BallButtonPrefabs[item.PlayerIndex], item.PlayerIndex, "Controller", -1, item.inputDevice);

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
