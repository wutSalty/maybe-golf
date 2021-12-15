using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBall : MonoBehaviour
{
    public GameObject SpawnLocation;
    public GameObject BallDragPrefab;
    public GameObject BallButtonPrefab;

    void Awake()
    {
        GameManager gameMan = GameManager.GM;
        //If entering via 2 player mode, spawn both balls

        //!!!Need to update with new method of profiling users

        if (gameMan.NumPlayers.Count == 1) //If game is entering with only 1 player, grab their preference
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
        else //Or else, spawn a ball for every user with the option they want
        {
            foreach (var item in gameMan.NumPlayers)
            {
                switch (item.ControlType)
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
        }
    }
}
