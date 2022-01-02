using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen loadingScreen;

    public GameObject loadScreen;
    public Slider loadingSlider;
    public CanvasGroup canvasGroup;

    private void Awake()
    {
        if (loadingScreen != null || loadingScreen != this)
        {
            loadingScreen = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);
    }

    public void BeginLoadingScene(string SceneToLoad)
    {
        StartCoroutine(StartLoad(SceneToLoad));
    }

    IEnumerator StartLoad(string SceneToLoad)
    {
        loadScreen.SetActive(true);
        yield return StartCoroutine(FadeScreen(1, 1, 0));

        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneToLoad);
        while (!operation.isDone)
        {
            loadingSlider.value = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }

        yield return StartCoroutine(FadeScreen(0, 1, 1));
        loadScreen.SetActive(false);
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
