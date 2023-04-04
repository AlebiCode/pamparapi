using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    [SerializeField] Canvas canvas;

    [SerializeField] RectTransform fullnessBar;
    [SerializeField] RectTransform loveBar;
    [SerializeField] RectTransform hygeneBar;

    public bool updateBars;

    [SerializeField] RectTransform crossWordWindow;
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
        UiResizer();
        
    }

    private void LateUpdate()
    {
        if (updateBars)
            UpdateBars();
    }

    private void MultRectTransformWidthAndHeight(RectTransform rectTransform, Vector2 values)
    {
        rectTransform.rect.Set(rectTransform.rect.x, rectTransform.rect.y, rectTransform.rect.width * values.x, rectTransform.rect.height * values.y);
    }

    private void UiResizer()
    {
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        Vector2 referenceRes = canvas.GetComponent<CanvasScaler>().referenceResolution;
        Vector2 res = new Vector2(canvasRectTransform.rect.width, canvasRectTransform.rect.height);
        Vector2 scaleMult = new Vector2(res.x / referenceRes.x, res.y / referenceRes.y);
        Debug.Log("REFERENCE RES:" + referenceRes +  "   RES:" + res.x + "," + res.y + "    SCALE MULT:" + scaleMult + "    CONVAS POS:" + canvas.transform.position + "    PANEL POS:" + crossWordWindow.transform.position);

        MultRectTransformWidthAndHeight(crossWordWindow, scaleMult);
        //crossWordWindow.position = canvas.transform.position + Vector3.up * canvasRectTransform.rect.height;
        crossWordWindow.transform.position = new Vector3(canvas.transform.position.x, canvasRectTransform.offsetMax.y, canvas.transform.position.z); ;
        
        spriteMask.transform.position = crosswordSpritemaskSize.transform.position;
        spriteMask.transform.localScale = new Vector3(crosswordSpritemaskSize.rect.width, crosswordSpritemaskSize.rect.height, 1);
    }

    private void UpdateBars()
    {
        updateBars = false;
        if (loveBar.transform.localScale.x != GameManager.instance.Love)
        {
            loveBar.transform.localScale = Vector3.MoveTowards(loveBar.transform.localScale, new Vector3(GameManager.instance.Love, 1, 1), Time.deltaTime);
            updateBars = true;
        }
    }
    public void SelectShopTab(GameObject tab)
    {
        if(activeShopTab) activeShopTab.SetActive(false);
        activeShopTab = tab;
        tab.SetActive(true);
    }

}
