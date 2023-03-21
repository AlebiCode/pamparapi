using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : IState
{
    public abstract void Initialize();
    public abstract void Execute();
    public abstract void Exit();
}
