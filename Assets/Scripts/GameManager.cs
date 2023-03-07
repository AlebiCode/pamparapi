using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private TMP_Text moneyText;

    [SerializeField] private float fullness, maxFullness, love, maxLove, hygene, maxHygene;
    [SerializeField] private float softCurrency, hardCurrency;


    //----------------------------------------------------

    public float SoftCurrency {
        set {
            softCurrency = value;
            moneyText.text = "Money " + softCurrency;
        }
        get { return softCurrency; } }
    public float HardCurrency { set { hardCurrency = value; } get { return hardCurrency; } }

    private void Awake()
    {
        if(!instance) instance = this; else Destroy(this);

        if (Application.platform == RuntimePlatform.Android)
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
        else
            Application.targetFrameRate = 120;
    }
}
