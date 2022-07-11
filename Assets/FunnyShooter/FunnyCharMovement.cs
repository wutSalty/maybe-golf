using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class FunnyCharMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject arrow;
    public GameObject bullet;
    private Camera mainCam;

    public EventSystem eventSys;
    public GameObject PausePanel;
    public GameObject ResumeButton;

    public float speed;
    public float waitTime = 0.3f;

    public Text deathCounterText;
    public Text moneyCounterText;

    private PlayerInput pInput;
    private Vector2 moveValue;
    private Vector2 aimValue;
    private bool HoldingFire = false;

    private Vector2 LeftMove;
    private bool gamePaused = false;

    private int DeathCount = 0;
    private int MoneyCount = 0;

    public int maxHealth = 75;
    private int currentHealth;
    public Slider healthSlider;

    public Text timerText;
    public float currentTime;

    private void Start()
    {
        mainCam = Camera.main;
        pInput = GetComponent<PlayerInput>();

        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;

        deathCounterText.text = "0";

        StartCoroutine(Shooting());
        StartCoroutine(IncrementTimer());
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

        if (eventSys.firstSelectedGameObject == null)
        {
            eventSys.firstSelectedGameObject = eventSys.currentSelectedGameObject;
        }

        if ((LeftMove != Vector2.zero) && (eventSys.currentSelectedGameObject != null))
        {
            eventSys.firstSelectedGameObject = eventSys.currentSelectedGameObject;
        }

        if ((LeftMove != Vector2.zero) && (eventSys.currentSelectedGameObject == null || eventSys.currentSelectedGameObject.activeSelf))
        {
            eventSys.SetSelectedGameObject(eventSys.firstSelectedGameObject);
        }
    }

    private void FixedUpdate()
    {
        Vector3 tempVect = new Vector3(moveValue.x, moveValue.y, 0);
        tempVect = tempVect.normalized * speed * Time.deltaTime;
        rb.MovePosition(transform.position + tempVect);
    }

    private void OnMoving(InputValue value)
    {
        LeftMove = value.Get<Vector2>();
    }

    private void OnMove(InputValue value)
    {
        moveValue = value.Get<Vector2>();
    }

    private void OnAim(InputValue value)
    {
        aimValue = value.Get<Vector2>();

        if (pInput.currentControlScheme == "KBMouse")
        {
            aimValue.x -= Screen.width / 2;
            aimValue.y -= Screen.height / 2;
        }
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

    private void OnMenu()
    {
        CheckPauseGame();
    }

    public void CheckPauseGame()
    {
        if (gamePaused)
        {
            Time.timeScale = 1;
            gamePaused = false;
            pInput.SwitchCurrentActionMap("Game");
            PausePanel.SetActive(false);
            eventSys.SetSelectedGameObject(null);

            HoldingFire = false;
        } 
        else
        {
            Time.timeScale = 0;
            gamePaused = true;
            pInput.SwitchCurrentActionMap("Menu");
            PausePanel.SetActive(true);
            eventSys.SetSelectedGameObject(ResumeButton);
        }
    }

    public void ReturnToMain()
    {
        AudioManager.instance.PlaySound("UI_beep");
        LoadingScreen.loadMan.LoadingMusic("MainMenu", false, "BGM_title");
    }

    public void AddToDeathCounter(int add)
    {
        DeathCount += add;
        deathCounterText.text = DeathCount.ToString();
    }

    public void AddToCash(int add)
    {
        MoneyCount += add;
        moneyCounterText.text = MoneyCount.ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        healthSlider.value = currentHealth;
    }

    private IEnumerator IncrementTimer()
    {
        while (true)
        {
            currentTime += Time.deltaTime;

            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime - minutes * 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            yield return null;
        }
    }
}
