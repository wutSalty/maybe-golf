using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class FunnyCharMovement : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public GameObject arrow;
    public GameObject bullet;
    public CircleCollider2D magnetCollider;

    private Camera mainCam;
    public Camera secondCam;

    [SerializeField] private int damage = 1;
    [Header("Properties Text")]
    public Text damageText;

    [SerializeField] private float critRate = 0.1f;
    public Text critText;

    [SerializeField] private float speed;
    public Text speedText;

    public Text rangeText;

    [SerializeField] private int DeathCount = 0;
    public Text deathCounterText;

    [SerializeField] private int MoneyCount = 0;
    public Text moneyCounterText;

    [Header("Other")]
    public float waitTime = 0.3f;

    private PlayerInput pInput;
    private Vector2 moveValue;
    private Vector2 aimValue;
    private bool HoldingFire = false;

    public Text timerText;
    [HideInInspector] public float currentTime;

    private void Start()
    {
        mainCam = Camera.main;
        pInput = GetComponent<PlayerInput>();

        deathCounterText.text = DeathCount.ToString();
        moneyCounterText.text = MoneyCount.ToString();

        StartCoroutine(Shooting());
        StartCoroutine(IncrementTimer());
    }

    private void Update()
    {
        mainCam.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        secondCam.transform.position = new Vector3(transform.position.x, transform.position.y, -10);

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

    public void SetHoldingFireState(bool value)
    {
        HoldingFire = value;
    }

    private IEnumerator Shooting()
    {
        while (true)
        {
            if (HoldingFire)
            {
                GameObject theBullet = Instantiate(bullet, arrow.transform.position, arrow.transform.rotation);
                FunnyProjectile funnyProjectile = theBullet.GetComponent<FunnyProjectile>();

                funnyProjectile.damage = damage;
                funnyProjectile.critChance = critRate;

                yield return new WaitForSeconds(waitTime);
            }
            else
            {
                yield return null;
            }
        }
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

    public int GetCash()
    {
        return MoneyCount;
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

    public void UpdateSpeed(float newSpeed, string newText)
    {
        speed = newSpeed;
        speedText.text = newText;
    }

    public void UpdateDamage(int newDamage, string newText)
    {
        damage = newDamage;
        damageText.text = newText;
    }

    public void UpdateCritRate(float newRate, string newText)
    {
        critRate = newRate;
        critText.text = newText;
    }

    public void UpdateMagnetRange(float newRange, string newText)
    {
        magnetCollider.radius = newRange;
        rangeText.text = newText;
    }
}
