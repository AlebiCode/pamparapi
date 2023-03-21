using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CustomTools :  EditorWindow
{
    [MenuItem("CustomTools/Data/ResetSaveFile")]

    public static void ResetSaveFile()
    {
        if (Application.isPlaying)
        {
            GameManager.instance.MyInventory.ownedItems = new int[(int)EquipementTypeEnum.ENUM_LENGHT];
            GameManager.instance.Fullness = GameManager.instance.Love = GameManager.instance.Hygene = 0;
            GameManager.instance.SaveDataToJson();
        }
        else
        {
            GameManager.ResetData();
        }

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
