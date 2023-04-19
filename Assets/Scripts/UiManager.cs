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
    private CanvasScaler canvasScaler;

    [SerializeField] RectTransform fullnessBar;
    [SerializeField] RectTransform loveBar;
    [SerializeField] RectTransform hygeneBar;

    [HideInInspector]public bool updateBars;

    [SerializeField] RectTransform crossWordPanel;
    private Vector3 crossWordWindow_onScreenPosition;
    private Vector3 crossWordWindow_offScreenPosition;
    [SerializeField] RectTransform fullTastieraPanel;
    private Vector3 tastiera_onScreenPosition;
    private Vector3 tastiera_offScreenPosition;
    [SerializeField] RectTransform tastieraBackground;
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
        canvasScaler = canvas.GetComponent<CanvasScaler>();
        UiResizer();
        GetAnimationPositions();
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
        Vector2 referenceRes = canvasScaler.referenceResolution;
        Vector2 res = new Vector2(canvasRectTransform.rect.width, canvasRectTransform.rect.height);
        Vector2 scaleMult = new Vector2(res.x / referenceRes.x, res.y / referenceRes.y);
        MultRectTransformWidthAndHeight(crossWordPanel, scaleMult);

        spriteMask.transform.localScale = new Vector3(tastieraBackground.rect.width, tastieraBackground.rect.height, 1);
        spriteMask.transform.position = fullTastieraPanel.transform.position + new Vector3(0, tastieraBackground.rect.height * canvas.transform.lossyScale.y / 2, 0);
    }

    private void GetAnimationPositions()
    {
        Vector3 heihtContainer; //la x non è corretta. Serve solo la y
        RectTransformUtility.ScreenPointToWorldPointInRectangle(crossWordPanel, new Vector2(0, Screen.currentResolution.height), Camera.main, out heihtContainer);
        crossWordWindow_offScreenPosition = new Vector3(canvas.transform.position.x, heihtContainer.y, canvas.transform.position.z);
        crossWordWindow_onScreenPosition = crossWordWindow_offScreenPosition - new Vector3(0, crossWordPanel.rect.height * canvas.transform.lossyScale.y, 0);
        crossWordPanel.transform.position = crossWordWindow_offScreenPosition;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(fullTastieraPanel, new Vector2(0, 0), Camera.main, out heihtContainer);
        tastiera_onScreenPosition = new Vector3(canvas.transform.position.x, heihtContainer.y, canvas.transform.position.z);
        tastiera_offScreenPosition = tastiera_onScreenPosition - new Vector3(0, fullTastieraPanel.rect.height * canvas.transform.lossyScale.y, 0);
        fullTastieraPanel.transform.position = tastiera_offScreenPosition;
    }
    public void AnimateCrosswordPanels(bool onScreen)
    {
        if (onScreen)
        {
            crossWordPanel.transform.position = crossWordWindow_onScreenPosition;
            fullTastieraPanel.transform.position = tastiera_onScreenPosition;
        }
        else
        {
            crossWordPanel.transform.position = crossWordWindow_offScreenPosition;
            fullTastieraPanel.transform.position = tastiera_offScreenPosition;
        }
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
