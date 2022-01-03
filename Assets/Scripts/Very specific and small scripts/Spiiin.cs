using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiiin : MonoBehaviour
{
    private void Update()
    {
        gameObject.transform.Rotate(0, 0, -400 * Time.deltaTime);
    }
}
