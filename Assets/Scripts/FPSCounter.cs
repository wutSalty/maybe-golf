using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public int avgFrameRate;
    private GUIStyle styleA;

    private void Awake()
    {
        StartCoroutine(getFPS());
    }

    private IEnumerator getFPS()
    {
        while (true)
        {
            float current = 0;
            current = (int)(1f / Time.unscaledDeltaTime);
            avgFrameRate = (int)current;

            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    private void OnGUI()
    {
        GUI.Label(
            new Rect (
                10,                              // x, left offset
                10,                              // y, bottom offset
                Screen.width * 50f / 1920,      // width
                Screen.height * 30f / 1080      // height
                ),
            avgFrameRate.ToString(),
            styleA = new GUIStyle(GUI.skin.textArea)
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter
            }
        );
    }
}
