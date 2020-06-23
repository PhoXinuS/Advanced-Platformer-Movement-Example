using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[ExecuteAlways]
public static class SaveSystem
{
    /// <summary>
    /// Saves GameData
    /// </summary>
    /// <param name="gameDataSO">ScriptableObject with GameData variable to which this data will be loaded</param>
    public static void SaveGameState(GameDataSO gameDataSO)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/greetings.miner";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, gameDataSO.data);
        stream.Close();
    }

    /// <summary>
    /// Used to load GameData
    /// </summary>
    /// <returns> Game Data loaded from device drive </returns>
    public static GameData LoadGameState ()
    {
        string path = Application.persistentDataPath + "/greetings.miner";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Sir! Save file not found in: " + path);
            return null;
        }
    }

    /// <summary>
    /// Used to load GameData when there is no certainty that the data exists
    /// </summary>
    /// <param name="gameDataSO">ScriptableObject with GameData variable to which this data will be loaded</param>
    /// <returns> Game Data loaded from device drive
    /// or new GameData created in memory if cann't access data saved previously</returns>
    public static GameData PreLoadGameState(GameDataSO gameDataSO)
    {
        string path = Application.persistentDataPath + "/greetings.miner";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;
        }
        else
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            formatter.Serialize(stream, gameDataSO.data);
            stream.Close();

            return gameDataSO.data;
        }
    }
}
