using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPatternsManager : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public PlayerHealth playerHealth;
    public BossShooting bossShoot;

    public GameObject LazerWarning;
    public float[] Locations;
    public int index = 0;

    public Sprite spriteB;
    public Sprite spriteC;
    public Sprite spriteD;

    public ParticleSystem smokeParticle;

    //private bool EnableAnim = false;

    //private void Update()
    //{
    //    if (BossStatus.bossStat.ForcePause || BossPauseGame.bossPause.MenuIsOpen)
    //    {
    //        animator.speed = 0;
    //        EnableAnim = true;
    //    }

    //    if((!BossStatus.bossStat.ForcePause && !BossPauseGame.bossPause.MenuIsOpen) && EnableAnim)
    //    {
    //        animator.speed = 1;
    //        EnableAnim = false;
    //    }
    //}

    //Phases and percentage of HP remaining
    //1.0 -> PhaseA -> 0.85 -> PhaseB -> 0.60 -> PhaseC -> 0.30 -> PhaseD -> 0.15 -> PhaseE -> 0

    public void SwitchSpriteB()
    {
        spriteRenderer.sprite = spriteB;
        bossShoot.PhaseA = false;
        bossShoot.PhaseB = true;
    }

    public void SwitchSpriteC()
    {
        spriteRenderer.sprite = spriteC;
        bossShoot.PhaseB = false;
        bossShoot.PhaseC = true;
    }

    public void SwitchSpriteD()
    {
        spriteRenderer.sprite = spriteD;
        bossShoot.PhaseC = false;
        bossShoot.PhaseD = true;
    }

    public void ToggleForceInvincibility()
    {
        playerHealth.IFrames = !playerHealth.IFrames;
    }

    public void PauseAttacks()
    {
        bossShoot.StopShooting();
    }

    public void BeginAttacks()
    {
        bossShoot.StartShooting();
    }

    public void FinalTransitionFlashes()
    {
        StopAllCoroutines();
        StartCoroutine(StartFlashing());
    }

    IEnumerator StartFlashing()
    {
        GameObject warning = Instantiate(LazerWarning, new Vector3(7, Locations[index], 0), Quaternion.identity);
        warning.SetActive(false);

        AudioManager.instance.PlaySound("IG_warning");
        for (int i = 0; i < 6; i++)
        {
            if (warning.activeSelf)
            {
                warning.SetActive(false);
            }
            else
            {
                warning.SetActive(true);
            }
            yield return new WaitForSeconds(0.1f);
        }

        index += 1;
        Destroy(warning, 0.02f);
        AudioManager.instance.PlaySound("IG_lazer");
    }

    public void BossDefeated()
    {
        BossStatus.bossStat.BossDefeated();
    }

    public void BeginSmoke()
    {
        smokeParticle.Play();
    }
}
