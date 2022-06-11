using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class TextDownloader : MonoBehaviour
{
    public TMP_Text TextSpace;
    [SerializeField, TextArea(5, 10)]
    private string PlaceholderText = "";
    [SerializeField, TextArea(5, 10)]
    private string NoInternetText = "";

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        TextSpace.text = PlaceholderText;
        yield return new WaitForSecondsRealtime(3f);

        UnityWebRequest www = new UnityWebRequest("https://drive.google.com/uc?export=download&id=1GMU49z_jDFNIPY1T0uSCVo5LG87J1twq");
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            TextSpace.text = NoInternetText;
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            TextSpace.text = www.downloadHandler.text;
        }
    }
}
