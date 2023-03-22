using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Golem : MonoBehaviour
{
    public bool CanBeLaunched; //este bool es para idicar si este tipo de golem se pueda lanzar, por ejemplo el jumper no se podra lanazar

    public GolemState State
    { 
        get { return _state; }
        set { ChangeState(value); }
    }

    [SerializeField] private GolemState _state;

    protected Rigidbody2D _rb;
    protected Collider2D _collider;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();      
    }

    protected virtual void Start()
    {
        State = GolemState.Disabled;
    }        

    private void ChangeState(GolemState newState)
    {
        
        switch(newState)
        {
            case GolemState.Disabled:
                _rb.isKinematic = true;
                _collider.enabled = false;
                break;

            case GolemState.Enabled:
                _rb.isKinematic = false;
                _collider.enabled = true;
                break;

            case GolemState.Available:
                _rb.isKinematic = true;
                _collider.enabled = true;
                break;
            case GolemState.BeingLaunched:
                _rb.isKinematic = false;
                _collider.enabled = true;
                break;
        }

        _state = newState;
    }

    public void EnterGolem()
    {
        State = GolemState.Enabled;
    }
}

public enum GolemState
{
    Disabled, Enabled, Available, BeingLaunched
}
