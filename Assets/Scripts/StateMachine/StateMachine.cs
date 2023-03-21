using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private IState currentState;

    public void Execute()
    {
        currentState?.Execute();
    }
    public void ChangeState(IState state)
    {
        currentState?.Exit();
        currentState = state;
        currentState.Initialize();
    }
}
