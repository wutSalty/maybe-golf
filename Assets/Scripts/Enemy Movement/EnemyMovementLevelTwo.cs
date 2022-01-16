using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementLevelTwo : MonoBehaviour
{
    public float TimeWaiting;
    public float TimeMoving;

    public Vector3 startPos;
    public Vector3 endPos;

    void Start()
    {
        StartCoroutine(MovementPattern());
    }

    IEnumerator MovementPattern()
    {
        float time;

        while (true)
        {
            time = 0;
            while (time < TimeMoving)
            {
                transform.position = Vector3.Lerp(startPos, endPos, time / TimeMoving);
                time += Time.deltaTime;
                yield return null;
            }
            transform.position = endPos;
            yield return new WaitForSeconds(TimeWaiting);

            time = 0;
            while (time < TimeMoving)
            {
                transform.position = Vector3.Lerp(endPos, startPos, time / TimeMoving);
                time += Time.deltaTime;
                yield return null;
            }
            transform.position = startPos;
            yield return new WaitForSeconds(TimeWaiting);
        }
    }
}
