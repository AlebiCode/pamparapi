using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipement : MonoBehaviour
{
    public static Equipement instance;

    private int[] ownedItems;
    public EquipementSlot[] equipementSlots = new EquipementSlot[(int)EquipementTypeEnum.ENUM_LENGHT];

    [SerializeField] private Transform buttonsParent;
    [SerializeField] private Transform buttonsSeparator;

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
        EquipementButton[] buttons = new EquipementButton[buttonsParent.childCount - 1];
        int i = 0;
        while(i < buttonsParent.childCount - 1)
        {
            EquipementButton temp = buttonsParent.GetChild(i).GetComponent<EquipementButton>();
            if (temp)
            {
                buttons[i] = temp;
                i++;
            }
        }

        //Controllo se gli oggetti sono posseduti o meno
        for (i=0; i < buttons.Length; i++)
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

    #endregion

    #region SHOP LOGIC

    void AddItemToOwned(EquipementButton equipButton)
    {
        if (!IsItemOwned(equipButton.Item))
        {
            ownedItems[(int)equipButton.Item.EquipSlot] += (1 << equipButton.Item.ItemID);
            SetButtonAsOwned(equipButton);
        }
    }

    #endregion

    void SetButtonAsOwned(EquipementButton equipButton)
    {
        equipButton.transform.SetSiblingIndex(buttonsSeparator.GetSiblingIndex());
    }

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
    GLASSES,
    HAT,

    ENUM_LENGHT
}
