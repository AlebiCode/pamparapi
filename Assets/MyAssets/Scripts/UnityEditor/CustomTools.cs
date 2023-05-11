using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class CustomTools : EditorWindow
{
    [MenuItem("CustomTools/Data/ResetSaveFile")]
    public static void ResetSaveFile()
    {
        GameManager.ResetData();
    }
    
    [MenuItem("CustomTools/Data/SaveData")]
    public static void SaveData()
    {
        if (Application.isPlaying)
            GameManager.instance.SaveDataToJson();
        else
            Debug.LogWarning("Game is not running, inventory not initialized!!");
    }
    
    [MenuItem("CustomTools/Data/LoadData")]
    public static void LoadData()
    {
        if (Application.isPlaying)
            GameManager.instance.LoadDataFromJson();
        else
            Debug.LogWarning("Game is not running, inventory not initialized!!");
    }
}
#endif