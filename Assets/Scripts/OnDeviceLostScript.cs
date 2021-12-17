using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OnDeviceLostScript : MonoBehaviour
{
    void OnDeviceLost()
    {
        Destroy(this.gameObject);
    }
}
