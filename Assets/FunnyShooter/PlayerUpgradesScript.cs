using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerUpgradesScript : MonoBehaviour
{
    private PlayerInput pInput;
    private FunnyCharMovement movementScript;
    private FunnyPlayerPause playerPause;
    private FunnyPlayerHealth playerHealth;

    public EventSystem eventSys;

    public GameObject shopObject;
    public GameObject firstShopItem;

    [Header("Tracking Upgrades")]
    public int[] UpgradeLevel;
    //Each element is an upgrade. The integer in each element represents the current upgrade

    public int[] UpgradeCost;
    //Cost to go to next upgrade

    public string[] DefaultText;
    //Default text for each upgrade (before cost)

    public MouseHover[] MouseHovers;
    //Where to edit the text

    public Sprite[] UpgradeImages;
    //Sprites showing locked and unlocked upgrades

    public Image[] UpgradeIndicators;
    //Images for each upgrade

    public int NumUpgrades = 5;
    //Number of upgrades

    public MouseHover[] WhatWillChangeHovers;
    //Text to show change in values

    public Button[] UpgradeButtons;
    //Buttons required

    [HideInInspector] public bool shopOpened = false;

    [Header("Upgrades")]
    public int[] Damages;
    public float[] CritRates;
    public float[] PlayerSpeeds;
    public int[] PlayerHealths;
    public float[] PickupRange;

    void Start()
    {
        pInput = GetComponent<PlayerInput>();
        movementScript = GetComponent<FunnyCharMovement>();
        playerPause = GetComponent<FunnyPlayerPause>();
        playerHealth = GetComponent<FunnyPlayerHealth>();
        NumUpgrades = UpgradeLevel.Length;

        OverrideDefaultPlayerValues();
        UpdateShopListings();
    }

    //Replace any defaults with the defaults here
    private void OverrideDefaultPlayerValues()
    {
        movementScript.UpdateDamage(Damages[0], CalculatePercentage(Damages[0], Damages[2]));
        movementScript.UpdateCritRate(CritRates[0], (CritRates[0]*100).ToString() + "%");
        movementScript.UpdateSpeed(PlayerSpeeds[0], CalculatePercentage(PlayerSpeeds[0], PlayerSpeeds[2]));
        playerHealth.UpgradeHealth(PlayerHealths[0]);
        movementScript.UpdateMagnetRange(PickupRange[0], CalculatePercentage(PickupRange[0], PickupRange[2]));
    }

    //For each upgrade, make sure their proper sprite is applied, the cost amount is correct, and update the cost required
    private void UpdateShopListings() 
    {
        for (int i = 0; i < NumUpgrades; i++)
        {
            UpgradeIndicators[i].sprite = UpgradeImages[UpgradeLevel[i]];
            MouseHovers[i].WhatShouldTheTextSay = DefaultText[i] + " (Cost: " + UpgradeCost[UpgradeLevel[i]] + ")";
            UpdateWhatWillChangeText(i);
        }
    }

    //When menu button pushed
    private void OnShop()
    {
        if (playerPause.gamePaused)
        {
            return;
        }

        CheckShopStatus();
    }

    //Open or close the shop
    public void CheckShopStatus()
    {
        if (shopOpened)
        {
            Time.timeScale = 1;
            shopOpened = false;
            shopObject.SetActive(false);
            eventSys.SetSelectedGameObject(null);
            pInput.SwitchCurrentActionMap("Game");

            foreach (var item in WhatWillChangeHovers)
            {
                item.ForceDeselect();
            }

            movementScript.SetHoldingFireState(false);
        }
        else
        {
            Time.timeScale = 0;
            shopOpened = true;
            pInput.SwitchCurrentActionMap("Menu");
            shopObject.SetActive(true);
            eventSys.SetSelectedGameObject(firstShopItem);
        }
    }

    //When the user clicks on an upgrade, make sure they have enough cash
    public void ClickedOnUpgrade(int upgrade)
    {
        if (UpgradeLevel[upgrade] >= UpgradeCost.Length)
        {
            print("Upgrade already maxed");
            return;
        }

        int cash = movementScript.GetCash(); //Current Cash
        int required = UpgradeCost[UpgradeLevel[upgrade]]; //Cash required for that specific upgrade. OOB error here

        if (cash < required)
        {
            print("Not enough cash. Currently have " + cash.ToString() + ". Require " + required.ToString());
        }
        else
        {
            print("Successful. Subtracted " + required.ToString());
            movementScript.AddToCash(-required); //Subtract amount

            ApplyUpgrade(upgrade);
        }
    }

    private void ApplyUpgrade(int upgrade)
    {
        int NewLevel = UpgradeLevel[upgrade] += 1;

        UpgradeIndicators[upgrade].sprite = UpgradeImages[NewLevel];

        if (NewLevel >= UpgradeCost.Length)
        {
            print("Upgrade maxed out");

            MouseHovers[upgrade].WhatShouldTheTextSay = DefaultText[upgrade] + " (MAX!)";
            WhatWillChangeHovers[upgrade].WhatShouldTheTextSay = "MAX!";
        }
        else
        {
            MouseHovers[upgrade].WhatShouldTheTextSay = DefaultText[upgrade] + " (Cost: " + UpgradeCost[NewLevel] + ")";
            UpdateWhatWillChangeText(upgrade);
        }

        RunUpgrades(upgrade, NewLevel);
    }

    private void RunUpgrades(int upgrade, int NewLevel)
    {
        switch (upgrade)
        {
            case 0: //Weapon Damage
                movementScript.UpdateDamage(Damages[NewLevel - 1], CalculatePercentage(Damages[NewLevel - 1], Damages[2]));
                break;

            case 1: //Crit Rate
                movementScript.UpdateCritRate(CritRates[NewLevel - 1], (CritRates[NewLevel - 1] * 100).ToString() + "%");
                break;

            case 2: //Speed
                movementScript.UpdateSpeed(PlayerSpeeds[NewLevel - 1], CalculatePercentage(PlayerSpeeds[NewLevel - 1], PlayerSpeeds[2]));
                break;

            case 3: //HP
                playerHealth.UpgradeHealth(PlayerHealths[NewLevel - 1]);
                break;

            case 4: //Pickup range
                movementScript.UpdateMagnetRange(PickupRange[NewLevel - 1], CalculatePercentage(PickupRange[NewLevel - 1], PickupRange[2]));
                break;

            default:
                break;
        }
    }

    private void UpdateWhatWillChangeText(int upgrade)
    {
        int CurrentLevel = UpgradeLevel[upgrade];

        switch (upgrade)
        {
            case 0://damage
                int damageDiff = Damages[CurrentLevel] - Damages[CurrentLevel - 1];
                WhatWillChangeHovers[upgrade].WhatShouldTheTextSay = "+ " + CalculatePercentage(damageDiff, Damages[2]);
                break;

            case 1://crit
                float critDiff = CritRates[CurrentLevel] - CritRates[CurrentLevel - 1];
                WhatWillChangeHovers[upgrade].WhatShouldTheTextSay = "+ " + (critDiff * 100).ToString() + "%";
                break;

            case 2://speed
                float speedDiff = PlayerSpeeds[CurrentLevel] - PlayerSpeeds[CurrentLevel - 1];
                WhatWillChangeHovers[upgrade].WhatShouldTheTextSay = "+ " + CalculatePercentage(speedDiff, PlayerSpeeds[2]);
                break;

            case 3://hp
                int hpDiff = PlayerHealths[CurrentLevel] - PlayerHealths[CurrentLevel - 1];
                WhatWillChangeHovers[upgrade].WhatShouldTheTextSay = "+ " + hpDiff.ToString();
                break;

            case 4://range
                float rangeDiff = PickupRange[CurrentLevel] - PickupRange[CurrentLevel - 1];
                WhatWillChangeHovers[upgrade].WhatShouldTheTextSay = "+ " + CalculatePercentage(rangeDiff, PickupRange[2]);
                break;

            default:
                break;
        }
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
