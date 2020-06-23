using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameDataSO))]
public class DataButtons : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("PreLoad Data"))
        {
            (target as GameDataSO)?.PreLoadData();
        }
        
        if (GUILayout.Button("Reset Data"))
        {
            (target as GameDataSO)?.ResetData();
        }
        
        if (GUILayout.Button("Save Data"))
        {
            (target as GameDataSO)?.SaveData();
        }
    }
}
