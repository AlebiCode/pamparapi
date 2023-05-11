using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource inputSource, pamparapiSource, crossWordSource; 

    [SerializeField] private AudioClip
        keyTapHigh,
        keyTapLow,
        correctWord,
        wonCrossword,
        pamparapìHappy,
        click,
        purchaseSound;

    private void Awake()
    {
        if(instance)
            Destroy(this);
        else
            instance = this;
    }

    public void PlayHappyPamparapi()
    {
        pamparapiSource.clip = pamparapìHappy;
        pamparapiSource.Play();
    }

    public void PlayGenericInputSound()
    {
        inputSource.clip = click;
        inputSource.Play();
    }
    public void PlayPurchaseSound()
    {
        inputSource.clip = purchaseSound;
        inputSource.Play();
    }

    public void PlayCrosswordCorrectWordSound()
    {
        crossWordSource.Stop();
        crossWordSource.clip = correctWord;
        crossWordSource.Play();
    }

    public void PlayCrosswordWonGameSound()
    {
        crossWordSource.Stop();
        crossWordSource.clip = wonCrossword;
        crossWordSource.Play();
    }

    public void PlayKeyTapHigh()
    {
        inputSource.clip = keyTapHigh;
        inputSource.Play();
    }
    public void PlayKeyTapLow()
    {
        inputSource.clip = keyTapLow;
        inputSource.Play();
    }

}
