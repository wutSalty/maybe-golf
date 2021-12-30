using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagWin : MonoBehaviour
{
    //If something enters the flag, check it's tag, and if it's player (which is the ball), do stuff
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("You win");

            collision.gameObject.GetComponent<MoveBall>().BallHasWon();

            //StartCoroutine(MoveToPosition(TheBall.transform, EndPosition, 0.5f)); //!!Testing
        }      
    }

    //Smoothly move the ball to the centre of the hole !!Testing
    //public IEnumerator MoveToPosition(Transform transform, Vector3 position, float timetomove)
    //{
    //    var currentPos = transform.position;
    //    var t = 0f;
    //    while (t < 1)
    //    {
    //        t += Time.deltaTime / timetomove;
    //        transform.position = Vector3.Lerp(currentPos, position, t);
    //        TheBall.transform.Rotate(0, 0, 400 * Time.deltaTime);
    //        yield return null;
    //    }
    //    if (t >= 1)
    //    {
    //        TheBallPhysics.velocity = new Vector2(0, 0);
    //        yield return null;
    //    }
    //}
}
//TODO
//Tweak smoothing velocity once hit goal