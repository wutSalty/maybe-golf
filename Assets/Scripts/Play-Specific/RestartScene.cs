using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//This might be a relic of the old pause system but is now unceccesary (But I'm too lazy to delete it just in case)
public class RestartScene : MonoBehaviour
{
    public void ResetController(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            DoTheRestart();
        }
    }

    public static void DoTheRestart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
