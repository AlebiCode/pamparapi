using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NewMov : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 finishPos;
    private float _actualPosX;

    private Coroutine slideCoroutine;
    [SerializeField] private float slideSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
            Application.targetFrameRate = 120;
        else
            Application.targetFrameRate = 120;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //float _cameraPosX = clampingCamera(transform.position.x, -5.65f, 5.65f);
        //transform.position = new Vector3(_cameraPosX, transform.position.y, transform.position.z);

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _actualPosX = transform.position.x;     //posizione inziale camera
                    startPos = touch.position;              //posizione inziale dito screen
                    //Debug.Log("Inizio");
                    break;
                case TouchPhase.Moved:
                    finishPos = touch.position;             //posizione attuale dito
                    var _sPos = Camera.main.ScreenToWorldPoint(startPos);       //start dito world
                    var _fPos = Camera.main.ScreenToWorldPoint(finishPos);      //attuale dito world
                    var _pos = (_sPos - _fPos);                                 //spostamento dito world
                    transform.position = new Vector3(_actualPosX + _pos.x, transform.position.y, transform.position.z);
                    //Debug.Log("Mi muovo");
                    break;
                case TouchPhase.Ended:
                    //Debug.Log("Finisco");
                    //Debug.Log(_actualPosX);
                    //transform.position = new Vector3(0, 0, -10.0f);
                    checkPosition();
                    break;
            }
        }

        
    }


    void checkPosition() //5.65/2
    {
        const int leftLimit = -1;           //quanti punti in cui fermarmi a SX ci sono?
        const int rightLimit = 1;           //quanti punti in cui fermarmi a DX ci sono?
        const float size = 5.65f;           //la distanza su asse x tra i punti in cui fermarsi.

        float newX = size * Mathf.Clamp( Mathf.Round(transform.position.x / size), leftLimit, rightLimit );
        Vector3 targetPosition = new Vector3(newX, transform.position.y, transform.position.z);

        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideToTarget(targetPosition));;
    }

    private IEnumerator SlideToTarget(Vector3 targetPosition)
    {
        while (transform.position != targetPosition)
        {
            //Debug.Log("I'm SLIDING!!!!");
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, slideSpeed * Time.deltaTime);
            yield return null;
        }
    }

}
