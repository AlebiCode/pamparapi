using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Equipement : MonoBehaviour
{
    public static Equipement instance;

    public EquipementSlot[] equipementSlots;// = new EquipementSlot[(int)EquipementTypeEnum.ENUM_LENGHT];

    [SerializeField] private EquipementButton buttonPrefab;
    [SerializeField] private Transform[] buttonsParents;

    [SerializeField] private GameObject ConfirmPurchaseWindow;
    private EquipementButton ConfirmPurchaseWindow_TargetButton;

    //-------------------------------------------------

    int[] OwnedItems => GameManager.instance.MyInventory.ownedItems;

    //-------------------------------------------------

    #region UnityMethods

    private void Awake()
    {
        if (!instance) instance = this; else Destroy(this);
    }
    private void Start()
    {
        InitializeButtons();
    }

    #endregion

    #region CONTROL LOGIC

    bool IsItemOwned(Item item)
    {
        return ((1 << item.ItemID) & OwnedItems[(int)item.EquipSlot]) != 0;
    }

    int CountItemsOwnedOfType(EquipementTypeEnum slot)
    {
        int output = 0;
        for (int i = 0; i < sizeof(int); i++)
            if (((1 << i) & OwnedItems[(int)slot]) != 0)
                output++;
        return output;
    }

    int GetItemShopOwnedIndex(Item item)
    {
        if(!IsItemOwned(item))
            return -1;
        int output = 0;
        for (int i = 0; i < item.ItemID; i++)
        {
            if (((1 << i) & OwnedItems[(int)item.EquipSlot]) != 0)
                output++;
        }
        Debug.LogWarning("Item " + item.name + " is at position " + output);;
        return output;
    }

    bool IsItemEquipped(Item item)
    {
        EquipementButton eq = equipementSlots[(int)item.EquipSlot].equipementButton;
        if(!eq)
            return false;
        return item == eq.Item;
    }

    Item GetItemAtSlot(EquipementTypeEnum equipementTypeEnum)
    {
        return equipementSlots[(int)equipementTypeEnum].equipementButton.Item;
    }

    #endregion

    #region INIT

    private void InitializeButtons()
    {
        Item[] allItems = GetButtonsInfoFromScriptables();
        //Debug.Log("I loaded " + allItems.Length + " items!");
        List<EquipementButton> buttons = new List<EquipementButton>();
        foreach (Item item in allItems)
        {
            EquipementButton eb = Instantiate(buttonPrefab, buttonsParents[(int)item.EquipSlot]);
            eb.transform.SetSiblingIndex(item.ItemID + 1);
            eb.SetItem(item);
            buttons.Add(eb);
        }
        int ownedCounter = 0;
        //Controllo se gli oggetti sono posseduti o meno
        for (int i=0; i < buttons.Count; i++)
        {
            if (IsItemOwned(buttons[i].Item))
            {
                //ITEM IS OWNED
                //Debug.Log("AT START I OWN " + buttons[i].Item.name);
                //metto il bottone prima del separatore.
                buttons[i].transform.SetSiblingIndex(ownedCounter);

                buttons[i].SetItemGraphicAsOwned();
                ownedCounter++;
            }
            else
            {
                //ITEM IS NOT OWNED
            }
        }
    }
        
    private Item[] GetButtonsInfoFromScriptables()
    {
        string path = "ScriptableObjects";
        Item[] outPut = Resources.LoadAll<Item>(path);
        Array.Sort(outPut, delegate (Item x, Item y) { return x.ItemID.CompareTo(y.ItemID); });
        return outPut;
    }

    #endregion

    #region EQUIP LOGIC

    public void EquipItem(EquipementButton equipementButton)
    {
        equipementButton.SetButtonEquippedGraphic(true);

        UnequipItem(equipementButton.Item.EquipSlot);
        equipementSlots[(int)equipementButton.Item.EquipSlot].spriteRenderer.sprite = equipementButton.Item.Sprite;
        equipementSlots[(int)equipementButton.Item.EquipSlot].spriteRenderer.enabled = true;
        foreach (EquipementTypeEnum equipType in equipementButton.Item.OverrideSlots)
        {
            equipementSlots[(int)equipType].spriteRenderer.enabled = false;
        }

        equipementSlots[(int)equipementButton.Item.EquipSlot].equipementButton = equipementButton;
    }
    public void UnequipItem(EquipementTypeEnum equipSlot)
    {

        EquipementButton equipementButton = equipementSlots[(int)equipSlot].equipementButton;
        if (!equipementButton)
            return;
        equipementButton.SetButtonEquippedGraphic(false);
        if (equipementButton.Item)
        {
            foreach (EquipementTypeEnum equipType in equipementButton.Item.OverrideSlots)
            {
                equipementSlots[(int)equipType].spriteRenderer.enabled = true;
            }
            equipementSlots[(int)equipSlot].spriteRenderer.enabled = false;
            equipementSlots[(int)equipSlot].spriteRenderer.sprite = null;

            equipementSlots[(int)equipSlot].equipementButton = null;
        }
    }

    public void UnequipItem(EquipementButton equipementButton)
    {
        equipementButton.SetButtonEquippedGraphic(false);
        Item item = equipementButton.Item;
        if (IsItemEquipped(item))
        {
            foreach (EquipementTypeEnum equipType in item.OverrideSlots)
            {
                equipementSlots[(int)equipType].spriteRenderer.enabled = true;
            }
            equipementSlots[(int)item.EquipSlot].spriteRenderer.enabled = false;
            equipementSlots[(int)item.EquipSlot].spriteRenderer.sprite = null;

            equipementSlots[(int)item.EquipSlot].equipementButton = null;
        }
    }

    #endregion

    #region SHOP LOGIC

    void AddItemToOwned(Item item)
    {
        if (!IsItemOwned(item))
        {
            OwnedItems[(int)item.EquipSlot] += (1 << item.ItemID);
            GameManager.instance.SaveDataToJson();
        }
    }

    #endregion

    #region INTERACTION LOGIC

    public void ItemButtonPress(EquipementButton equipementButton)
    {
        if (IsItemOwned(equipementButton.Item))
        {
            //possiedo questo oggetto
            //Debug.Log("I OWN THIS!!");
            if (IsItemEquipped(equipementButton.Item))
            {
                //unequip item
                AudioManager.instance.PlayGenericInputSound();
                UnequipItem(equipementButton.Item.EquipSlot);
            }
            else
            {
                //equip item
                AudioManager.instance.PlayGenericInputSound();
                EquipItem(equipementButton);
            }
        }
        else
        {
            //non possiedo questo oggetto. Vuoi comprarlo?
            if (equipementButton.Item.Cost <= GameManager.instance.SoftCurrency)
            {
                AudioManager.instance.PlayGenericInputSound();
                ConfirmPurchaseWindow_TargetButton = equipementButton;
                ConfirmPurchaseWindow.SetActive(true);
            }
            else
            {
                AudioManager.instance.PlayGenericInputSound();
                Debug.Log("Non hai abbastanza denaro!");
            }
        }
    }

    public void ConfirmPurchaseButton()
    {
        AudioManager.instance.PlayPurchaseSound();
        GameManager.instance.SoftCurrency -= ConfirmPurchaseWindow_TargetButton.Item.Cost;
        AddItemToOwned(ConfirmPurchaseWindow_TargetButton.Item);
        SetButtonAsOwned(ConfirmPurchaseWindow_TargetButton);
        ConfirmPurchaseWindow_TargetButton = null;
    }

    #endregion

    #region UI LOGIC

    void SetButtonAsOwned(EquipementButton equipButton)
    {
        //Debug.Log("Index of " + equipButton.Item.name + ": " + GetItemShopOwnedIndex(equipButton.Item));
        equipButton.transform.SetSiblingIndex(GetItemShopOwnedIndex(equipButton.Item));

        equipButton.SetItemGraphicAsOwned();
    }


    #endregion

    //-------------------------------

    [System.Serializable]
    public struct EquipementSlot
    {
        public EquipementButton equipementButton;
        public SpriteRenderer spriteRenderer;
    }
}



public enum EquipementTypeEnum
{
    HAT,
    GLASSES,
    SHIRTS,
    SCARFS,
    ACCESSORIES,

    ENUM_LENGHT
}
