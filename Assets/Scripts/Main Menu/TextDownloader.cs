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

    [SerializeField]
    private string LinkToAppData = "https://drive.google.com/uc?export=download&id=1kzGmaJT3tPalS1BbYc4WG4VqjVg8Z6kz";

    private AppdataClass jsonHere;

    void Start()
    {
        if (GameManager.GM.DownloadedText == "")
        {
            StartCoroutine(CheckFiles()); //If not already downloaded, check the files
        }
        else
        {
            TextSpace.text = GameManager.GM.DownloadedText;
        }
    }

    IEnumerator CheckFiles()
    {
        TextSpace.text = PlaceholderText;
        yield return new WaitForSecondsRealtime(3.5f); //Set a wait time to make the player think its doing stuff

        yield return StartCoroutine(GetAppdata()); //Wait here while we get the appdata json

        TextSpace.text = GameManager.GM.DownloadedText; //Then set the text to the proper text when done
    }

    IEnumerator GetAppdata()
    {
        UnityWebRequest webRequest = new UnityWebRequest(LinkToAppData); //Get the appdata json
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success) //If no good, just say no internet
        {
            print(webRequest.error);
            GameManager.GM.DownloadedText = Application.version + NoInternetText;
        }
        else //Or else download json as text file and chuck it into check the json
        {
            string jsonData = webRequest.downloadHandler.text;
            CheckTheJSON(jsonData);
        }
    }

    private void CheckTheJSON(string json)
    {
        GameManager.GM.jsonHere = JsonUtility.FromJson<AppdataClass>(json); //Chuck text into class
        jsonHere = GameManager.GM.jsonHere;

        if (jsonHere.appID != GameManager.GM.appID) //If the id in json is different to internal id, throw error
        {
            GameManager.GM.DownloadedText = Application.version + NoInternetText;
        }
        else if (jsonHere.appBuild != GameManager.GM.appBuild) //If build is different, say update available
        {
            GameManager.GM.DownloadedText = "Your version: " + Application.version + "\n" + "Latest version: " + jsonHere.appVer + "\n\n" + jsonHere.updateAvailable;
        }
        else
        {
            string text = Application.version + jsonHere.internetAvailable;
            GameManager.GM.DownloadedText = text; //If they are the same, then we can say success
        }
    }
}
