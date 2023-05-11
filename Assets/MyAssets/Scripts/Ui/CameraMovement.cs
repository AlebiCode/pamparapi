using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 finishPos;
    private float _actualPosX;

    private Coroutine slideCoroutine;
    [SerializeField] private float slideSpeed = 1;


    void Update()
    {
        if (Input.touchCount >= 1)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _actualPosX = Camera.main.transform.position.x;     //posizione inziale camera
                    startPos = touch.position;              //posizione inziale dito screen
                    break;
                case TouchPhase.Moved:
                    finishPos = touch.position;             //posizione attuale dito
                    var _sPos = Camera.main.ScreenToWorldPoint(startPos);       //start dito world
                    var _fPos = Camera.main.ScreenToWorldPoint(finishPos);      //attuale dito world
                    var _pos = (_sPos - _fPos);                                 //spostamento dito world
                    Camera.main.transform.position = new Vector3(_actualPosX + _pos.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
                    break;
                case TouchPhase.Ended:
                    checkPosition();
                    break;
            }
        }
    }


    void checkPosition()
    {
        const int leftLimit = -1;           //quanti punti in cui fermarmi a SX ci sono?
        const int rightLimit = 1;           //quanti punti in cui fermarmi a DX ci sono?
        const float size = 5.65f;           //la distanza su asse x tra i punti in cui fermarsi.

        float newX = size * Mathf.Clamp( Mathf.Round(Camera.main.transform.position.x / size), leftLimit, rightLimit );
        Vector3 targetPosition = new Vector3(newX, Camera.main.transform.position.y, Camera.main.transform.position.z);

        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideToTarget(targetPosition));;
    }

    private IEnumerator SlideToTarget(Vector3 targetPosition)
    {
        while (Camera.main.transform.position != targetPosition)
        {
            //Debug.Log("I'm SLIDING!!!!");
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, targetPosition, slideSpeed * Time.deltaTime);
            yield return null;
        }
    }

}
