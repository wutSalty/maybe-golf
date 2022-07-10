using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int enemyHealth = 50;
    private int internalHealth;

    [SerializeField] private Slider hpBar;

    private void Start()
    {
        internalHealth = enemyHealth;
        hpBar.value = enemyHealth;
    }

    public void TakeDamage(int damage)
    {
        internalHealth -= damage;
        hpBar.value = internalHealth;

        if (internalHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
