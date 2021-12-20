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
                    switch (item.ControlType) //Depending on their control type, spawn in different ball
                    {
                        case 0:
                            CurrentBall = PlayerInput.Instantiate(BallDragPrefab, item.PlayerIndex, null, -1, item.inputDevice);
                            break;

                        case 1:
                            CurrentBall = PlayerInput.Instantiate(BallButtonPrefab, item.PlayerIndex, "Keyboard", -1, item.inputDevice);
                            break;

                        case 2:
                            CurrentBall = PlayerInput.Instantiate(BallButtonPrefab, item.PlayerIndex, "Controller", -1, item.inputDevice);
                            break;

                        default:
                            break;
                    };

                    //Assigns different sprite, and sprite layer for each player
                    CurrentBall.gameObject.GetComponent<SpriteRenderer>().sprite = MultiSprites[item.PlayerIndex];

                    CurrentBall.gameObject.transform.position = SpawnLocation.transform.position;
                    CurrentBall.gameObject.GetComponent<MoveBall>().MaskSprite.GetComponent<SpriteMask>().frontSortingOrder = DefaultSpriteMasks[item.PlayerIndex] + 5;
                    CurrentBall.gameObject.GetComponent<MoveBall>().MaskSprite.GetComponent<SpriteMask>().backSortingOrder = DefaultSpriteMasks[item.PlayerIndex] - 5;
                    CurrentBall.gameObject.GetComponent<MoveBall>().InsideSprite.GetComponent<SpriteRenderer>().sortingOrder = DefaultSpriteMasks[item.PlayerIndex];
                }
            }
        }
    }
}
