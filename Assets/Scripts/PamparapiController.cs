using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PamparapiController : MonoBehaviour
{
    public static PamparapiController instance;

    private StateMachine sm;
    public Vector2 motionVector;

    private void Awake()
    {
        if(instance)
            Destroy(gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        //sm = new StateMachine();
        //sm.ChangeState(new Idle());
    }

    private void FixedUpdate()
    {
        //sm.Execute();

        if(Input.GetKey(KeyCode.W))
            MovePamparapi(Vector3.up);
        if (Input.GetKey(KeyCode.S))
            MovePamparapi(Vector3.down);
        if (Input.GetKey(KeyCode.D))
            MovePamparapi(Vector3.right);
        if (Input.GetKey(KeyCode.A))
            MovePamparapi(Vector3.left);
    }

    void MovePamparapi(Vector3 movement)
    {
        //Debug.Log("KOs");
        GetComponent<Rigidbody2D>().MovePosition(transform.position + movement * Time.fixedDeltaTime);
        //motionVector = movement;
    }
}
