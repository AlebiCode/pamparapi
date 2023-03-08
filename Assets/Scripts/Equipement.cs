using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equipement : MonoBehaviour
{
    public static Equipement instance;

    private int[] ownedItems;
    public EquipementSlot[] equipementSlots = new EquipementSlot[(int)EquipementTypeEnum.ENUM_LENGHT];

    [SerializeField] private Transform buttonsParent;
    [SerializeField] private Transform buttonsSeparator;

    [SerializeField] private GameObject ConfirmPurchaseWindow;
    private EquipementButton ConfirmPurchaseWindow_TargetButton;

    //-------------------------------------------------

    //-------------------------------------------------

    #region UnityMethods

    private void Awake()
    {
        if (!instance) instance = this; else Destroy(this);
    }
    private void Start()
    {
        InitializeInventory();
        InitializeButtons();
    }

    #endregion

    #region CONTROL LOGIC

    bool IsItemOwned(Item item)
    {
        return ((1 << item.ItemID) & ownedItems[(int)item.EquipSlot]) != 0;
    }

    bool IsItemEquipped(Item item)
    {
        return item == equipementSlots[(int)item.EquipSlot].item;
    }

    #endregion

    #region INIT

    void InitializeInventory()
    {
        //Inizializza
        ownedItems = new int[(int)EquipementTypeEnum.ENUM_LENGHT];
        //Load here!!!
        //FIX!!!
    }
    private void InitializeButtons()
    {
        //creo un array contenente tutti i bottoni (escluso il buttonsSeparator!)
        EquipementButton[] buttons = buttonsParent.GetComponentsInChildren<EquipementButton>();

        //Controllo se gli oggetti sono posseduti o meno
        for (int i=0; i < buttons.Length; i++)
        {
            if (IsItemOwned(buttons[i].Item))
            {
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
            ownedItems[(int)item.EquipSlot] += (1 << item.ItemID);
        }
    }

    #endregion

    #region INTERACTION LOGIC

    public void ItemButtonPress(EquipementButton equipementButton)
    {
        if (IsItemOwned(equipementButton.Item))
        {
            //possiedo questo oggetto
            if (IsItemEquipped(equipementButton.Item))
            {
                //unequip item
                UnequipItem(equipementButton.Item.EquipSlot);
                SetButtonEquippedGraphic(equipementButton, false);
            }
            else
            {
                //equip item
                EquipItem(equipementButton.Item);
                SetButtonEquippedGraphic(equipementButton, true);
            }
        }
        else
        {
            //non possiedo questo oggetto. Vuoi comprarlo?
            ConfirmPurchaseWindow_TargetButton = equipementButton;
            ConfirmPurchaseWindow.SetActive(true);
        }
    }

    public void ConfirmPurchaseButton()
    {
        AddItemToOwned(ConfirmPurchaseWindow_TargetButton.Item);
        SetButtonAsOwned(ConfirmPurchaseWindow_TargetButton);
        ConfirmPurchaseWindow_TargetButton = null;
    }

    #endregion

    #region UI LOGIC

    void SetButtonAsOwned(EquipementButton equipButton)
    {
        equipButton.transform.SetSiblingIndex(buttonsSeparator.GetSiblingIndex());

        equipButton.GetComponent<Image>().color = Color.yellow;
    }

    void SetButtonEquippedGraphic(EquipementButton equipButton, bool isEquipped)
    {
        //setta l'estetica. E' IMPLICITO CHE QUESTA OPERAZIONE VIENE EFFETTUATA SU UN BOTTONE DI UN OGGETTO CHE POSSIEDO.
        if (isEquipped)
        {
            //l'oggetto è equipaggiato
            equipButton.GetComponent<Image>().color = Color.green;
        }
        else
        {
            //l'oggetto non è equipaggiato
            equipButton.GetComponent<Image>().color = Color.yellow;
        }
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
