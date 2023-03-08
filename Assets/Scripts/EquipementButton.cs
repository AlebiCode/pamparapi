using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipementButton : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private Button button;

    public Item Item => item;
    public Button Button => button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }
}
