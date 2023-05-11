using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RandomEmoticons : MonoBehaviour
{
    public static RandomEmoticons instance;

    [SerializeField] private SpriteRenderer fumettoRenderer;
    [SerializeField] private SpriteRenderer emozioneRenderer;

    [SerializeField] private Sprite[] fumetti;
    [SerializeField] private Sprite[] emozioni;

    [SerializeField] private float minTimeBetweenEmotions, maxTimeBetweenEmotions;
    [SerializeField] private float velocitaApparizioneFumetto, velocitaApparizioneEmozione;
    [SerializeField] private float durataEmozione;
    [SerializeField] private Vector2 maxMovementOffset;

    private void Awake()
    {
        if(instance)
            Destroy(gameObject);
        else
            instance = this;
    }

    void Start()
    {
        RestartEmotions();
    }
   
    public void RestartEmotions()
    {
        StopEmotions();
        StartCoroutine(CicloEmozioni());
    }

    public void StopEmotions()
    {
        fumettoRenderer.color = new Color(1, 1, 1, 0);
        emozioneRenderer.color = new Color(1, 1, 1, 0);
        StopAllCoroutines();
    }

    private IEnumerator CicloEmozioni()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBetweenEmotions, maxTimeBetweenEmotions));
            fumettoRenderer.transform.position =
                fumettoRenderer.transform.parent.position + new Vector3(Random.Range(0, maxMovementOffset.x), Random.Range(0, maxMovementOffset.y));
            fumettoRenderer.sprite = fumetti[Random.Range(0, fumetti.Length)];
            fumettoRenderer.DOFade(1, velocitaApparizioneFumetto);
            yield return new WaitForSeconds(velocitaApparizioneFumetto);
            emozioneRenderer.sprite = emozioni[Random.Range(0, emozioni.Length)];
            emozioneRenderer.DOFade(1, velocitaApparizioneEmozione);
            yield return new WaitForSeconds(velocitaApparizioneEmozione);
            yield return new WaitForSeconds(durataEmozione);
            emozioneRenderer.DOFade(0, velocitaApparizioneEmozione);
            yield return new WaitForSeconds(velocitaApparizioneEmozione);
            fumettoRenderer.DOFade(0, velocitaApparizioneFumetto);
            yield return new WaitForSeconds(velocitaApparizioneFumetto);
        }
    }

}
