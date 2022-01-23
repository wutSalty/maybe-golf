using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerShootProjectile : MonoBehaviour
{
    public Transform GolfBall;

    public GameObject BulletObject;
    public GameObject MissileObject;

    public Transform ProjectileSpawnLocation;

    public Slider BulletImage;
    public Slider MissileImage;

    private PlayerProjectileBehaviour[] ListOfProjectiles;
    private int CurrentProjectile;
    private float WaitTime;
    private bool Holding;

    private void Start()
    {
        StartCoroutine(HoldingMouse());
        OnSwitchToPrimary();
    }

    private void Update()
    {
        if (Holding)
        {
            GolfBall.transform.Rotate(0, 0, 600 * Time.deltaTime);
        } else
        {
            GolfBall.transform.Rotate(0, 0, 200 * Time.deltaTime);
        }

        if ((BossControllerDisconnect.BossControlDC.CurrentlyDC || BossPauseGame.bossPause.MenuIsOpen) && Holding)
        {
            Holding = false;
        }
    }

    void OnFiring()
    {
        Holding = !Holding;
    }

    void OnSwapProjectiles()
    {
        if (CurrentProjectile == 0)
        {
            OnSwitchToSecondary();
        }
        else
        {
            OnSwitchToPrimary();
        }
    }

    void OnSwitchToPrimary()
    {
        BulletImage.gameObject.SetActive(true);
        MissileImage.gameObject.SetActive(false);
        CurrentProjectile = 0;
        WaitTime = 0.25f;
    }

    void OnSwitchToSecondary()
    {
        BulletImage.gameObject.SetActive(false);
        MissileImage.gameObject.SetActive(true);
        CurrentProjectile = 1;
        WaitTime = 1f;
    }

    IEnumerator HoldingMouse()
    {
        while (true) //Always running
        {
            if (Holding) //If player is holding down
            {
                ListOfProjectiles = FindObjectsOfType<PlayerProjectileBehaviour>();

                if (ListOfProjectiles.Length < 6)
                {
                    switch (CurrentProjectile)
                    {
                        case 0:
                            Instantiate(BulletObject, ProjectileSpawnLocation.position, Quaternion.identity);
                            StartCoroutine(WeaponCooldown(BulletImage, WaitTime));
                            yield return new WaitForSeconds(WaitTime);
                            break;

                        case 1:
                            Instantiate(MissileObject, ProjectileSpawnLocation.position, Quaternion.identity);
                            StartCoroutine(WeaponCooldown(MissileImage, WaitTime));
                            yield return new WaitForSeconds(WaitTime);
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
            else if (!Holding)
            {
                yield return null;
            }
        }
    }

    IEnumerator WeaponCooldown(Slider slider, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            slider.value = slider.maxValue - (time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        slider.value = 0f;
    }
}