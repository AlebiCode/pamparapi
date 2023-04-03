using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    [SerializeField] Canvas canvas;

    [SerializeField] RectTransform fullnessBar;
    [SerializeField] RectTransform loveBar;
    [SerializeField] RectTransform hygeneBar;

    public bool updateBars;

    [SerializeField] RectTransform crosswordSpritemaskSize;
    [SerializeField] SpriteMask spriteMask;
    
    [SerializeField] GameObject activeShopTab;
    
    //-----------------------------------------
    public Canvas Canvas => canvas;
    //-----------------------------------------


    // Start is called before the first frame update
    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(this);
    }
    void Start()
    {
        spriteMask.transform.position = crosswordSpritemaskSize.transform.position;
        spriteMask.transform.localScale = new Vector3(crosswordSpritemaskSize.rect.width, crosswordSpritemaskSize.rect.height, 1);
    }

    private void LateUpdate()
    {
        if (updateBars)
            UpdateBars();
    }

    private void UpdateBars()
    {
        updateBars = false;
        loveBar.transform.localScale = new Vector3(GameManager.instance.Love, 1, 1);
    }
    public void SelectShopTab(GameObject tab)
    {
        if(activeShopTab) activeShopTab.SetActive(false);
        activeShopTab = tab;
        tab.SetActive(true);
    }

}
