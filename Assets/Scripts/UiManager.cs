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

    Vector2Int canvasSizeForAnchoredPositions;
    Vector2 sizeMult;

    [SerializeField] RectTransform crossWordWindow;
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
        StartCoroutine(UiInitialization());
    }

    private void LateUpdate()
    {
        if (updateBars)
            UpdateBars();
    }
    
    private IEnumerator UiInitialization()
    {
        //Debug.Log("Res: " + Screen.currentResolution + "Canvas pixelrect Height" + canvas.pixelRect.height + " MinMax diff " + (canvas.pixelRect.yMax - canvas.pixelRect.yMin));
        yield return new WaitForSeconds(1);  //a quanto pare non tutte le ui sono sistemate dal canvas nel primo frame. Quindi aspetto un frame. Magari non è nemmeno più così lol

        sizeMult = new Vector2(Screen.currentResolution.width / canvasScaler.referenceResolution.x, Screen.currentResolution.height / canvasScaler.referenceResolution.y);
        //Debug.Log(sizeMult);
        //questo se è settato a conservare la larghezza!
        canvasSizeForAnchoredPositions = new Vector2Int(Mathf.RoundToInt(Screen.currentResolution.width / sizeMult.x), Mathf.RoundToInt(Screen.currentResolution.height / sizeMult.x));
        //Debug.Log(canvasSizeForAnchoredPositions);

        CrossWordUi();
    }

    private void CrossWordUi()
    {
        //Debug.Log(crossWordWindow.sizeDelta);
        crossWordWindow.sizeDelta = new Vector2(0, canvasSizeForAnchoredPositions.y * 0.6f);
        crossWordWindow_offScreenPosition = new Vector2(0, 0);
        crossWordWindow_onScreenPosition = crossWordWindow_offScreenPosition - new Vector3(0, crossWordWindow.rect.height);
        crossWordWindow.anchoredPosition = crossWordWindow_offScreenPosition;

        fullTastieraPanel.sizeDelta = new Vector2(0, canvasSizeForAnchoredPositions.y * 0.4f);
        tastiera_onScreenPosition = new Vector2(0,0);
        tastiera_offScreenPosition = tastiera_onScreenPosition - new Vector3(0, fullTastieraPanel.rect.height);
        fullTastieraPanel.anchoredPosition = tastiera_offScreenPosition;

        //setto la spritemask della tastiera (che nasconde parte del pamparapì)
        spriteMask.transform.localScale = new Vector3(tastieraBackground.rect.width, tastieraBackground.rect.height, 1);
        spriteMask.transform.position = fullTastieraPanel.transform.position + new Vector3(0, tastieraBackground.rect.height * canvas.transform.lossyScale.y / 2, 0);
    }

    public void AnimateCrosswordPanels(bool onScreen)
    {
        if (onScreen)
        {
            crossWordWindow.anchoredPosition = crossWordWindow_onScreenPosition;
            fullTastieraPanel.anchoredPosition = tastiera_onScreenPosition;
        }
        else
        {
            crossWordWindow.anchoredPosition = crossWordWindow_offScreenPosition;
            fullTastieraPanel.anchoredPosition = tastiera_offScreenPosition;
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
        if (fullnessBar.transform.localScale.x != GameManager.instance.Fullness)
        {
            fullnessBar.transform.localScale = Vector3.MoveTowards(fullnessBar.transform.localScale, new Vector3(GameManager.instance.Fullness, 1, 1), Time.deltaTime);
            updateBars = true;
        }
        if (hygeneBar.transform.localScale.x != GameManager.instance.Hygene)
        {
            hygeneBar.transform.localScale = Vector3.MoveTowards(hygeneBar.transform.localScale, new Vector3(GameManager.instance.Hygene, 1, 1), Time.deltaTime);
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
