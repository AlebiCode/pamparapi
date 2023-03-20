using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CustomTools :  EditorWindow
{
    [MenuItem("CustomTools/Data/OwnedItems/ResetOwnedItems")]
    public static void ResetInventory()
    {
        if (Application.isPlaying)
        {
            GameManager.instance.MyInventory.ownedItems = new int[(int)EquipementTypeEnum.ENUM_LENGHT];
            GameManager.SaveInventoryToJson(GameManager.instance.MyInventory);
        }
        else
            GameManager.SaveInventoryToJson(new Inventory());

    }
    [MenuItem("CustomTools/Data/OwnedItems/SaveOwnedItems")]
    public static void SaveOwnedItems()
    {
        if (Application.isPlaying)
            GameManager.SaveInventoryToJson(GameManager.instance.MyInventory);
        else
            Debug.LogWarning("Game is not running, inventory not initialized!!");
    }
    [MenuItem("CustomTools/Data/OwnedItems/LoadOwnedItems")]
    public static void LoadOwnedItems()
    {
        if (Application.isPlaying)
            GameManager.instance.LoadOwnedItemsFromJson();
        else
            Debug.LogWarning("Game is not running, inventory not initialized!!");
    }
}
