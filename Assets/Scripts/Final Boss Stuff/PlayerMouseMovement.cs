using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMouseMovement : MonoBehaviour
{

    public SpriteRenderer ObjectBounds;

    Vector2 ScreenBounds;
    Vector2 mousepos;
    float YPos;
    float ObjectHeight;

    private void Start()
    {
        ScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        ObjectHeight = ObjectBounds.bounds.extents.y;
    }

    void Update()
    {
        mousepos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        YPos = Mathf.Clamp(mousepos.y, ScreenBounds.y * -1 + ObjectHeight, ScreenBounds.y - ObjectHeight);
        gameObject.transform.position = new Vector3(transform.position.x, YPos, 0);
    }

    public void DirtyQuit()
    {
        LoadingScreen.loadMan.BeginLoadingScene("MainMenu", false);
    }
}
