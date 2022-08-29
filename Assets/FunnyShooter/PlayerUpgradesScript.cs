using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class PlayerUpgradesScript : MonoBehaviour
{
    public FunnyGameManager gameMan;
    private FunnyCharMovement movementScript;
    private FunnyPlayerPause playerPause;

    public EventSystem eventSys;
    public PlayerInput pInput;
    public InputSystemUIInputModule normalInputModule;

    [Header("Input Action References")]
    public InputActionReference gamePoint;
    public InputActionReference menuPoint;

    public InputActionReference gameMove;
    public InputActionReference menuMove;

    public InputActionReference gameClick;
    public InputActionReference menuClick;

    public InputActionReference gameSubmit;
    public InputActionReference menuSubmit;

    [Header("Others")]
    public GameObject shopObject;
    public GameObject firstShopItem;
    public Text ShopErrorText;

    public PlayerUpgrades[] Upgrades;

    //Sprites showing locked and unlocked upgrades
    public Sprite[] UpgradeImages;

    [HideInInspector] public bool shopOpened = false;

    private Coroutine coroutine;

    private Animator canvasAnim;

    void Start()
    {
        movementScript = GetComponent<FunnyCharMovement>();
        playerPause = GetComponent<FunnyPlayerPause>();
        canvasAnim = shopObject.GetComponentInParent<Animator>();
        pInput = GetComponent<PlayerInput>();

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
        if (playerPause.gamePaused || !gameMan.GameIsActive)
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
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            ShopErrorText.text = "";

            Time.timeScale = 1;
            shopOpened = false;

            canvasAnim.Play("Shop_Close");
            eventSys.SetSelectedGameObject(null);
            eventSys.firstSelectedGameObject = null;

            normalInputModule.point = menuPoint;
            normalInputModule.move = menuMove;
            normalInputModule.leftClick = menuClick;
            normalInputModule.submit = menuSubmit;
        }
        else
        {
            Time.timeScale = 0.1f;
            shopOpened = true;

            normalInputModule.point = gamePoint;
            normalInputModule.move = gameMove;
            normalInputModule.leftClick = gameClick;
            normalInputModule.submit = gameSubmit;

            canvasAnim.Play("Shop_Open");
            eventSys.firstSelectedGameObject = firstShopItem;

            if (pInput.currentControlScheme == "Controller")
            {
                eventSys.SetSelectedGameObject(firstShopItem);
            }
        }
    }

    public void ForceCloseShop()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        ShopErrorText.text = "";

        shopOpened = false;
        canvasAnim.Play("Shop_Close");
        eventSys.SetSelectedGameObject(null);
    }

    public void ClickedOnUpgrade(string UpgradeName)
    {
        PlayerUpgrades theUpgrade = Array.Find(Upgrades, u => u.UpgradeName == UpgradeName);
        if (theUpgrade == null)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            print("Error. Upgrade " + UpgradeName + " not found");
            coroutine = StartCoroutine(TextForAFewSeconds("Error. Upgrade " + UpgradeName + " not found"));
            return;
        }

        if (theUpgrade.CurrentLevel >= theUpgrade.UpgradeCost.Length)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            print("Upgrade already maxed");
            coroutine = StartCoroutine(TextForAFewSeconds("Upgrade already maxed"));
            return;
        }

        int cash = movementScript.GetCash(); //Current Cash
        int required = theUpgrade.UpgradeCost[theUpgrade.CurrentLevel];

        if (cash < required)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            print("Not enough cash. Currently have " + cash.ToString() + ". Require " + required.ToString());
            coroutine = StartCoroutine(TextForAFewSeconds("Not enough cash. Currently have " + cash.ToString() + ". Require " + required.ToString()));
        }
        else
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            print("Successful. Subtracted " + required.ToString());
            coroutine = StartCoroutine(TextForAFewSeconds("Successful. Subtracted " + required.ToString()));

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

    private IEnumerator TextForAFewSeconds(string WhatToSay)
    {
        ShopErrorText.text = WhatToSay;
        yield return new WaitForSecondsRealtime(2f);
        ShopErrorText.text = "";
    }
}
