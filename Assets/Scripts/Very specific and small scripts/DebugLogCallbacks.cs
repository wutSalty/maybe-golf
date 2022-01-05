using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DebugLogCallbacks : MonoBehaviour
{
    public int maxLines = 8; //Max lines that debug should show
    private Queue<string> queue = new Queue<string>(); //Allows to hold items in an order
    private string currentText = "";
    private string playerPrefsTxt = "";
    private GUIStyle styleA; //The style used to display the log
    private GUIStyle styleB;

    private void Start()
    {
        UpdatePlayPrefsText();
    }

    public void UpdatePlayPrefsText()
    {
        playerPrefsTxt = "WindowMode: " + PlayerPrefs.GetInt("WindowMode") + "\nWindowSize: " + PlayerPrefs.GetInt("WindowSize", 0) + "\nInputType: " + PlayerPrefs.GetInt("InputType", 0) + "\nSensitivity: " + PlayerPrefs.GetFloat("Sensitivity", 4) + "\nDebugWindow: " + PlayerPrefs.GetInt("DebugWindow", 0);
    }

    //When the script is enabled, subscribe to the debug log. This will make HandleLog occur whenever something happens
    void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    //When the script is disabled, unsubscribed to the debug log. This prevents weird things from happening
    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Delete oldest message if there's already too many
        if (queue.Count >= maxLines) queue.Dequeue();

        //Add the new log into the queue (after it's checked the length isn't too long)
        queue.Enqueue(Truncate(logString, 50));

        //Attaches each string from the queue together
        var builder = new StringBuilder();
        foreach (string st in queue)
        {
            builder.Append("\n").Append(st);
        }

        //Updates the text in the window log
        currentText = builder.ToString();
    }

    //If the string is too long, cut it down
    public string Truncate(string value, int maxChars)
    {
        if (value.Length <= maxChars)
        {
            return value;
        } else
        {
            return value.Substring(0, maxChars) + "...";
        }
    }

    //Draws the debug window in the bottom left corner and scales according to window size
    void OnGUI()
    {
        GUI.Label(
           new Rect(
               5,                   // x, left offset
               Screen.height - (Screen.height * 270f / 1080) - 5, // y, bottom offset
               Screen.width * 600f / 1920,                // width
               Screen.height * 270f / 1080                 // height
           ),
           currentText,             // the display text
           styleA = new GUIStyle(GUI.skin.textArea)
           {
               fontSize = 20,
               alignment = TextAnchor.LowerLeft
           }
        );

        GUI.Label(
            new Rect(
                Screen.width - (Screen.width * 390f / 1920) - 5,
                5,
                Screen.width * 390f / 1920,
                Screen.height * 220f / 1080
                ),
            playerPrefsTxt,
            styleB = new GUIStyle(GUI.skin.textArea)
            {
                fontSize = 20,
            }
            );
    }
}
