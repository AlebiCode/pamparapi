using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace NS_animation
{

    public class AnimationManager : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        [SerializeField] private GameObject leftEyelid, rightEyelid;
        private (Tween, Tween) moveTween, rotateTween;

        public EyesStates statoOcchi = EyesStates.ENUM_LENGHT;
        [SerializeField] private float eyesMovementTime;
        [SerializeField] private float eyesRotationTime;

        //----------------------------------------------------------
        private EyesStates StatoOcchi
        {
            //get { return statoOcchi; }
            set
            {
                if (statoOcchi != value)
                {
                    statoOcchi = value;

                    moveTween.Item1.Kill();
                    moveTween.Item2.Kill();
                    rotateTween.Item1.Kill();
                    rotateTween.Item2.Kill();
                    (Vector3, Vector3) moveTarget = (new Vector3(), new Vector3());
                    (Vector3, Vector3) rotTarget = (new Vector3(), new Vector3());
                    switch (statoOcchi)
                    { 
                        case EyesStates.open:
                            moveTarget = (new Vector3(0.067f, .03f, 0), new Vector3(0.211f, .03f, 0));
                            rotTarget = (new Vector3(0, 0, 0), new Vector3(0, 0, 0));
                            break;
                        case EyesStates.closed:
                            moveTarget = (new Vector3(0.067f, -.01f, 0), new Vector3(0.211f, -.01f, 0));
                            rotTarget = (new Vector3(0, 0, 0), new Vector3(0, 0, 0));
                            break;
                        case EyesStates.angry:
                            moveTarget = (new Vector3(0.08f, .03f, 0), new Vector3(0.2f, .03f, 0));
                            rotTarget = (new Vector3(0, 0, -30), new Vector3(0, 0, 30));
                            break;
                        case EyesStates.sad:
                            moveTarget = (new Vector3(0.05f, .03f, 0), new Vector3(0.22f, .03f, 0));
                            rotTarget = (new Vector3(0, 0, 30), new Vector3(0, 0, -30));
                            break;
                    }
                    moveTween.Item1 = leftEyelid.transform.DOLocalMove(moveTarget.Item1, eyesMovementTime);
                    moveTween.Item2 = rightEyelid.transform.DOLocalMove(moveTarget.Item2, eyesMovementTime);
                    rotateTween.Item1 = leftEyelid.transform.DOLocalRotate(rotTarget.Item1, eyesRotationTime);
                    rotateTween.Item2 = rightEyelid.transform.DOLocalRotate(rotTarget.Item2, eyesRotationTime);
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
                StatoOcchi = (EyesStates)((int)statoOcchi + 1);
                if (statoOcchi == EyesStates.ENUM_LENGHT)
                    StatoOcchi = 0;
            }
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