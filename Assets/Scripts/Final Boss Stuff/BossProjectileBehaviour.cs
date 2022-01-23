using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectileBehaviour : MonoBehaviour
{
    public int DamageGiven = 5;
    public float speed = 15f;
    public float Duration = 1.6f;

    public bool AmLazer = false;

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
        float duration = Duration;

        while (time < duration)
        {
            MyX += -speed * Time.deltaTime;
            transform.position = new Vector3(MyX, MyY, 0);
            time += Time.deltaTime;
            yield return null;
        }

        Destroy(this.gameObject, 0.02f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!AmLazer)
            {
                Destroy(this.gameObject, 0.02f);
            }
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(DamageGiven);
        }
    }
}