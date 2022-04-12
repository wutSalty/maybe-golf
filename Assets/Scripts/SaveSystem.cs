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
        //Set streams
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();

        //Serialise, convert to bytes, then checksum
        formatter.Serialize(ms, data);
        byte[] bytes = ms.ToArray();
        var hash = GetMD5Checksum(bytes);
        Debug.Log("Hash for Save File is = " + System.BitConverter.ToString(hash).Replace("-", ""));
        ms.Close();

        //Append checksum to file
        byte[] appended = new byte[hash.Length + bytes.Length];
        System.Buffer.BlockCopy(hash, 0, appended, 0, hash.Length);
        System.Buffer.BlockCopy(bytes, 0, appended, hash.Length, bytes.Length);

        //Check that hash was written correctly
        byte[] readHash = new byte[16];
        System.Buffer.BlockCopy(appended, 0, readHash, 0, readHash.Length);
        Debug.Log("Hash from inside the save file is = " + System.BitConverter.ToString(readHash).Replace("-", ""));

        //Get path ready then save file
        string path = Application.persistentDataPath + "/playerData.golf";
        File.WriteAllBytes(path, appended);
    }

    //Reminder MD5 hash is 128 bits = 16 bytes

    //Load the game. Open file, convert to data then return data
    public static PlayerData LoadGame()
    {
        string path = Application.persistentDataPath + "/playerData.golf";
        if (File.Exists(path))
        {
            //Set binary formatter and FileStream
            BinaryFormatter formatter = new BinaryFormatter();

            //Open the file as bytes
            byte[] bytes = File.ReadAllBytes(path);

            //Copy first 16 bytes from File and copy into readHash
            byte[] readHash = new byte[16];
            System.Buffer.BlockCopy(bytes, 0, readHash, 0, readHash.Length);
            var readHashString = System.BitConverter.ToString(readHash).Replace("-", "");
            Debug.Log("Hash from inside the save file is = " + System.BitConverter.ToString(readHash).Replace("-", ""));

            //Copy rest of File into readRest
            byte[] readRest = new byte[bytes.Length - readHash.Length];
            System.Buffer.BlockCopy(bytes, readHash.Length, readRest, 0, readRest.Length);

            //Get checksum from readRest
            var fileHash = GetMD5Checksum(readRest);
            var fileHashString = System.BitConverter.ToString(fileHash).Replace("-", "");
            Debug.Log("Hash for Loaded File is = " + System.BitConverter.ToString(fileHash).Replace("-", ""));

            //Compare checksums
            if (fileHashString == readHashString)
            {
                //If valid, load game normally
                //Take bytes into stream, then deserialise it as PlayerData
                using (MemoryStream memStream = new MemoryStream(readRest))
                {
                    PlayerData data = (PlayerData)formatter.Deserialize(memStream);
                    Debug.Log("Data loaded successfully!");
                    return data;
                }
                
            } else
            {
                //If invalid, don't do anything and return null to force new save file
                Debug.Log("File Checksum invalid. Creating new Save File");
            }

            return null;

        } else
        {
            Debug.Log("Oopsie woopsie, file doesn't exist");
            return null;
        }
    }

    //Get a cryptographically generated set of bytes that correspond to the data inside the save file
    //Ensures data hasn't been majorly been corrupted or tampered with
    public static byte[] GetMD5Checksum(byte[] stream)
    {
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            var hash = md5.ComputeHash(stream);
            return hash;
            //return System.BitConverter.ToString(hash).Replace("-", "");
        }
    }
}
