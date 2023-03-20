using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemName", menuName = "ScriptableObjects/Equippable Item", order = 1)]
public class Item : ScriptableObject
{
    [Range(0, 31)] [SerializeField] int itemID;
    [SerializeField] EquipementTypeEnum equipSlot;
    [SerializeField] List<EquipementTypeEnum> overrideSlots = new List<EquipementTypeEnum>();
    [SerializeField] float cost;
    [SerializeField] Sprite sprite;

    public int ItemID => itemID;
    public EquipementTypeEnum EquipSlot => equipSlot;
    public List<EquipementTypeEnum> OverrideSlots => overrideSlots;
    public float Cost => cost;
    public Sprite Sprite => sprite;

}

