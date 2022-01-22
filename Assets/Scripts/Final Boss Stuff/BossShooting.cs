using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShooting : MonoBehaviour
{
    public GameObject EnemyBullet;
    public GameObject EnemyRocket;
    public float TimeInterval;
    public int AmountPerInterval;

    public bool PhaseA;
    public bool PhaseB;
    public bool PhaseC;

    //Figure this out
    [System.Serializable]
    public class PhaseDetails
    {
        public int TypeOfProjectile;
        public float TimeBetweenShots;
    }

    [System.Serializable]
    public class AttackPattern
    {
        public PhaseDetails[] details;
        public float TimeBetweenWaves;
    }

    public AttackPattern[] attackPattern;

    private void Start()
    {
        StartCoroutine(ShootProjectile());
    }

    IEnumerator ShootProjectile()
    {
        while (true)
        {
            yield return new WaitForSeconds(TimeInterval);

            if (PhaseA) //Phase A Attacks
            {
                foreach (var item in attackPattern)
                {

                }


                for (int i = 0; i < AmountPerInterval; i++)
                {
                    Instantiate(EnemyBullet, transform.position, Quaternion.identity);
                    yield return new WaitForSeconds(0.5f);
                }
            }
            
            if (PhaseB)
            {
                for (int i = 0; i < AmountPerInterval; i++)
                {
                    Instantiate(EnemyRocket, transform.position, Quaternion.identity);
                    yield return new WaitForSeconds(0.5f);
                }
            }
            
        }
    }
}
