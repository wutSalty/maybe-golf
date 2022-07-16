using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUpgrade : PlayerUpgrades
{
    [field: Header("Child Class")]
    [field: SerializeField] public int[] Damages { get; private set; }

    private void Start()
    {
        CharMovement.UpdateDamage(Damages[0], CalculatePercentage(Damages[0], Damages[2]));
        CharMovement.UpdateMaxHits(3);
    }

    public override void CalculateStatDiff()
    {
        int damageDiff = Damages[CurrentLevel] - Damages[CurrentLevel - 1];
        HoverForAddition.WhatShouldTheTextSay = "+ " + CalculatePercentage(damageDiff, Damages[2]);
    }

    public override void ApplyUpgrade()
    {
        CharMovement.UpdateDamage(Damages[CurrentLevel - 1], CalculatePercentage(Damages[CurrentLevel - 1], Damages[2]));
    }
}
