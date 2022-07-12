using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunnyProjectile : MonoBehaviour
{
    public float speed;
    public float aliveTime = 10;
    [HideInInspector] public int damage = 1;
    public int maxHits = 3;
    [HideInInspector] public float critChance = 0.1f;

    private float time;
    private int noHits = 0;

    private void Start()
    {
        StartCoroutine(Flying());
    }

    private IEnumerator Flying()
    {
        while (time < aliveTime)
        {
            transform.position += transform.right * speed * Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject, 0.1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            int amount = 0;
            bool crit = false;
            DamageCritChance(out amount, out crit);

            collision.GetComponent<EnemyHealth>().TakeDamage(amount, crit);
            noHits += 1;
            if (noHits >= 3)
            {
                Destroy(gameObject);
            }
        }
    }

    private void DamageCritChance(out int amount, out bool crit)
    {
        amount = damage + Random.Range(-2, 2);

        if (Random.value <= critChance)
        {
            amount *= 2;
            crit = true;
        }
        else
        {
            crit = false;
        }
    }
}
