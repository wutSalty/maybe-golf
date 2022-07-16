using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FunnyPlayerHealth : MonoBehaviour
{
    public FunnyGameManager funnyMan;
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
        if (currentHealth <= 0)
        {
            return;
        }

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GetComponent<FunnyCharMovement>().HoldingFire = false;
            funnyMan.CheckTime();
        }

        healthSlider.value = currentHealth;
        healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    }
}
