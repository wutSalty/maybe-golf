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
    public GameObject loadingCanvas;
    public Slider loadingSlider;
    public CanvasGroup canvasGroup;
    public Image BallImg;

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
        loadingCanvas.SetActive(true);
        yield return StartCoroutine(FadeScreen(1, 1, 0));

        Time.timeScale = 1;

        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneToLoad);
        while (!operation.isDone)
        {
            //loadingSlider.value = Mathf.Clamp01(operation.progress / 0.9f);
            loadingSlider.value = Mathf.Clamp(operation.progress / 0.9f, 0, 0.94f);
            yield return null;
        }

        yield return StartCoroutine(FadeScreen(0, 1, 1));
        loadingCanvas.SetActive(false);

        if (timer)
        {
            GameStatus.gameStat.BeginGame();
        }
    }

    IEnumerator StartLoadMusic(string SceneToLoad, bool timer, string oldAudio, string newAudio)
    {
        BallImg.sprite = GameManager.GM.BallSkins[GameManager.GM.BallSkin];

        loadingSlider.value = 0;
        loadingCanvas.SetActive(true);
        StartCoroutine(TransitionBGM(oldAudio, 1));

        yield return StartCoroutine(FadeScreen(1, 1, 0));

        Time.timeScale = 1;

        //If loading a course and Player 1 disconnects then cancel the SceneLoad
        if (SceneManager.GetActiveScene().name == "MainMenu" && GameManager.GM.NumPlayers[0].PlayerIndex == 99)
        {
            StartCoroutine(TransitionBGM(oldAudio, 1));
            yield return StartCoroutine(FadeScreen(0, 1, 1));
            loadingCanvas.SetActive(false);
            yield break;
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneToLoad);
        while (!operation.isDone)
        {
            //loadingSlider.value = Mathf.Clamp01(operation.progress / 0.9f);
            loadingSlider.value = Mathf.Clamp(operation.progress / 0.9f, 0, 0.94f);
            yield return null;
        }

        StartCoroutine(TransitionBGM(newAudio, 1));

        yield return StartCoroutine(FadeScreen(0, 1, 1));
        loadingCanvas.SetActive(false);

        if (timer)
        {
            GameStatus.gameStat.BeginGame();
        }
    }

    //Fades or unfades the screen as required
    IEnumerator FadeScreen(float targetValue, float duration, float ogValue)
    {
        float startValue = ogValue;
        float time = 0;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startValue, targetValue, time / duration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = targetValue;
    }

    public void FadeBGM(string name)
    {
        StartCoroutine(TransitionBGM(name, 1));
    }

    IEnumerator TransitionBGM(string oldAudio, float duration)
    {
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
