using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPacks : MonoBehaviour
{
    public float FlightTime;
    public float FlightSpeed;
    public float RotationSpeed;

    private float MyY;
    private float MyX;
    private float RotY = 0;

    // Start is called before the first frame update
    void Start()
    {
        MyY = transform.position.y;
        MyX = transform.position.x;

        StartCoroutine(ShootForwards());
    }

    private void Update()
    {
        RotY += RotationSpeed * Time.deltaTime;
        if (RotY >= 360)
        {
            RotY -= 360;
        }
        gameObject.transform.Rotate(new Vector3(0, RotY, 0));
    }

    IEnumerator ShootForwards()
    {
        float time = 0;
        float duration = FlightTime;

        while (time < duration)
        {
            MyX += FlightSpeed * Time.deltaTime;
            transform.position = new Vector3(MyX, MyY, -1);
            time += Time.deltaTime;
            yield return null;
        }

        Destroy(this.gameObject, 0.02f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerHealth>().TakeHeal(10, this);
        }
    }
}
