using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    private const string savesFolder = "/Saves";
    private const string saveDataPath = savesFolder + "/SaveData.json";
    [SerializeField] private TMP_Text moneyText;

    private Inventory myInventory;
    [SerializeField] private float fullness = 1, love = 1, hygene = 1;
    //[SerializeField] private float maxFullness, maxLove, maxHygene;
    [SerializeField] private int softCurrency, hardCurrency;

    //----------------------------------------------------
    public Inventory MyInventory { set { myInventory = value; } get { return myInventory; } }
    public float Fullness { set { fullness = Mathf.Clamp(value, 0, 1); UiManager.instance.updateBars = true; } get { return fullness; } }
    public float Love { set { love = Mathf.Clamp(value, 0, 1); UiManager.instance.updateBars = true; } get { return love; } }
    public float Hygene { set { hygene = Mathf.Clamp(value, 0, 1); UiManager.instance.updateBars = true; } get { return hygene; } }
    //----------------------------------------------------

    public int SoftCurrency {
        set {
            softCurrency = value;
            moneyText.text = "Money " + softCurrency;
        }
        get { return softCurrency; } }
    public int HardCurrency { set { hardCurrency = value; } get { return hardCurrency; } }

    private void Awake()
    {
        if(!instance) instance = this; else Destroy(this);


        if (Application.platform == RuntimePlatform.Android)
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
        else
            Application.targetFrameRate = 120;
    }

    private void Start()
    {
        LoadDataFromJson();

        //Debug.LogWarning("DATA: " + myInventory);
    }

    void OnApplicationQuit()
    {
        SaveDataToJson();
        Debug.Log("Application ending after " + Time.time + " seconds");
    }

    #region Saving

    public void LoadDataFromJson(bool recursiveParent = true)
    {
        try
        {
            string path = Application.persistentDataPath + saveDataPath;
            Debug.Log(path);
            StreamReader reader = new StreamReader(path);
            SaveData saveData = JsonUtility.FromJson<SaveData>(reader.ReadToEnd());
            if (saveData != null)
            {
                myInventory = saveData.inventory;
                Fullness = saveData.fullness;
                Love = saveData.love;
                Hygene = saveData.hygene;
                SoftCurrency = saveData.softCurrency;
            }
            else
                throw new System.Exception();
            reader.Close();
        }
        catch
        {
            if (recursiveParent)
            {
                Debug.LogWarning("Could not load save file. Creating new save file.");
                ResetData();
                LoadDataFromJson(false);
            }
            else
                Debug.LogError("Could not create new save file.");
        }
    }

    public void SaveDataToJson()
    {
        SaveData saveData = new SaveData();
        saveData.inventory = myInventory;
        saveData.fullness = fullness; saveData.love = love; saveData.hygene = hygene;
        saveData.softCurrency = softCurrency;
        if (!Directory.Exists(Application.persistentDataPath + savesFolder))
            Directory.CreateDirectory(Application.persistentDataPath + savesFolder);
        string json = JsonUtility.ToJson(saveData);
        string path = Application.persistentDataPath + saveDataPath;
        File.WriteAllText(path, json);
    }

    public static void ResetData()
    {
        SaveData saveData = new SaveData();
        saveData.inventory = new Inventory();
        saveData.fullness = saveData.love = saveData.hygene = 0.5f;
        saveData.softCurrency = 100;
        if (!Directory.Exists(Application.persistentDataPath + savesFolder))
            Directory.CreateDirectory(Application.persistentDataPath + savesFolder);
        string json = JsonUtility.ToJson(saveData);
        string path = Application.persistentDataPath + saveDataPath;
        File.WriteAllText(path, json);
    }

    #endregion
}

[System.Serializable]
public class SaveData
{
    public Inventory inventory;
    public float fullness, love, hygene;
    public int softCurrency;
}