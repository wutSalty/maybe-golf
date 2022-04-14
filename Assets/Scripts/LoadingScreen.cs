using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Handles loading between scenes and the loading screen
public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen loadMan;

    //UI elements for loading
    public Slider loadingSlider;
    public Image BallImg;
    public Text NextStopText;
    public Image NextStopImg;
    public Sprite[] NextStopSprites;

    public Animator anim;
    public float WaitTime = 1f;

    private void Awake()
    {
        if (loadMan != null && loadMan != this)
        {
            Destroy(this.gameObject);
        } else
        {
            loadMan = this;
        }
        DontDestroyOnLoad(this);
    }

    //When instructed to begin, begin loading
    public void BeginLoadingScene(string SceneToLoad, bool timer)
    {
        StartCoroutine(StartLoad(SceneToLoad, timer));
    }

    public void LoadingMusic(string SceneToLoad, bool timer, string newAudio)
    {
        StartCoroutine(StartLoadMusic(SceneToLoad, timer, AudioManager.instance.CurrentlyPlayingBGM, newAudio));
    }

    //Begins fade to black, loads the scene in the background, then fades out
    IEnumerator StartLoad(string SceneToLoad, bool timer)
    {
        BallImg.sprite = GameManager.GM.BallSkins[GameManager.GM.BallSkin];
        loadingSlider.value = 0;

        switch (SceneToLoad)
        {
            case "MainMenu":
                NextStopText.text = "Next Stop: Main Menu";
                NextStopImg.sprite = NextStopSprites[0];
                break;

            case "SampleScene":
                NextStopText.text = "Next Stop: Tutorial";
                NextStopImg.sprite = NextStopSprites[1];
                break;

            case "LevelOne":
                NextStopText.text = "Next Stop: Level One";
                NextStopImg.sprite = NextStopSprites[2];
                break;

            case "LevelTwo":
                NextStopText.text = "Next Stop: Level Two";
                NextStopImg.sprite = NextStopSprites[3];
                break;

            case "LevelThree":
                NextStopText.text = "Next Stop: Level Three";
                NextStopImg.sprite = NextStopSprites[4];
                break;

            case "LevelFour":
                NextStopText.text = "Next Stop: Level Four";
                NextStopImg.sprite = NextStopSprites[5];
                break;

            case "TheFinalBoss":
                NextStopText.text = "Next Stop: Final Boss";
                NextStopImg.sprite = NextStopSprites[6];
                break;

            default:
                NextStopText.text = "Next Stop: Unknown...?";
                NextStopImg.sprite = NextStopSprites[0];
                break;
        }
        NextStopImg.SetNativeSize();

        anim.SetTrigger("BeginLoad");
        yield return new WaitForSecondsRealtime(WaitTime);

        Time.timeScale = 1;

        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneToLoad);
        while (!operation.isDone)
        {
            loadingSlider.value = Mathf.Clamp(operation.progress / 0.9f, 0, 0.94f);
            yield return null;
        }

        anim.SetTrigger("EndLoad");
        yield return new WaitForSecondsRealtime(WaitTime);

        if (timer)
        {
            yield return new WaitForSecondsRealtime(0.3f);
            GameStatus.gameStat.BeginGame();
        }
    }

    IEnumerator StartLoadMusic(string SceneToLoad, bool timer, string oldAudio, string newAudio)
    {
        BallImg.sprite = GameManager.GM.BallSkins[GameManager.GM.BallSkin];
        loadingSlider.value = 0;

        switch (SceneToLoad)
        {
            case "MainMenu":
                NextStopText.text = "Next Stop: Main Menu";
                NextStopImg.sprite = NextStopSprites[0];
                break;

            case "SampleScene":
                NextStopText.text = "Next Stop: Tutorial";
                NextStopImg.sprite = NextStopSprites[1];
                break;

            case "LevelOne":
                NextStopText.text = "Next Stop: Level One";
                NextStopImg.sprite = NextStopSprites[2];
                break;

            case "LevelTwo":
                NextStopText.text = "Next Stop: Level Two";
                NextStopImg.sprite = NextStopSprites[3];
                break;

            case "LevelThree":
                NextStopText.text = "Next Stop: Level Three";
                NextStopImg.sprite = NextStopSprites[4];
                break;

            case "LevelFour":
                NextStopText.text = "Next Stop: Level Four";
                NextStopImg.sprite = NextStopSprites[5];
                break;

            case "TheFinalBoss":
                NextStopText.text = "Next Stop: Final Boss";
                NextStopImg.sprite = NextStopSprites[6];
                break;

            default:
                NextStopText.text = "Next Stop: Unknown...?";
                NextStopImg.sprite = NextStopSprites[0];
                break;
        }
        NextStopImg.SetNativeSize();

        StartCoroutine(TransitionBGM(oldAudio, 1));

        anim.SetTrigger("BeginLoad");
        yield return new WaitForSecondsRealtime(WaitTime);

        Time.timeScale = 1;

        //If loading a course and Player 1 disconnects then cancel the SceneLoad
        if (SceneManager.GetActiveScene().name == "MainMenu" && GameManager.GM.NumPlayers[0].PlayerIndex == 99)
        {
            StartCoroutine(TransitionBGM(oldAudio, 1));
            anim.SetTrigger("EndLoad");
            yield return new WaitForSecondsRealtime(WaitTime);
            yield break;
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneToLoad);
        while (!operation.isDone)
        {
            loadingSlider.value = Mathf.Clamp(operation.progress / 0.9f, 0, 0.94f);
            yield return null;
        }

        StartCoroutine(TransitionBGM(newAudio, 1));

        anim.SetTrigger("EndLoad");
        yield return new WaitForSecondsRealtime(WaitTime);

        if (timer)
        {
            yield return new WaitForSecondsRealtime(0.3f);
            GameStatus.gameStat.BeginGame();
        }
    }

    public void FadeBGM(string name)
    {
        StartCoroutine(TransitionBGM(name, 1));
    }

    IEnumerator TransitionBGM(string oldAudio, float duration)
    {
        print("ping, music change");

        Sound s = Array.Find(AudioManager.instance.sounds, sound => sound.name == oldAudio);
        if (s == null)
        {
            Debug.Log("Uhoh, can't find " + oldAudio + " to play");
            yield break;
        }

        float ogValue = 0;
        float newValue = 0;
        bool WasPlaying = false;

        if (!s.source.isPlaying)
        {
            ogValue = 0;
            s.source.volume = 0;
            
            switch (s.audioPurpose)
            {
                case 0:
                    newValue = PlayerPrefs.GetFloat("BGM", 5) / 10;
                    AudioManager.instance.CurrentlyPlayingBGM = s.name;
                    break;

                case 1:
                    newValue = PlayerPrefs.GetFloat("UI", 5) / 10;
                    break;

                case 2:
                    newValue = PlayerPrefs.GetFloat("InGame", 5) / 10;
                    break;

                default:
                    break;
            }

            s.source.Play();
        }
        else
        {
            WasPlaying = true;
            ogValue = s.source.volume;
            newValue = 0;
        }

        float time = 0;

        while (time < duration)
        {
            s.source.volume = Mathf.Lerp(ogValue, newValue, time / duration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        s.source.volume = newValue;

        if (WasPlaying)
        {
            s.source.Stop();
            s.source.volume = ogValue;
        }
    }
}
