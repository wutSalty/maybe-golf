using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

//The big ol Save and Load system
public static class SaveSystem 
{
    //To save the game. Convert data to binary then save to file
    public static void SaveGame(PlayerData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playerData.golf";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    //Load the game. Open file, convert to data then return data
    public static PlayerData LoadGame()
    {
        string path = Application.persistentDataPath + "/playerData.golf";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;

        } else
        {
            Debug.Log("Oppsie woopsie, file doesn't exist");
            return null;
        }
    }
}
