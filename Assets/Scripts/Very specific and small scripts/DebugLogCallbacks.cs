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
    public GUIStyle style; //The style used to display the log

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
           style = new GUIStyle(GUI.skin.textArea)
           {
               fontSize = 20,
               alignment = TextAnchor.LowerLeft
           }
        );
    }
}
