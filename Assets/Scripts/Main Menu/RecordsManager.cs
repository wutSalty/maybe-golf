using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordsManager : MonoBehaviour
{
    public GameObject parentPanel;
    public Sprite UnknownBallSprite;

    public void UpdateSkins()
    {
        foreach (Transform children in parentPanel.transform)
        {
            Destroy(children.gameObject);
        }

        int index = 0;
        foreach (var item in GameManager.GM.BallSkins)
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
}
