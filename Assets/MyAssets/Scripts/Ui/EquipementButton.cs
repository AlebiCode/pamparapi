using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipementButton : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private Image itemImage;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Text itemDisplayedName, itemPrice;

    private Button button;

    //-------------------------------------

    public Item Item => item;
    public Button Button => button;

    private void Awake()
    {
        button = GetComponent<Button>();
        //SetItem(item, false);   //TEMPORANEO!!
    }

    public void SetItem(Item item, bool isOwned = false)
    {
        this.item = item;

        itemImage.sprite = item.MiniatureSprite != null ? item.MiniatureSprite : item.Sprite;
        itemDisplayedName.text = item.name;
        if(isOwned)
            itemPrice.enabled = false;
        else
            itemPrice.text = "£" + item.Cost;
    }

    public void SetItemGraphicAsOwned()
    {
        buttonImage.color = Color.yellow;
        itemPrice.enabled = false;
    }

    public void SetButtonEquippedGraphic(bool isEquipped)
    {
        //setta l'estetica. E' IMPLICITO CHE QUESTA OPERAZIONE VIENE EFFETTUATA SU UN BOTTONE DI UN OGGETTO CHE POSSIEDO.
        if (isEquipped)
        {
            //l'oggetto è equipaggiato
            buttonImage.color = Color.green;
        }
        else
        {
            //l'oggetto non è equipaggiato
            buttonImage.color = Color.yellow;
        }
    }

    public void OnPress()
    {
        Equipement.instance.ItemButtonPress(this);
    }
}
