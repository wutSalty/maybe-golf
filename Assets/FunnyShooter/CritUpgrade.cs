using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritUpgrade : PlayerUpgrades
{
    [field: Header("Child Class")]
    [field: SerializeField] public float[] Crits { get; private set; }

    private void Start()
    {
        CharMovement.UpdateCritRate(Crits[0], (Crits[0] * 100).ToString() + "%");
    }

    public override void CalculateStatDiff()
    {
        float critDiff = Crits[CurrentLevel] - Crits[CurrentLevel - 1];
        HoverForAddition.WhatShouldTheTextSay = "+ " + (critDiff * 100).ToString() + "%";
    }

    public override void ApplyUpgrade()
    {
        CharMovement.UpdateCritRate(Crits[CurrentLevel - 1], (Crits[CurrentLevel - 1] * 100).ToString() + "%");
    }
}
