using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    [SerializeField] int itemID;
    [SerializeField] EquipementTypeEnum equipSlot;
    [SerializeField] float cost;
    [SerializeField] Sprite sprite;
}

