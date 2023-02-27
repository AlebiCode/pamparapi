using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    [SerializeField] Canvas canvas;

    [SerializeField] Transform pamparapìStartingCrosswordPosition;
    [SerializeField] Transform pamparapìEndingCrosswordPosition;
    [SerializeField] RectTransform crosswordSpritemaskSize;
    [SerializeField] SpriteMask spriteMask;

    public Canvas Canvas => canvas;
    public Transform PamparapiStartingCrosswordPosition => pamparapìStartingCrosswordPosition;
    public Transform PamparapiEndingCrosswordPosition => pamparapìEndingCrosswordPosition;


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

}
