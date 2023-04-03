using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HairAnimator : MonoBehaviour
{
    [SerializeField] private Hair[] hairList;
    [SerializeField] private float baseSpeed = 1;

    private void Start()
    {
        //StartCoroutine(StandardBounce());
    }

    private void Update()
    {
    }

    void NonLoSo(Vector3 movement)
    {
        foreach (Hair hair in hairList)
        {
            //hair.transform.eulerAngles
        }
    }

    float EaseInOutQuad(float x){
        return x < 0.5 ? 2 * x* x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;
    }

}
