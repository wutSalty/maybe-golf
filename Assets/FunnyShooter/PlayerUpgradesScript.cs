using System;
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
    //private FunnyPlayerHealth playerHealth;

    public EventSystem eventSys;

    public GameObject shopObject;
    public GameObject firstShopItem;

    public PlayerUpgrades[] Upgrades;

    //Sprites showing locked and unlocked upgrades
    public Sprite[] UpgradeImages;

    [HideInInspector] public bool shopOpened = false;

    void Start()
    {
        pInput = GetComponent<PlayerInput>();
        movementScript = GetComponent<FunnyCharMovement>();
        playerPause = GetComponent<FunnyPlayerPause>();
        //playerHealth = GetComponent<FunnyPlayerHealth>();

        SetupInitialUpgrades();
    }

    private void SetupInitialUpgrades()
    {
        foreach (var item in Upgrades)
        {
            item.SetNewIndicator(UpgradeImages[1]);
            item.UpdateDescriptionText();
            item.CalculateStatDiff();
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

            foreach (var item in Upgrades)
            {
                item.HoverForAddition.ForceDeselect();
            }
        }
        else
        {
            Time.timeScale = 0.1f;
            shopOpened = true;
            pInput.SwitchCurrentActionMap("Menu");
            shopObject.SetActive(true);
            eventSys.SetSelectedGameObject(firstShopItem);

            movementScript.SetHoldingFireState(false);
        }
    }

    public void ClickedOnUpgrade(string UpgradeName)
    {
        PlayerUpgrades theUpgrade = Array.Find(Upgrades, u => u.UpgradeName == UpgradeName);
        if (theUpgrade == null)
        {
            print("Error. Upgrade " + UpgradeName + " not found.");
            return;
        }

        if (theUpgrade.CurrentLevel >= theUpgrade.UpgradeCost.Length)
        {
            print("Upgrade already maxed");
            return;
        }

        int cash = movementScript.GetCash(); //Current Cash
        int required = theUpgrade.UpgradeCost[theUpgrade.CurrentLevel];

        if (cash < required)
        {
            print("Not enough cash. Currently have " + cash.ToString() + ". Require " + required.ToString());
        }
        else
        {
            print("Successful. Subtracted " + required.ToString());
            movementScript.AddToCash(-required); //Subtract amount

            ApplyUpgrade(theUpgrade);
        }
    }

    private void ApplyUpgrade(PlayerUpgrades theUpgrade)
    {
        int NewLevel = theUpgrade.SetNewCurrentLevel(theUpgrade.CurrentLevel + 1);
        theUpgrade.SetNewIndicator(UpgradeImages[NewLevel]);

        if (NewLevel >= theUpgrade.UpgradeCost.Length)
        {
            print("Upgrade maxed out");

            theUpgrade.SetNewDesc(theUpgrade.DefaultDescription + " (MAX!)");
            theUpgrade.SetNewAddition("MAX!");
        }
        else
        {
            theUpgrade.SetNewDesc(theUpgrade.DefaultDescription + " (Cost: " + theUpgrade.UpgradeCost[NewLevel] + ")");
            theUpgrade.CalculateStatDiff();
        }

        theUpgrade.ApplyUpgrade();
    }
}
