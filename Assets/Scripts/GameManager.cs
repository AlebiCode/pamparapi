using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private const string ownedItemsPath = "/Json/OwnedItems.json";

    private Inventory myInventory;

    [SerializeField] private TMP_Text moneyText;

    [SerializeField] private float fullness = 1, love = 1, hygene = 1;
    //[SerializeField] private float maxFullness, maxLove, maxHygene;
    [SerializeField] private float softCurrency, hardCurrency;

    //----------------------------------------------------
    public Inventory MyInventory { set { myInventory = value; } get { return myInventory; } }
    public float Fullness { set { fullness = value; UiManager.instance.UpdateFullnessBar(); } get { return fullness; } }
    public float Love { set { love = value; UiManager.instance.UpdateLoveBar(); } get { return love; } }
    public float Hygene { set { hygene = value; UiManager.instance.UpdateHygeneBar(); } get { return hygene; } }
    //----------------------------------------------------

    public float SoftCurrency {
        set {
            softCurrency = value;
            moneyText.text = "Money " + softCurrency;
        }
        get { return softCurrency; } }
    public float HardCurrency { set { hardCurrency = value; } get { return hardCurrency; } }

    private void Awake()
    {
        if(!instance) instance = this; else Destroy(this);

        LoadOwnedItemsFromJson();

        if (Application.platform == RuntimePlatform.Android)
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
        else
            Application.targetFrameRate = 120;
    }

    private void Update()
    {
        //DEBUG!!! FIX!!
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveInventoryToJson(myInventory);
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            LoadOwnedItemsFromJson();
        }
    }

    #region Saving

    public void LoadOwnedItemsFromJson()
    {
        try
        {
            string path = Application.dataPath + ownedItemsPath;
            StreamReader reader = new StreamReader(path);
            Inventory inv = JsonUtility.FromJson<Inventory>(reader.ReadToEnd());
            if(inv != null)
                myInventory = inv;
            else
                myInventory = new Inventory();
        }
        catch
        {
            Debug.LogWarning("Could not load save file.");
            myInventory = new Inventory();
            SaveInventoryToJson(myInventory);
        }
    }

    public static void SaveInventoryToJson(Inventory inventory)
    {
        string json = JsonUtility.ToJson(inventory);
        string path = Application.dataPath + ownedItemsPath;
        File.WriteAllText(path, json);
    }


    #endregion

}
