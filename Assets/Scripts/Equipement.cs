using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Equipement : MonoBehaviour
{
    public static Equipement instance;

    public EquipementSlot[] equipementSlots = new EquipementSlot[(int)EquipementTypeEnum.ENUM_LENGHT];

    [SerializeField] private EquipementButton buttonPrefab;
    //[SerializeField] private Transform buttonsParent;
    [SerializeField] private Transform[] buttonsParents;
    //[SerializeField] private Transform buttonsSeparator;

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
        //InitializeInventory();
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
        return output;
    }

    bool IsItemEquipped(Item item)
    {
        return item == equipementSlots[(int)item.EquipSlot].item;
    }

    #endregion

    #region INIT
    /*
    void InitializeInventory()
    {
        //Inizializza
        GameManager.instance.MyInventory = new int[(int)EquipementTypeEnum.ENUM_LENGHT];
        for(int i =0 ; i<ownedItems.Length; i++)
            ownedItems[i]= 0;
        //Load here!!!
        //FIX!!!
    }*/
    private void InitializeButtons()
    {
        Item[] allItems = GetButtonsInfoFromScriptables();
        Debug.Log("I loaded " + allItems.Length + " items!");
        List<EquipementButton> buttons = new List<EquipementButton>();
        foreach (Item item in allItems)
        {
            EquipementButton eb = Instantiate(buttonPrefab, buttonsParents[(int)item.EquipSlot]);
            eb.transform.SetSiblingIndex(item.ItemID + 1);
            eb.SetItem(item);
            buttons.Add(eb);
        }

        //Controllo se gli oggetti sono posseduti o meno
        for (int i=0; i < buttons.Count; i++)
        {
            if (IsItemOwned(buttons[i].Item))
            {
                Debug.Log("AT START I OWN " + buttons[i].Item.name);
                //ITEM IS OWNED
                //metto il bottone prima del separatore.
                SetButtonAsOwned(buttons[i]);
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
        return Resources.LoadAll<Item>(path);
    }

    #endregion

    #region EQUIP LOGIC

    public void EquipItem(Item item)
    {
        UnequipItem(item.EquipSlot);
        equipementSlots[(int)item.EquipSlot].spriteRenderer.sprite = item.Sprite;
        equipementSlots[(int)item.EquipSlot].spriteRenderer.enabled = true;
        foreach (EquipementTypeEnum equipType in item.OverrideSlots)
        {
            equipementSlots[(int)equipType].spriteRenderer.enabled = false;
        }

        equipementSlots[(int)item.EquipSlot].item = item;
    }
    public void UnequipItem(EquipementTypeEnum equipSlot)
    {
        Item itemToUnequip = equipementSlots[(int)equipSlot].item;
        if (itemToUnequip)
        {
            foreach (EquipementTypeEnum equipType in itemToUnequip.OverrideSlots)
            {
                equipementSlots[(int)equipType].spriteRenderer.enabled = true;
            }
            equipementSlots[(int)equipSlot].spriteRenderer.enabled = false;
            equipementSlots[(int)equipSlot].spriteRenderer.sprite = null;

            equipementSlots[(int)equipSlot].item = null;
        }
    }

    public void UnequipItem(Item item)
    {
        if (IsItemEquipped(item))
        {
            foreach (EquipementTypeEnum equipType in item.OverrideSlots)
            {
                equipementSlots[(int)equipType].spriteRenderer.enabled = true;
            }
            equipementSlots[(int)item.EquipSlot].spriteRenderer.enabled = false;
            equipementSlots[(int)item.EquipSlot].spriteRenderer.sprite = null;

            equipementSlots[(int)item.EquipSlot].item = null;
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
                UnequipItem(equipementButton.Item.EquipSlot);
                equipementButton.SetButtonEquippedGraphic(false);
            }
            else
            {
                //equip item
                EquipItem(equipementButton.Item);
                equipementButton.SetButtonEquippedGraphic(true);
            }
        }
        else
        {
            //non possiedo questo oggetto. Vuoi comprarlo?
            if (equipementButton.Item.Cost <= GameManager.instance.SoftCurrency)
            {
                ConfirmPurchaseWindow_TargetButton = equipementButton;
                ConfirmPurchaseWindow.SetActive(true);
            }
            else
            {
                Debug.Log("Non hai abbastanza denaro!");
            }
        }
    }

    public void ConfirmPurchaseButton()
    {
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
        public Item item;
        public SpriteRenderer spriteRenderer;
    }
}



public enum EquipementTypeEnum
{
    HAT,
    GLASSES,

    ENUM_LENGHT
}
