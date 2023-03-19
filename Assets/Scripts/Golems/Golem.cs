using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Golem : MonoBehaviour
{
    [SerializeField]
    public GolemState State
    { 
        get { return _state; }
        set { ChangeState(value); }
    }

    protected Rigidbody2D _rb;
    private GolemState _state;

    protected virtual void Awake()
    {
        State = GolemState.Disabled;
        _rb = GetComponent<Rigidbody2D>();
    }

    private void ChangeState(GolemState newState)
    {
        
        switch(newState)
        {
            case GolemState.Disabled:

                break;

            case GolemState.Enabled:

                break;

            case GolemState.Available:

                break;
        }

        _state = newState;
    }
}

public enum GolemState
{
    Disabled, Enabled, Available
}
