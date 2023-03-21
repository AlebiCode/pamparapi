using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PamparapiController : MonoBehaviour
{
    private StateMachine sm;

    private void Start()
    {
        sm = new StateMachine();
        sm.ChangeState(new Idle());
    }

    private void Update()
    {
        sm.Execute();
    }


}
