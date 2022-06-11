using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileBehaviour : MonoBehaviour
{
    public GameObject HealthDropPrefabs;

    public int DamageGiven = 1;
    public float FlightSpeed = 15f;
    public float FlightTime = 1.4f;
    public bool BeatEnemyProjectileA;

    private float MyY;
    private float MyX;

    // Start is called before the first frame update
    void Start()
    {
        MyY = transform.position.y;
        MyX = transform.position.x;

        StartCoroutine(ShootForwards());
    }

    IEnumerator ShootForwards()
    {
        float time = 0;
        float duration = FlightTime;

        while (time < duration)
        {
            if (BossStatus.bossStat.ForcePause)
            {
                yield return new WaitUntil(() => BossStatus.bossStat.ForcePause == false);
            }

            MyX += FlightSpeed * Time.deltaTime;
            transform.position = new Vector3(MyX, MyY, 0);
            time += Time.deltaTime;
            yield return null;
        }

        Destroy(this.gameObject, 0.02f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyProjectileA")) //Bullets
        {
            Destroy(collision.gameObject, 0.02f);

            if (!BeatEnemyProjectileA)
            {
                Destroy(this.gameObject, 0.02f);
            }
        }

        if (collision.CompareTag("EnemyProjectileB")) //Missiles
        {
            if (BeatEnemyProjectileA)
            {
                Instantiate(HealthDropPrefabs, new Vector3(transform.position.x, transform.position.y, -1), Quaternion.identity);
                Destroy(collision.gameObject, 0.02f);
            }

            Destroy(this.gameObject, 0.02f);
        }

        if (collision.CompareTag("EnemyProjectileC")) //Lazers
        {
            Destroy(this.gameObject, 0.02f);
        }

        if (collision.CompareTag("Enemy")) //The boss itself
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(DamageGiven);
            Destroy(this.gameObject, 0.02f);
        }
    }
}
