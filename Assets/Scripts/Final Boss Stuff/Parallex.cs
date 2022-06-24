using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallex : MonoBehaviour
{
    public float speed;

    float pos = 0;
    Renderer rendering;

    private void Start()
    {
        rendering = gameObject.GetComponent<Renderer>();

        StartCoroutine(ScrollThing());
    }

    IEnumerator ScrollThing()
    {
        while (true)
        {
            pos += speed * Time.deltaTime;
            if (pos > 1.0f)
            {
                pos -= 1.0f;
            }
            rendering.material.mainTextureOffset = new Vector2(pos, 0);

            yield return null;
        }
    }
}
