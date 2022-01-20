using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShooting : MonoBehaviour
{
    public GameObject EnemyProjectile;
    public float TimeInterval;
    public int AmountPerInterval;

    private void Start()
    {
        StartCoroutine(ShootProjectile());
    }

    IEnumerator ShootProjectile()
    {
        while (true)
        {
            yield return new WaitForSeconds(TimeInterval);

            for (int i = 0; i < AmountPerInterval; i++)
            {
                Instantiate(EnemyProjectile, transform.position, Quaternion.identity);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
