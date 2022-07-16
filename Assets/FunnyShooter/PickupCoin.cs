using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupCoin : MonoBehaviour
{
    private TargetJoint2D joint;
    public float MaxForce = 80;
    public float MaxFreq = 10;
    public float Dampening = 0.2f;

    private void Update()
    {
        if (joint == null)
        {
            return;
        }

        joint.target = new Vector2 (Camera.main.transform.position.x, Camera.main.transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<FunnyCharMovement>().AddToCash(1);
            Destroy(gameObject);
        }

        if (collision.CompareTag("Magnet") && joint == null)
        {
            joint = gameObject.AddComponent<TargetJoint2D>();
            joint.maxForce = MaxForce;
            joint.frequency = MaxFreq;
            joint.dampingRatio = Dampening;
        }
    }
}
