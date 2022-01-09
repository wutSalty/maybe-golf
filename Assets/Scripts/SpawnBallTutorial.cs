using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to handle spawning balls during the tutorial as they use a different prefab for dialogue
public class SpawnBallTutorial : MonoBehaviour
{
    public GameObject SpawnLocation; //Location to spawn the ball (uses empty gameobject)
    public GameObject TutorialBallDragPrefab; //The "template" for mouse input
    public GameObject TutorialBallButtonPrefab; //The "template" for button input

    private void Awake()
    {
        var InputType = PlayerPrefs.GetInt("InputType", 0);
        var SkinType = PlayerPrefs.GetInt("BallSkin", 0);
        GameObject ABall;

        switch (InputType)
        {
            case 0:
                ABall = Instantiate(TutorialBallDragPrefab, SpawnLocation.transform.position, Quaternion.identity);
                var BallControllerA = ABall.GetComponent<DragAndAimControllerManager>();
                BallControllerA.BallSprite.sprite = GameManager.GM.BallSkins[SkinType];
                break;

            case 1:
                ABall = Instantiate(TutorialBallButtonPrefab, SpawnLocation.transform.position, Quaternion.identity);
                var BallControllerB = ABall.GetComponent<AltAimControllerManager>();
                BallControllerB.BallSprite.sprite = GameManager.GM.BallSkins[SkinType];
                break;

            default:
                break;
        }
    }
}
