using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipement : MonoBehaviour
{
    public SpriteRenderer hatSlot, occhialiSlot;

    private int[] inventory;

    //-------------------------------------------------
    //public int OwnedHats { set { ownedHats = value; } get { return ownedHats; } }

    //-------------------------------------------------

    private void Start()
    {
        LoadInventory();
    }

    void LoadInventory()
    {
        //Inizializza
        inventory = new int[(int)EquipementTypeEnum.ENUM_LENGHT];
        //Load here!!!
        //FIX!!!
    }
}

public enum EquipementTypeEnum
{
    GLASSES,
    HAT,

    ENUM_LENGHT
}
