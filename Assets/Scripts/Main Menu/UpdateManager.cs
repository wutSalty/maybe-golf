using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UpdateManager : MonoBehaviour
{
    private string downloadServer = "";
    private string saveTo = "\\temp";
    private string ExefileName = "\\updater.exe";
    private string BatfileName = "\\updater.bat";

    [TextArea(10, 20)]
    public string batchContents;

    public Text CurrentlyDoingText;

    //Get location from download, and make sure the path exists
    public void BeginDownloadingUpdate()
    {
        CurrentlyDoingText.text = "Currently checking file system...";

        downloadServer = GameManager.GM.jsonHere.downloadServer;

        string path = Application.persistentDataPath + saveTo;

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        StartCoroutine(DownloadFile(path));
    }

    //Grab exe from webserver and save the data
    IEnumerator DownloadFile(string path)
    {
        yield return new WaitForSecondsRealtime(5f);

        CurrentlyDoingText.text = "Currently downloading updater...";

        UnityWebRequest www = new UnityWebRequest(downloadServer);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            print(www.error);
            //Return call to UI manager to show failed attempt and to try again
        }
        else
        {
            File.WriteAllBytes(path + ExefileName, www.downloadHandler.data);
            CreateBatchFile(path);
        }
    }

    //Use the user's exe to grab game location and edit a batch file to fit
    private void CreateBatchFile(string path)
    {
        CurrentlyDoingText.text = "Finalising updater...";

        string exeLoc = Application.dataPath;
        if (Application.platform == RuntimePlatform.OSXPlayer)
        {
            exeLoc = Path.GetFullPath(Path.Combine(exeLoc, @"..\..\"));
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            exeLoc = Path.GetFullPath(Path.Combine(exeLoc, @"..\"));
        }
        batchContents = batchContents.Replace("[DIR]", exeLoc);
        batchContents = batchContents.Replace("[EXE]", Application.productName + ".exe");

        string batchLocation = path + BatfileName;
        StreamWriter writer = new StreamWriter(batchLocation, false);
        writer.Write(batchContents);
        writer.Close();

        StartCoroutine(BeginUpdate(batchLocation));
    }

    //Countdown to install
    IEnumerator BeginUpdate(string path)
    {
        yield return new WaitForSecondsRealtime(3f);

        CurrentlyDoingText.text = "Download complete!\nThe game will close in 3";
        yield return new WaitForSecondsRealtime(1f);

        CurrentlyDoingText.text = "Download complete!\nThe game will close in 2";
        yield return new WaitForSecondsRealtime(1f);

        CurrentlyDoingText.text = "Download complete!\nThe game will close in 1";
        yield return new WaitForSecondsRealtime(1f);

        BeginUpdating(path);
    }

    //Run the batch file then close the game
    private void BeginUpdating(string path)
    {
        System.Diagnostics.Process.Start(path);
        Application.Quit();
    }
}
