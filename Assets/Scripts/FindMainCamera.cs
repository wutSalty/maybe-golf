using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FindMainCamera : MonoBehaviour
{
    public Canvas[] canvas;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in canvas)
        {
            if (item.renderMode != RenderMode.ScreenSpaceCamera)
            {
                item.renderMode = RenderMode.ScreenSpaceCamera;
            }
            
            item.worldCamera = Camera.main;
            item.planeDistance = 1;
        }
        SceneManager.activeSceneChanged += sceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= sceneChanged;
    }

    private void sceneChanged(Scene current, Scene next)
    {
        foreach (var item in canvas)
        {
            item.worldCamera = Camera.main;
            item.planeDistance = 1;
        }
    }
}
