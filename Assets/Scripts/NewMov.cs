using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NewMov : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 finishPos;
    private float _actualPosX;
    // Start is called before the first frame update
    void Start()
    {
        
        
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
                    Debug.Log("Inizio");
                    break;
                case TouchPhase.Moved:
                    finishPos = touch.position;             //posizione attuale dito
                    var _sPos = Camera.main.ScreenToWorldPoint(startPos);       //start dito world
                    var _fPos = Camera.main.ScreenToWorldPoint(finishPos);      //attuale dito world
                    var _pos = (_sPos - _fPos);                                 //spostamento dito world
                    transform.position = new Vector3(_actualPosX + _pos.x, transform.position.y, transform.position.z);
                    Debug.Log("Mi muovo");
                    break;
                case TouchPhase.Ended:
                    Debug.Log("Finisco");
                    Debug.Log(_actualPosX);
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
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        //- - - - - - FUNZIONAMENTO - - - - - - -
        //(transform.position.x / size) = dalla origine 0 a transform.position.x, quanti "size" ci stanno, ovvero: quanti punti in cui mi posso fermare ci sono? (float) (ad esempio: ci stanno 0,7 size)
        //Mathf.Round(^^^^^^^^^^) = arrotondo all'intero più vicino, ovvero: qual'è il punto più vicino in cui mi posso fermare? (ad esempio: il punto 1)
        //Mathf.Clamp(^^^^^^^^^^, leftLimit, rightLimit); = mi assicuro che si rispettino i limiti massimi e minimi.
        //size * ^^^^^^^^^^ = ottengo la coordinata del punto in cui mi posso fermare più vicino (ad esempio: 1 * 5.65f = 5.65f, il punto più vicino in cui fermarmi)




        /*
        switch (_actualPoseX)
        {
            case left:                  //se la posizione iniziale cam è left
                if (_camera > max)          //se posizione attuale cam è > pos iniziale cam + 5.56/2/2
                {
                    transform.position = new Vector3(middle, transform.position.y, transform.position.z);       //vai a middle
                    _actualPosX = middle;                                                                       //la nuova pos iniziale è middle
                }
                else                        //se posizione attuale cam è <= pos iniziale cam + 5.56/2/2
                {
                    transform.position = new Vector3(_actualPoseX, transform.position.y, transform.position.z); //torna pos iniziale
                }
                break;
            case middle:                //se la posizione iniziale cam è middle
                if (_camera < min)          //se posizione attuale cam è < pos iniziale cam - 5.56/2/2
                {
                    transform.position = new Vector3(left, transform.position.y, transform.position.z);         //vai a left
                    _actualPosX = left;                                                                         //la nuova pos iniziale è left
                }
                else if (_camera > max) //se posizione attuale cam è > pos iniziale cam + 5.56/2/2
                {
                    transform.position = new Vector3(right, transform.position.y, transform.position.z);
                    _actualPosX = right;
                }
                else if (_camera > min)
                {
                    transform.position = new Vector3(_actualPoseX, transform.position.y, transform.position.z);
                }
                else if (_camera < max)
                {
                    transform.position = new Vector3(_actualPoseX, transform.position.y, transform.position.z);
                }
                break;
            case right:
                if (_camera < min)
                {
                    transform.position = new Vector3(middle, transform.position.y, transform.position.z);
                    _actualPosX = middle;
                }
                else
                {
                    transform.position = new Vector3(_actualPoseX, transform.position.y, transform.position.z);
                }
                break;
        }
    */
    }
}
