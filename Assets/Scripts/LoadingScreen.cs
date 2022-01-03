using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen loadMan;

    public GameObject loadingCanvas;
    public Slider loadingSlider;
    public CanvasGroup canvasGroup;
    public GameObject SpinImg;

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

    public void BeginLoadingScene(string SceneToLoad, bool timer)
    {
        StartCoroutine(StartLoad(SceneToLoad, timer));
    }

    IEnumerator StartLoad(string SceneToLoad, bool timer)
    {
        SpinImg.transform.localRotation = Quaternion.identity;
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
            GameStatus.gameStat.StartTimer();
        }
    }

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
