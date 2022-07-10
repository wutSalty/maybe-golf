using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunnyProjectile : MonoBehaviour
{
    public float speed;
    public float aliveTime = 10;
    public int damage = 1;

    private float time;

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
            collision.GetComponent<EnemyHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}