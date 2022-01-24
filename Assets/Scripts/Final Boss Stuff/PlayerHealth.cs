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
    private Animator animator;

    void Start()
    {
        if (tag != "Enemy")
        {
            pInput = GetComponentInParent<PlayerInput>();
            pIndex = pInput.playerIndex;
            animator = GetComponent<Animator>();

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
                animator.SetTrigger("Death"); //Make death animation
                return;
            }

            IFrames = true;
            StartCoroutine(VulnCountdown());
        }
    }

    public void TakeHeal(int heal, HealthPacks pack)
    {
        if (CurrentHealth + heal <= MaxHealth)
        {
            CurrentHealth += heal;
            healthBar.SetHealth(CurrentHealth);
        }

        else if (CurrentHealth + heal - MaxHealth < 10)
        {
            heal = CurrentHealth + heal - MaxHealth;
            CurrentHealth += heal;
            healthBar.SetHealth(CurrentHealth);
        }
        
        Destroy(pack.gameObject, 0.02f);
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
