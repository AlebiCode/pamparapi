using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

namespace NS_animation
{

    public class AnimationManager : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        [SerializeField] private GameObject pamparapi;
        [SerializeField] private SortingGroup pamparapiSortingGroup;
        [SerializeField] private GameObject leftEyelid, rightEyelid;
        private (Tween, Tween) lidMoveTween, lidRotateTween;

        public EyesStates statoOcchi = EyesStates.ENUM_LENGHT;
        [SerializeField] private float eyesMovementTime;
        [SerializeField] private float eyesRotationTime;

        [SerializeField] Transform pamparapiStartingCrosswordPosition;
        [SerializeField] Transform pamparapiEndingCrosswordPosition;

        //----------------------------------------------------------
        private EyesStates StatoOcchi
        {
            //get { return statoOcchi; }
            set
            {
                if (statoOcchi != value)
                {
                    statoOcchi = value;

                    lidMoveTween.Item1.Kill();
                    lidMoveTween.Item2.Kill();
                    lidRotateTween.Item1.Kill();
                    lidRotateTween.Item2.Kill();
                    (Vector3, Vector3) moveTarget = (new Vector3(), new Vector3());
                    (Vector3, Vector3) rotTarget = (new Vector3(), new Vector3());
                    switch (statoOcchi)
                    { 
                        case EyesStates.open:
                            moveTarget = (new Vector3(0.202f, 0.16f, 0), new Vector3(0.391f, 0.16f, 0));
                            rotTarget = (new Vector3(0, 0, 0), new Vector3(0, 0, 0));
                            break;
                        case EyesStates.closed:
                            moveTarget = (new Vector3(0.202f, 0.11f, 0), new Vector3(0.391f, 0.11f, 0));
                            rotTarget = (new Vector3(0, 0, 0), new Vector3(0, 0, 0));
                            break;
                        case EyesStates.angry:
                            moveTarget = (new Vector3(0.202f, 0.13f, 0), new Vector3(0.391f, 0.13f, 0));
                            rotTarget = (new Vector3(0, 0, -30), new Vector3(0, 0, 30));
                            break;
                        case EyesStates.sad:
                            moveTarget = (new Vector3(0.202f, 0.13f, 0), new Vector3(0.391f, 0.13f, 0));
                            rotTarget = (new Vector3(0, 0, 30), new Vector3(0, 0, -30));
                            break;
                    }
                    lidMoveTween.Item1 = leftEyelid.transform.DOLocalMove(moveTarget.Item1, eyesMovementTime);
                    lidMoveTween.Item2 = rightEyelid.transform.DOLocalMove(moveTarget.Item2, eyesMovementTime);
                    lidRotateTween.Item1 = leftEyelid.transform.DOLocalRotate(rotTarget.Item1, eyesRotationTime);
                    lidRotateTween.Item2 = rightEyelid.transform.DOLocalRotate(rotTarget.Item2, eyesRotationTime);
                    /*if(moveEyelids != null)
                       StopCoroutine(moveEyelids); 
                    moveEyelids = StartCoroutine(MoveEyelids(eyeLidstargetPosRot));*/
                }

            }
        }
        //----------------------------------------------------------

        private void Start()
        {
            StatoOcchi = EyesStates.open;     
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                StatoOcchi = EyesStates.closed;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                StatoOcchi = EyesStates.open;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                StatoOcchi = EyesStates.sad;
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                StatoOcchi = EyesStates.angry;
            }

        }

        private void LateUpdate()
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    StatoOcchi = (EyesStates)((int)statoOcchi + 1);
                    if (statoOcchi == EyesStates.ENUM_LENGHT)
                        StatoOcchi = 0;
                }
            }
        }

        public void MoveToCrosswordScreen()
        {
            StartCoroutine(MoveFromMainToCrosswordScreenCoroutine());
        }

        public void MoveToMainScreen()
        {
            StartCoroutine(MoveFromCrosswordToMainScreenCoroutine());
        }

        private IEnumerator MoveFromMainToCrosswordScreenCoroutine()
        {
            pamparapi.transform.DOMove(pamparapiStartingCrosswordPosition.position, 0.4f);
            pamparapi.transform.DOScale(0.8f, 0.4f);
            yield return new WaitForSeconds(0.4f);
            //UiManager.instance.Canvas.sortingOrder = -100;
            pamparapiSortingGroup.sortingOrder = 51;
            pamparapi.transform.DOMove(pamparapiEndingCrosswordPosition.position, 0.4f);
        }
        private IEnumerator MoveFromCrosswordToMainScreenCoroutine()
        {
            pamparapi.transform.DOScale(1f, 0.4f).SetEase(Ease.OutCirc);

            pamparapi.transform.DOMove(new Vector3(-.8f, 1, 0), 0.2f).SetEase(Ease.OutExpo);
            yield return new WaitForSeconds(0.2f);
            //UiManager.instance.Canvas.sortingOrder = 100;
            pamparapiSortingGroup.sortingOrder = 0;
            pamparapi.transform.DOMove(Vector3.zero, 0.2f).SetEase(Ease.InExpo);
        }

        public enum EyesStates
        {
            closed,
            open,
            angry,
            sad,

            ENUM_LENGHT
        }
    }

}