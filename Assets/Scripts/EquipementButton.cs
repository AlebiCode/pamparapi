using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipementButton : MonoBehaviour
{
    [SerializeField] private Item item;
    private Button button;

    public Item Item => item;
    public Button Button => button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPress()
    {
        Equipement.instance.ItemButtonPress(this);
    }
}
