using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShooting : MonoBehaviour
{
    public GameObject EnemyBullet;
    public GameObject EnemyRocket;
    public GameObject EnemyLazer;

    public GameObject LazerWarning;

    public float TimeInterval;
    public int AmountPerInterval;

    public bool PhaseA;
    public bool PhaseB;
    public bool PhaseC;
    public bool PhaseD;
    public bool PhaseE;

    private GameObject warning;

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

    public void StartShooting()
    {
        StartCoroutine(ShootProjectile());
        Debug.Log("Shooting startd");
    }

    public void StopShooting()
    {
        StopAllCoroutines();
        if (warning != null)
        {
            Destroy(warning, 0.02f);
        }
        Debug.Log("Shooting stopp");
    }

    IEnumerator ShootProjectile()
    {
        while (true)
        {
            if (BossStatus.bossStat.ForcePause)
            {
                yield return new WaitUntil(() => BossStatus.bossStat.ForcePause == false);
            }

            if (PhaseA) //Phase A Attacks
            {
                yield return new WaitForSeconds(attackPattern[0].TimeBetweenWaves);

                foreach (var item in attackPattern[0].details)
                {
                    if (BossStatus.bossStat.ForcePause)
                    {
                        yield return new WaitUntil(() => BossStatus.bossStat.ForcePause == false);
                    }
                    switch (item.TypeOfProjectile)
                    {
                        case 0:
                            Instantiate(EnemyBullet, transform.position, Quaternion.identity);
                            AudioManager.instance.PlaySound("IG_bulletboss");
                            yield return new WaitForSeconds(item.TimeBetweenShots);
                            break;

                        case 1:
                            Instantiate(EnemyRocket, transform.position, Quaternion.identity);
                            AudioManager.instance.PlaySound("IG_rocketboss");
                            yield return new WaitForSeconds(item.TimeBetweenShots);
                            break;

                        default:
                            break;
                    }
                }
            }
            
            else if (PhaseB) //Phase B attacks
            {
                yield return new WaitForSeconds(attackPattern[1].TimeBetweenWaves);

                foreach (var item in attackPattern[1].details)
                {
                    if (BossStatus.bossStat.ForcePause)
                    {
                        yield return new WaitUntil(() => BossStatus.bossStat.ForcePause == false);
                    }
                    switch (item.TypeOfProjectile)
                    {
                        case 0:
                            Instantiate(EnemyBullet, transform.position, Quaternion.identity);
                            AudioManager.instance.PlaySound("IG_bulletboss");
                            yield return new WaitForSeconds(item.TimeBetweenShots);
                            break;

                        case 1:
                            Instantiate(EnemyRocket, transform.position, Quaternion.identity);
                            AudioManager.instance.PlaySound("IG_rocketboss");
                            yield return new WaitForSeconds(item.TimeBetweenShots);
                            break;

                        default:
                            break;
                    }
                }
            }

            else if (PhaseC) //Phase C attacks
            {
                yield return new WaitForSeconds(attackPattern[2].TimeBetweenWaves);

                foreach (var item in attackPattern[2].details)
                {
                    if (BossStatus.bossStat.ForcePause)
                    {
                        yield return new WaitUntil(() => BossStatus.bossStat.ForcePause == false);
                    }
                    switch (item.TypeOfProjectile)
                    {
                        case 0:
                            Instantiate(EnemyBullet, transform.position, Quaternion.identity);
                            AudioManager.instance.PlaySound("IG_bulletboss");
                            yield return new WaitForSeconds(item.TimeBetweenShots);
                            break;

                        case 1:
                            Instantiate(EnemyRocket, transform.position, Quaternion.identity);
                            AudioManager.instance.PlaySound("IG_rocketboss");
                            yield return new WaitForSeconds(item.TimeBetweenShots);
                            break;

                        case 2:
                            StartCoroutine(PrepareLazer());
                            yield return new WaitForSeconds(item.TimeBetweenShots);
                            break;

                        default:
                            break;
                    }
                }
            }

            else if (PhaseD) //Phase D attacks
            {
                yield return new WaitForSeconds(attackPattern[3].TimeBetweenWaves);

                foreach (var item in attackPattern[3].details)
                {
                    if (BossStatus.bossStat.ForcePause)
                    {
                        yield return new WaitUntil(() => BossStatus.bossStat.ForcePause == false);
                    }
                    switch (item.TypeOfProjectile)
                    {
                        case 0:
                            Instantiate(EnemyBullet, transform.position, Quaternion.identity);
                            AudioManager.instance.PlaySound("IG_bulletboss");
                            yield return new WaitForSeconds(item.TimeBetweenShots);
                            break;

                        case 1:
                            Instantiate(EnemyRocket, transform.position, Quaternion.identity);
                            AudioManager.instance.PlaySound("IG_rocketboss");
                            yield return new WaitForSeconds(item.TimeBetweenShots);
                            break;

                        case 2:
                            StartCoroutine(PrepareLazer());
                            yield return new WaitForSeconds(item.TimeBetweenShots);
                            break;

                        default:
                            break;
                    }
                }
            }

            else if (PhaseE) //Phase E attacks
            {
                yield return new WaitForSeconds(attackPattern[4].TimeBetweenWaves);

                foreach (var item in attackPattern[4].details)
                {
                    if (BossStatus.bossStat.ForcePause)
                    {
                        yield return new WaitUntil(() => BossStatus.bossStat.ForcePause == false);
                    }
                    switch (item.TypeOfProjectile)
                    {
                        case 0:
                            Instantiate(EnemyBullet, transform.position, Quaternion.identity);
                            AudioManager.instance.PlaySound("IG_bulletboss");
                            yield return new WaitForSeconds(item.TimeBetweenShots);
                            break;

                        case 1:
                            Instantiate(EnemyRocket, transform.position, Quaternion.identity);
                            AudioManager.instance.PlaySound("IG_rocketboss");
                            yield return new WaitForSeconds(item.TimeBetweenShots);
                            break;

                        case 2:
                            StartCoroutine(PrepareLazer());
                            yield return new WaitForSeconds(item.TimeBetweenShots);
                            break;

                        default:
                            break;
                    }
                }
            }

            else
            {
                yield return null;
            }
        }
    }

    IEnumerator PrepareLazer()
    {
        Vector3 pos = transform.position;
        //LazerWarning.transform.position = new Vector3(7, pos.y, 0);

        warning = Instantiate(LazerWarning, new Vector3(7, pos.y, 0), Quaternion.identity);
        warning.SetActive(false);

        if (BossStatus.bossStat.ForcePause)
        {
            yield return new WaitUntil(() => BossStatus.bossStat.ForcePause == false);
        }
        AudioManager.instance.PlaySound("IG_warning");
        for (int i = 0; i < 6; i++)
        {
            if (BossStatus.bossStat.ForcePause)
            {
                yield return new WaitUntil(() => BossStatus.bossStat.ForcePause == false);
            }
            if (warning.activeSelf)
            {
                warning.SetActive(false);
            }
            else
            {
                warning.SetActive(true);
            }
            yield return new WaitForSeconds(0.1f);
        }

        if (BossStatus.bossStat.ForcePause)
        {
            yield return new WaitUntil(() => BossStatus.bossStat.ForcePause == false);
        }
        AudioManager.instance.PlaySound("IG_lazer");
        Instantiate(EnemyLazer, new Vector3(25f, pos.y, 0), Quaternion.identity);
        Destroy(warning, 0.02f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>().TakeDamage(20);
        }
    }
}
