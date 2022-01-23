using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    private bool IsPlayer = false;
    private PlayerInput pInput;
    private int pIndex;

    public int CurrentHealth = 0;
    public int MaxHealth = 50;

    public HealthBar healthBar;

    public SpriteRenderer GolfGear;
    public int VulnCounter = 6;
    public bool IFrames;

    public bool PlayerDead;

    void Start()
    {
        if (tag != "Enemy")
        {
            pInput = GetComponentInParent<PlayerInput>();
            pIndex = pInput.playerIndex;

            healthBar.SetMaxHealth(MaxHealth);
            CurrentHealth = MaxHealth;
            IsPlayer = true;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!IFrames)
        {
            CurrentHealth -= damage;
            healthBar.SetHealth(CurrentHealth);

            if (CurrentHealth <= 0 && IsPlayer)
            {
                BossStatus.bossStat.UpdatePlayerStatus(pIndex);
                PlayerDead = true;
                gameObject.SetActive(false);
                return;
            }

            IFrames = true;
            StartCoroutine(VulnCountdown());
        }
    }

    IEnumerator VulnCountdown()
    {
        for (int i = 0; i < VulnCounter; i++)
        {
            if (GolfGear.color.a == 0)
            {
                GolfGear.color = new Color(1, 1, 1, 1);
            }
            else
            {
                GolfGear.color = new Color(1, 1, 1, 0);
            }
            yield return new WaitForSeconds(0.5f);
        }
        IFrames = false;
    }
}
