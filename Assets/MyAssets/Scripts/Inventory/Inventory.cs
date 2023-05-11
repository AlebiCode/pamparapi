using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    public int[] ownedItems = new int[(int)EquipementTypeEnum.ENUM_LENGHT];
}
