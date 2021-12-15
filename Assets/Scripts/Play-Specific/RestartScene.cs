using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
