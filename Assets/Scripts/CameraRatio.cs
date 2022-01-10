using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Adjusts the size of the camera so the entire map can be viewed
public class CameraRatio : MonoBehaviour
{
    public float SixteenByNine = 7f;
    public float FourByThree = 5f;

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
            FixCamera(SixteenByNine);
        }
        else if ((Screen.width * 1.0) / Screen.height == (16 * 1.0) / 9)
        {
            FixCamera(FourByThree);
        }
        Debug.Log("OnSceneLoaded " + (Screen.width * 1.0) / Screen.height);
    }

    public void FixCamera(float aspect)
    {
        cam.orthographicSize = aspect;
    }
}
