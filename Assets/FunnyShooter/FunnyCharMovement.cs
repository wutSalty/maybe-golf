using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FunnyCharMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject arrow;
    public GameObject bullet;
    public Camera mainCam;

    public float speed;
    public float waitTime = 0.3f;

    private Vector2 moveValue;
    private Vector2 aimValue;
    private bool HoldingFire = false;

    private void Start()
    {
        mainCam = Camera.main;
        StartCoroutine(Shooting());
    }

    private void Update()
    {
        mainCam.transform.position = new Vector3(transform.position.x, transform.position.y, -10);

        if (aimValue != Vector2.zero)
        {
            float aimX = aimValue.x;
            float aimY = aimValue.y;

            float radian = Mathf.Atan2(aimY, aimX);
            float angle = Mathf.Rad2Deg * radian;
            arrow.transform.rotation = Quaternion.Euler(0, 0, angle);

            Vector3 dire = new Vector3(aimX, aimY, 0);
            dire = dire.normalized * 0.35f;
            arrow.transform.localPosition = dire;
        }
    }

    private void FixedUpdate()
    {
        Vector3 tempVect = new Vector3(moveValue.x, moveValue.y, 0);
        tempVect = tempVect.normalized * speed * Time.deltaTime;
        rb.MovePosition(transform.position + tempVect);

        
    }

    private void OnMove(InputValue value)
    {
        moveValue = value.Get<Vector2>();
    }

    private void OnAim(InputValue value)
    {
        aimValue = value.Get<Vector2>();
    }

    private void OnShoot()
    {
        HoldingFire = !HoldingFire;
        
    }

    private IEnumerator Shooting()
    {
        while (true)
        {
            if (HoldingFire)
            {
                Instantiate(bullet, arrow.transform.position, arrow.transform.rotation);
                yield return new WaitForSeconds(waitTime);
            }
            else
            {
                yield return null;
            }
        }
    }

    public void ReturnToMain()
    {
        AudioManager.instance.PlaySound("UI_beep");
        LoadingScreen.loadMan.LoadingMusic("MainMenu", false, "BGM_title");
    }
}
