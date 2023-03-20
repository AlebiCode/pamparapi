using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FoodName", menuName = "ScriptableObjects/Food", order = 1)]
public class Food : ScriptableObject, IConsumable
{
    [SerializeField] private float fullness;
    public void OnConsume()
    {
        GameManager.instance.Fullness = Mathf.Min(GameManager.instance.Fullness + fullness, 1);
    }
}
