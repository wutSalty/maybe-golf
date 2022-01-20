using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectileBehaviour : MonoBehaviour
{
    public int DamageGiven = 5;
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
        float duration = 1.6f;

        while (time < duration)
        {
            MyX += -15f * Time.deltaTime;
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
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(DamageGiven);
            Destroy(this.gameObject, 0.02f);
        }
    }
}
