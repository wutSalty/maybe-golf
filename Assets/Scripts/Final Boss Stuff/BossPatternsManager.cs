using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPatternsManager : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public PlayerHealth playerHealth;
    public BossShooting bossShoot;

    public Sprite spriteB;

    public void SwitchSpriteB()
    {
        spriteRenderer.sprite = spriteB;
    }
}
