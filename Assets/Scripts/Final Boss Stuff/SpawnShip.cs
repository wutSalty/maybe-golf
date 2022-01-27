using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnShip : MonoBehaviour
{
    public GameObject MousePlayerPrefab;
    public GameObject ButtonPlayerPrefab;

    public Transform[] SpawnLocs;
    public Sprite[] MultiSprites;
    public Sprite[] ShipSprite;

    GameObject Player;
    PlayerInput player;
    PlayerShootProjectile playerShootProjectile;

    private void Awake()
    {
        GameManager gameMan = GameManager.GM;

        if (gameMan.SingleMode || gameMan.NumPlayers.Count == 1)
        {
            int InputType = PlayerPrefs.GetInt("InputType", 0);
            int SkinType = gameMan.BallSkin;

            switch (InputType)
            {
                case 0:
                    Player = Instantiate(MousePlayerPrefab, SpawnLocs[1].position, Quaternion.identity);                  
                    break;

                case 1:
                    Player = Instantiate(ButtonPlayerPrefab, SpawnLocs[1].position, Quaternion.identity);     
                    break;

                default:
                    break;
            }

            playerShootProjectile = Player.GetComponentInChildren<PlayerShootProjectile>();
            playerShootProjectile.GolfBall.gameObject.GetComponent<SpriteRenderer>().sprite = gameMan.BallSkins[SkinType];
        }
        else
        {
            foreach (var item in gameMan.NumPlayers)
            {
                if (item.PlayerIndex != 99)
                {
                    switch (item.ControlType)
                    {
                        case 0:
                            player = PlayerInput.Instantiate(MousePlayerPrefab, item.PlayerIndex, "Mouse", -1, item.inputDevice);

                            player.transform.position = SpawnLocs[item.PlayerIndex].position;
                            playerShootProjectile = player.gameObject.GetComponentInChildren<PlayerShootProjectile>();
                            playerShootProjectile.GolfBall.gameObject.GetComponent<SpriteRenderer>().sprite = MultiSprites[item.PlayerIndex];
                            player.GetComponentInChildren<PlayerHealth>().GolfGear.GetComponent<SpriteRenderer>().sprite = ShipSprite[item.PlayerIndex];
                            break;

                        case 1:
                            player = PlayerInput.Instantiate(ButtonPlayerPrefab, item.PlayerIndex, "Keyboard", -1, item.inputDevice);

                            player.transform.position = SpawnLocs[item.PlayerIndex].position;
                            playerShootProjectile = player.gameObject.GetComponentInChildren<PlayerShootProjectile>();
                            playerShootProjectile.GolfBall.gameObject.GetComponent<SpriteRenderer>().sprite = MultiSprites[item.PlayerIndex];
                            player.GetComponentInChildren<PlayerHealth>().GolfGear.GetComponent<SpriteRenderer>().sprite = ShipSprite[item.PlayerIndex];
                            break;

                        case 2:
                            player = PlayerInput.Instantiate(ButtonPlayerPrefab, item.PlayerIndex, "Controller", -1, item.inputDevice);

                            player.transform.position = SpawnLocs[item.PlayerIndex].position;
                            playerShootProjectile = player.gameObject.GetComponentInChildren<PlayerShootProjectile>();
                            playerShootProjectile.GolfBall.gameObject.GetComponent<SpriteRenderer>().sprite = MultiSprites[item.PlayerIndex];
                            player.GetComponentInChildren<PlayerHealth>().GolfGear.GetComponent<SpriteRenderer>().sprite = ShipSprite[item.PlayerIndex];
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }
}
