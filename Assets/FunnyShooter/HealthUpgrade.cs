using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpgrade : PlayerUpgrades
{
    [field: Header("Child Class")]
    [field: SerializeField] public int[] PlayerHealths { get; private set; }

    private void Start()
    {
        PlayerHealth.UpgradeHealth(PlayerHealths[0]);
    }

    public override void CalculateStatDiff()
    {
        int hpDiff = PlayerHealths[CurrentLevel] - PlayerHealths[CurrentLevel - 1];
        HoverForAddition.WhatShouldTheTextSay = "+ " + hpDiff.ToString();
    }

    public override void ApplyUpgrade()
    {
        PlayerHealth.UpgradeHealth(PlayerHealths[CurrentLevel - 1]);
    }
}
