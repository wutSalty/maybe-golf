using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiiin : MonoBehaviour
{
    public int SpinSpeed = 400;

    private void OnEnable()
    {
        gameObject.transform.localRotation = Quaternion.identity;
    }

    private void Update()
    {
        gameObject.transform.Rotate(0, 0, -SpinSpeed * Time.deltaTime);
    }
}
