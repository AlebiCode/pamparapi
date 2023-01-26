using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayFPS : MonoBehaviour
{
    Text fpsText;
    float updateTimer;

    private void Start()
    {
        fpsText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer > 0.2f)
        {
            fpsText.text = ((int)(1.0f / Time.deltaTime)).ToString();
            updateTimer = 0;
        }
    }
}
