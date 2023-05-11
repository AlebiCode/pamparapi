using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ColorShifter : MonoBehaviour
{
    private Image image;
    [SerializeField] private float colorDuration = 1f;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        if(image)
            StartCoroutine(ColorCycler());
    }

    private IEnumerator ColorCycler()
    {
        image.color = Color.cyan;
        while (true)
        {
            image.DOColor(new Color(0,0.8f,0), colorDuration);
            yield return new WaitForSeconds(colorDuration);
            image.DOColor(Color.yellow, colorDuration);
            yield return new WaitForSeconds(colorDuration);
            image.DOColor(Color.red, colorDuration);
            yield return new WaitForSeconds(colorDuration);
            image.DOColor(Color.magenta, colorDuration);
            yield return new WaitForSeconds(colorDuration);
            image.DOColor(Color.blue, colorDuration);
            yield return new WaitForSeconds(colorDuration);
            image.DOColor(Color.cyan, colorDuration);
            yield return new WaitForSeconds(colorDuration);
        }
    }
}
