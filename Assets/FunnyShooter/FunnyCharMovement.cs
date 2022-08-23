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

    public GameObject mapPlayer;

    private Camera mainCam;
    public Camera secondCam;

    public SpriteRenderer golfSprite;

    [Header("Properties Text")]
    [SerializeField] private int damage = 1;
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

    private int maxHits = 3;

    public Text ControlsText;

    [Header("Other")]
    public float waitTimeA = 0.2f;
    public float waitTimeB = 0.8f;

    public Vector3[] Vector3ForB;
    public Quaternion[] RotationForB;

    private PlayerInput pInput;
    private Vector2 moveValue;
    private Vector2 aimValue;
    [HideInInspector] public bool HoldingFire = true;

    private PlayerUpgradesScript upgradesScript;
    private Vector3 LastDirection;

    private bool ShopJustClosed;
    private Coroutine LerpingVector;

    public int CurrentWeapon { get; private set; } //0 or 1

    private void Start()
    {
        pInput = GetComponent<PlayerInput>();
        upgradesScript = GetComponent<PlayerUpgradesScript>();
        mainCam = Camera.main;

        golfSprite.sprite = GameManager.GM.BallSkins[GameManager.GM.BallSkin];

        deathCounterText.text = DeathCount.ToString();
        moneyCounterText.text = MoneyCount.ToString();

        StartCoroutine(Shooting());
    }

    private void Update()
    {
        mainCam.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        secondCam.transform.position = new Vector3(transform.position.x, transform.position.y, -10);

        if (aimValue != Vector2.zero && !upgradesScript.shopOpened)
        {
            float aimX = aimValue.x;
            float aimY = aimValue.y;

            float radian = Mathf.Atan2(aimY, aimX);
            float angle = Mathf.Rad2Deg * radian;
            arrow.transform.rotation = Quaternion.Euler(0, 0, angle);

            mapPlayer.transform.rotation = Quaternion.Euler(0, 0, angle);

            Vector3 dire = new Vector3(aimX, aimY, 0);
            dire = dire.normalized * 0.35f;
            arrow.transform.localPosition = dire;
        }
    }

    private void FixedUpdate()
    {
        if (upgradesScript.shopOpened) //If the shop is open, keep the user moving in one direction
        {
            if (LerpingVector != null)
            {
                StopCoroutine(LerpingVector);
                LerpingVector = null;
            }

            rb.MovePosition(transform.position + LastDirection);
            ShopJustClosed = true;
        }
        else
        {
            if (ShopJustClosed && moveValue == Vector2.zero) //If the shop has just closed and no movement detected, start automove
            {
                LerpingVector = StartCoroutine(LerpVectorToZero());
                ShopJustClosed = false;
            }
            else if (LerpingVector == null) //If not lerping then move as normal
            {
                ShopJustClosed = false;

                Vector3 tempVect = new Vector3(moveValue.x, moveValue.y, 0);
                tempVect = tempVect * speed * Time.deltaTime;
                rb.MovePosition(transform.position + tempVect);

                LastDirection = tempVect;
            }
            else if (LerpingVector != null && moveValue != Vector2.zero) //If lerping but movement detected, stop lerp
            {
                ShopJustClosed = false;

                StopCoroutine(LerpingVector);
                LerpingVector = null;
            }
        }
    }

    private IEnumerator LerpVectorToZero()
    {
        float duration = 5f;

        for (float t = 0f; t < duration; t+= Time.deltaTime)
        {
            LastDirection = Vector3.Lerp(LastDirection, Vector3.zero, t / duration);
            rb.MovePosition(transform.position + LastDirection);
            yield return null;
        }
        LastDirection = Vector3.zero;
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

    private void OnWeaponUp()
    {
        if (!upgradesScript.shopOpened)
        {
            CurrentWeapon = 0;
        }
    }

    private void OnWeaponDown()
    {
        if (!upgradesScript.shopOpened)
        {
            CurrentWeapon = 1;
        }
    }

    private IEnumerator Shooting()
    {
        while (true)
        {
            if (HoldingFire)
            {
                switch (CurrentWeapon)
                {
                    case 0:
                        GameObject theBullet = Instantiate(bullet, arrow.transform.position, arrow.transform.rotation);
                        FunnyProjectile funnyProjectile = theBullet.GetComponent<FunnyProjectile>();

                        funnyProjectile.damage = damage;
                        funnyProjectile.critChance = critRate;
                        funnyProjectile.maxHits = maxHits;

                        yield return new WaitForSeconds(waitTimeA);
                        break;

                    case 1:
                        for (int i = 0; i < 4; i++)
                        {
                            Vector3 finalOffset = arrow.transform.rotation * Vector3ForB[i];

                            GameObject otherBullet = Instantiate(bullet, transform.position + finalOffset, RotationForB[i] * arrow.transform.rotation);
                            FunnyProjectile otherProjectiles = otherBullet.GetComponent<FunnyProjectile>();

                            otherProjectiles.damage = damage;
                            otherProjectiles.critChance = critRate;
                            otherProjectiles.maxHits = maxHits;
                        }

                        yield return new WaitForSeconds(waitTimeB);
                        break;

                    default:

                        yield return null;
                        break;
                }
                
                
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

    public void UpdateMaxHits(int newValue)
    {
        maxHits = newValue;
    }

    private void OnControlsChanged(PlayerInput input)
    {
        if (input.currentControlScheme == "KBMouse")
        {
            ControlsText.text = "Move: WASD, Aim: MOUSE, Switch: LMB/RMB, Upgrades: E, Menu: ESC";
        }
        else
        {
            ControlsText.text = "Move: LEFTSTICK, Aim: RIGHTSTICK, Switch: UP/DOWN, Upgrades: LB, Menu: MENU";
        }
    }
}
