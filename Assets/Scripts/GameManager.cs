using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    private const string saveDataPath = "/Json/SaveData.json";
    [SerializeField] private TMP_Text moneyText;

    private Inventory myInventory;
    [SerializeField] private float fullness = 1, love = 1, hygene = 1;
    //[SerializeField] private float maxFullness, maxLove, maxHygene;
    [SerializeField] private int softCurrency, hardCurrency;

    //----------------------------------------------------
    public Inventory MyInventory { set { myInventory = value; } get { return myInventory; } }
    public float Fullness { set { fullness = value; UiManager.instance.UpdateFullnessBar(); } get { return fullness; } }
    public float Love { set { love = value; UiManager.instance.UpdateLoveBar(); } get { return love; } }
    public float Hygene { set { hygene = value; UiManager.instance.UpdateHygeneBar(); } get { return hygene; } }
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

        LoadDataFromJson();

        if (Application.platform == RuntimePlatform.Android)
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
        else
            Application.targetFrameRate = 120;
    }


    #region Saving

    public void LoadDataFromJson()
    {
        try
        {
            string path = Application.dataPath + saveDataPath;
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
        }
        catch
        {
            Debug.LogWarning("Could not load save file.");
        }
    }

    public void SaveDataToJson()
    {
        SaveData saveData = new SaveData();
        saveData.inventory = myInventory;
        saveData.fullness = fullness; saveData.love = love; saveData.hygene = hygene;
        saveData.softCurrency = softCurrency;
        string json = JsonUtility.ToJson(saveData);
        string path = Application.dataPath + saveDataPath;
        File.WriteAllText(path, json);
    }

    public static void ResetData()
    {
        SaveData saveData = new SaveData();
        saveData.inventory = new Inventory();
        string json = JsonUtility.ToJson(saveData);
        string path = Application.dataPath + saveDataPath;
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