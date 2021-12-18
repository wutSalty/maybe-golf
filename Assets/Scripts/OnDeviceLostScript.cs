using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDeviceLostScript : MonoBehaviour
{
    //If the controller has been disconnected due to running out of batteries or other, start the delay
    void OnDeviceLost()
    {
        //Destroy(this.gameObject);
        StartCoroutine(DelayDeviceLost());
    }

    //Once delay is up, delete player game object
    IEnumerator DelayDeviceLost()
    {
        yield return new WaitForSeconds(0.005f);
        Destroy(this.gameObject);
    }

    //If 'Back' is pressed, also delete the game object (but no delay cause this doesn't break things for some reason)
    void OnRemoveController()
    {
        Destroy(this.gameObject);
    }
}
