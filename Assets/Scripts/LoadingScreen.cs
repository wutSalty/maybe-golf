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

    //Begins fade to black, loads the scene in the background, then fades out
    IEnumerator StartLoad(string SceneToLoad, bool timer)
    {
        BallImg.sprite = GameManager.GM.BallSkins[PlayerPrefs.GetInt("BallSkin", 0)];

        loadingSlider.value = 0;
        loadingCanvas.SetActive(true);
        yield return StartCoroutine(FadeScreen(1, 1, 0));

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

    //Fades or unfades the screen as required
    IEnumerator FadeScreen(float targetValue, float duration, float ogValue)
    {
        float startValue = ogValue;
        float time = 0;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startValue, targetValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = targetValue;
    }
}
