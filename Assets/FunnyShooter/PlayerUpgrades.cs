using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUpgrades : MonoBehaviour
{
    [field: Header("Base Class")]
    [field: SerializeField] public string UpgradeName { get; private set; }
    [field: SerializeField] public int CurrentLevel { get; private set; } = 1;
    [field: SerializeField] public string DefaultDescription { get; private set; }

    [field: SerializeField] public int[] UpgradeCost { get; private set; }

    [field: SerializeField] public MouseHover HoverForDesc { get; private set; }
    [field: SerializeField] public MouseHover HoverForAddition { get; private set; }
    [field: SerializeField] public Image UpgradeIndicator { get; private set; }

    protected FunnyCharMovement CharMovement;
    protected FunnyPlayerHealth PlayerHealth;

    private void Awake()
    {
        CharMovement = GetComponentInParent<FunnyCharMovement>();
        PlayerHealth = GetComponentInParent<FunnyPlayerHealth>();
    }

    public int SetNewCurrentLevel(int SetLevel)
    {
        CurrentLevel = SetLevel;
        return SetLevel;
    }

    public void SetNewDesc(string theText)
    {
        HoverForDesc.WhatShouldTheTextSay = theText;
    }

    public void SetNewAddition(string theText)
    {
        HoverForAddition.WhatShouldTheTextSay = theText;
    }

    public void SetNewIndicator(Sprite sprite)
    {
        UpgradeIndicator.sprite = sprite;
    }

    public void UpdateDescriptionText()
    {
        HoverForDesc.WhatShouldTheTextSay = DefaultDescription + " (Cost: " + UpgradeCost[CurrentLevel] + ")";
    }

    public virtual void CalculateStatDiff()
    {
        print("Lol wrong one");
    }

    public virtual void ApplyUpgrade()
    {
        print("Lol wrong one");
    }

    public string CalculatePercentage(int ValueA, int ValueB)
    {
        float percentageFloat = (ValueA * 1.0f / ValueB) * 100;
        int percentageInt = Mathf.RoundToInt(percentageFloat);
        string percentageString = percentageInt.ToString() + "%";
        return percentageString;
    }

    public string CalculatePercentage(float ValueA, float ValueB)
    {
        float percentageFloat = (ValueA / ValueB) * 100;
        int percentageInt = Mathf.RoundToInt(percentageFloat);
        string percentageString = percentageInt.ToString() + "%";
        return percentageString;
    }
}
