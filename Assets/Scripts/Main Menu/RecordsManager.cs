using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RecordsManager : MonoBehaviour
{
    //Elements for ball skins
    public GameObject parentPanel;
    public Sprite UnknownBallSprite;

    //Elements for scroll shelf
    public Image[] ScrollImages;
    public Button[] ScrollButtons;
    public Sprite[] KnownScrollSprites;
    public Sprite UnknownScrollSprite;

    //Elements for scroll screen
    public Text ScrollTitle;
    public Text ScrollBody;
    public GameObject ScrollTextPanel;
    [System.Serializable]
    public class ScrollData
    {
        public string ScrollName;
        [TextArea(4,10)] public string TheActualText;
    }
    public ScrollData[] scrollDatas;
    public GameObject ScrollBackButton;

    //Elements for level records
    public Text LevelNameTxt;
    public Text HitRecordTxt;
    public Text BestTimeTxt;
    private int CurrentLevelIndex = 0;
    public List<string> LevelNames;

    public Text TimesPlayedSoloTxt;
    public Text TimesPlayedMultiTxt;

    //EventSys stuff
    public EventSystem eventSystem;
    public GameObject LastSelectedObject;

    private void Start()
    {
        foreach (var item in GameManager.GM.LevelData)
        {
            LevelNames.Add(item.ExternalName);
        }
    }

    //To update everything in the records tab
    public void UpdateThings()
    {
        UpdateSkins();

        CheckScrolls();

        FirstOpenLevelRecords();

        CheckOtherStats();
    }

    //Updating the ball skins based on unlocked balls
    public void UpdateSkins()
    {
        foreach (Transform children in parentPanel.transform) //Delete all the old balls first
        {
            Destroy(children.gameObject);
        }

        int index = 0;
        foreach (var item in GameManager.GM.BallSkins) //Then create new images for each of the balls and substitute the appropriate sprites
        {
            GameObject NewThing = new GameObject();
            Image NewImage = NewThing.AddComponent<Image>();
            Spiiin NewSpin = NewThing.AddComponent<Spiiin>();

            NewThing.name = "Ball Icon " + index;
            NewThing.transform.SetParent(parentPanel.transform, false);
            NewSpin.SpinSpeed = 150;

            if (GameManager.GM.UnlockedBallSkins[index])
            {
                NewImage.sprite = item;
            }
            else
            {
                NewImage.sprite = UnknownBallSprite;
            }

            NewThing.SetActive(true);

            index += 1;
        }
    }

    //Check which scrolls have already been unlocked and accessible
    public void CheckScrolls()
    {
        int index = 0;
        foreach (var item in GameManager.GM.LevelData)
        {
            if (item.LevelInt == 5)
            {
                break;
            }

            if (item.CollectableGet != 2)
            {
                ScrollImages[index].sprite = UnknownScrollSprite;
            } else
            {
                ScrollImages[index].sprite = KnownScrollSprites[index];
            }
            index += 1;
        }
    }

    //When scroll button pressed, update the text then show the screen
    public void ClickScroll(int ScrollInt)
    {
        if (GameManager.GM.LevelData[ScrollInt].CollectableGet != 2)
        {
            ScrollTitle.text = "???";
            ScrollBody.text = "This scroll hasn't been collected yet. Try looking around different levels to see if you can find it...";
        }
        else
        {
            ScrollTitle.text = scrollDatas[ScrollInt].ScrollName;
            ScrollBody.text = scrollDatas[ScrollInt].TheActualText;
        }

        ScrollTextPanel.SetActive(true);

        LastSelectedObject = eventSystem.currentSelectedGameObject;
        eventSystem.SetSelectedGameObject(ScrollBackButton);
        eventSystem.firstSelectedGameObject = ScrollBackButton;
        AudioManager.instance.PlaySound("UI_beep");
    }

    //Close scroll screen
    public void ClickCloseScroll()
    {
        ScrollTextPanel.SetActive(false);

        eventSystem.SetSelectedGameObject(LastSelectedObject);
        eventSystem.firstSelectedGameObject = LastSelectedObject;
        LastSelectedObject = null;
        AudioManager.instance.PlaySound("UI_beep");
    }

    //Reset best-stuff to tutorial stuff
    public void FirstOpenLevelRecords()
    {
        CurrentLevelIndex = 0;
        LevelNameTxt.text = "Tutorial";
        if (GameManager.GM.LevelData[0].BestHits == 0)
        {
            HitRecordTxt.text = "Hit Record: N/A";
            BestTimeTxt.text = "Best Time: N/A";
        }
        else
        {
            HitRecordTxt.text = "Hit Record: " + GameManager.GM.LevelData[0].BestHits;
            BestTimeTxt.text = "Best Time: " + GameManager.GM.LevelData[0].BestTime.ToString("F2") + " sec";
        } 
    }

    public void LevelClickLeft()
    {
        CurrentLevelIndex -= 1;
        if (CurrentLevelIndex < 0)
        {
            CurrentLevelIndex = GameManager.GM.LevelData.Count - 1;
        }

        //Hiding Boss Level
        if (CurrentLevelIndex == 5 && !GameManager.GM.BossLevelUnlocked)
        {
            CurrentLevelIndex -= 1;
        }

        LevelNameTxt.text = LevelNames[CurrentLevelIndex];
        if (GameManager.GM.LevelData[CurrentLevelIndex].BestTime == 0)
        {
            HitRecordTxt.text = "Hit Record: N/A";
            BestTimeTxt.text = "Best Time: N/A";
        }
        else if (CurrentLevelIndex == 5)
        {
            HitRecordTxt.text = "Hit Record: N/A";
            BestTimeTxt.text = "Best Time: " + GameManager.GM.LevelData[CurrentLevelIndex].BestTime.ToString("F2") + " sec";
        }
        else
        {
            HitRecordTxt.text = "Hit Record: " + GameManager.GM.LevelData[CurrentLevelIndex].BestHits;
            BestTimeTxt.text = "Best Time: " + GameManager.GM.LevelData[CurrentLevelIndex].BestTime.ToString("F2") + " sec";
        }
        AudioManager.instance.PlaySound("UI_beep");
    }

    public void LevelClickRight()
    {
        CurrentLevelIndex += 1;
        if (CurrentLevelIndex > GameManager.GM.LevelData.Count - 1)
        {
            CurrentLevelIndex = 0;
        }

        if (CurrentLevelIndex == 5 && !GameManager.GM.BossLevelUnlocked)
        {
            CurrentLevelIndex = 0;
        }

        LevelNameTxt.text = LevelNames[CurrentLevelIndex];
        if (GameManager.GM.LevelData[CurrentLevelIndex].BestTime == 0)
        {
            HitRecordTxt.text = "Hit Record: N/A";
            BestTimeTxt.text = "Best Time: N/A";
        }
        else if (CurrentLevelIndex == 5)
        {
            HitRecordTxt.text = "Hit Record: N/A";
            BestTimeTxt.text = "Best Time: " + GameManager.GM.LevelData[CurrentLevelIndex].BestTime.ToString("F2") + " sec";
        }
        else
        {
            HitRecordTxt.text = "Hit Record: " + GameManager.GM.LevelData[CurrentLevelIndex].BestHits;
            BestTimeTxt.text = "Best Time: " + GameManager.GM.LevelData[CurrentLevelIndex].BestTime.ToString("F2") + " sec";
        }
        AudioManager.instance.PlaySound("UI_beep");
    }

    public void CheckOtherStats()
    {
        TimesPlayedSoloTxt.text = "Times Played (Solo): " + GameManager.GM.TimesPlayedSolo;
        TimesPlayedMultiTxt.text = "Times Played (Multi): " + GameManager.GM.TimesPlayedMulti;
    }
}
