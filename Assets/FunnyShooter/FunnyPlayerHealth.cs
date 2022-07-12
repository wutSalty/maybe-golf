using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FunnyPlayerHealth : MonoBehaviour
{
    [HideInInspector] public int maxHealth = 50;
    private int currentHealth;
    public Slider healthSlider;

    public Text healthText;

    private void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    }

    public void UpgradeHealth(int NewHealth)
    {
        int healthDiff = NewHealth - maxHealth; //calculate difference
        maxHealth = NewHealth; //Set to max health
        currentHealth += healthDiff; //Add difference to current

        healthSlider.maxValue = maxHealth; //Apply to slider
        healthSlider.value = currentHealth;
        healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        healthSlider.value = currentHealth;
        healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            TakeDamage(2);
        }
    }
}
