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
        //If entering via 2 player mode, spawn both balls
        if (GameManager.GM.TwoPlayerMode == true)
        {
            Instantiate(BallDragPrefab, SpawnLocation.transform.position, Quaternion.identity);
            Instantiate(BallButtonPrefab, SpawnLocation.transform.position, Quaternion.identity);

        } else //Or else, grab single player preference and spawn in the corresponding ball
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
    }
}
