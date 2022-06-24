using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollCollectable : MonoBehaviour
{
    int CollectableStatus;
    SpriteRenderer rendering;
    Animator animator;
    bool Collected;
    ParticleSystem particles;

    void Start()
    {
        if (!GameManager.GM.SingleMode)
        {
            gameObject.SetActive(false);
            GameStatus.gameStat.UIIcon.gameObject.SetActive(false);
            return;
        }

        animator = GetComponent<Animator>();
        rendering = GetComponent<SpriteRenderer>();
        particles = GetComponentInChildren<ParticleSystem>();

        GameStatus.gameStat.CollectableStatus = GameManager.GM.LevelData[GameStatus.gameStat.GMLevelIndex].CollectableGet;
        CollectableStatus = GameStatus.gameStat.CollectableStatus;

        switch (CollectableStatus)
        {
            case 0:
                GameStatus.gameStat.UIIcon.sprite = GameStatus.gameStat.NotCollectedSprite;
                break;

            case 1: //Should never occur if GameStat saves GM to 2
                GameStatus.gameStat.UIIcon.sprite = GameStatus.gameStat.CollectedSprite;
                rendering.sprite = GameStatus.gameStat.NotCollectedSprite;
                break;

            case 2:
                GameStatus.gameStat.UIIcon.sprite = GameStatus.gameStat.CollectedSprite;
                rendering.sprite = GameStatus.gameStat.NotCollectedSprite;
                break;

            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            switch (CollectableStatus)
            {
                case 0:
                    CollectableStatus = 1;
                    GameStatus.gameStat.CollectableStatus = CollectableStatus;
                    GameStatus.gameStat.UIIcon.sprite = GameStatus.gameStat.CollectedSprite;
                    animator.SetTrigger("Trigger");
                    break;

                case 1:

                    break;

                case 2:
                    animator.SetTrigger("Trigger");
                    break;

                default:
                    break;
            }
            if (!Collected)
            {
                AudioManager.instance.PlaySound("IG_scroll");
                Collected = true;
            }
        }
    }
}
