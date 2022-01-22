using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int CurrentHealth = 0;
    public int MaxHealth = 50;

    public HealthBar healthBar;

    public SpriteRenderer GolfGear;
    public int VulnCounter = 6;
    public bool IFrames;

    void Start()
    {
        if (tag != "Enemy")
        {
            healthBar.SetMaxHealth(MaxHealth);
            CurrentHealth = MaxHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!IFrames)
        {
            CurrentHealth -= damage;
            healthBar.SetHealth(CurrentHealth);
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
