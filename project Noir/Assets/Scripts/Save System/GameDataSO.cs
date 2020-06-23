using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variable ScriptableObjects/GameData")]
public class GameDataSO : ScriptableObject
{
    public GameData data;

    /// <summary>
    /// Saves GameData from this Scriptable object to device drive
    /// </summary>
    public void SaveData()
    {
        SaveSystem.SaveGameState(this);
    }

    /// <summary>
    /// Tries to load GameData from device drive to this Scriptable Object
    /// </summary>
    public void LoadData()
    {
        data = SaveSystem.LoadGameState();
    }

    /// <summary>
    /// Tries to load GameData from device drive to this Scriptable Object
    /// If csnnot access the file - creates new one instead
    /// </summary>
    public void PreLoadData()
    {
        data = SaveSystem.PreLoadGameState(this);
    }


    public void ResetData()
    {
        data = new GameData();
        SaveSystem.SaveGameState(this);
    }
}
