using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraRatio : MonoBehaviour
{
    [HideInInspector]
    public Camera cam;

    //This happens first, sets camera component to cam
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    //Once the scene is loaded, subscribe to the event
    private void OnEnable()
    {
        cam = GetComponent<Camera>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //When this event is called, check the aspect ratio of the screen and make sure the camera is scalled correctly
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        cam = GetComponent<Camera>();
        if ((Screen.width * 1.0) / Screen.height == (4 * 1.0) / 3)
        {
            FixCamera(7f);
        }
        else if ((Screen.width * 1.0) / Screen.height == (16 * 1.0) / 9)
        {
            FixCamera(5f);
        }
        Debug.Log("OnSceneLoaded " + (Screen.width * 1.0) / Screen.height);
    }

    public void FixCamera(float aspect)
    {
        cam.orthographicSize = aspect;
    }
}
