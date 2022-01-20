using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShootProjectile : MonoBehaviour
{
    public Transform GolfBall;
    public GameObject ProjectilePrefab;
    public Transform ProjectileSpawnLocation;

    private PlayerProjectileBehaviour[] ListOfProjectiles;
    private bool Holding;

    private void Start()
    {
        StartCoroutine(HoldingMouse());
    }

    private void Update()
    {
        if (Holding)
        {
            GolfBall.transform.Rotate(0, 0, 600 * Time.deltaTime);
        } else
        {
            GolfBall.transform.Rotate(0, 0, 200 * Time.deltaTime);
        }
    }

    void OnFiring()
    {
        Holding = !Holding;
    }

    IEnumerator HoldingMouse()
    {
        while (true)
        {
            if (Holding)
            {
                ListOfProjectiles = FindObjectsOfType<PlayerProjectileBehaviour>();

                if (ListOfProjectiles.Length < 6)
                {
                    Instantiate(ProjectilePrefab, ProjectileSpawnLocation.position, Quaternion.identity);
                }

                yield return new WaitForSeconds(0.25f);
            }
            else if (!Holding)
            {
                yield return null;
            }
        }
    }
}
