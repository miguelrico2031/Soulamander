using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Golem : MonoBehaviour
{
    public bool CanBeLaunched; //este bool es para idicar si este tipo de golem se pueda lanzar, por ejemplo el jumper no se podra lanazar
    public bool StartsScenePossed;
    public bool IsTalking = false;
    public bool IsCarryingGolem
    {
        get { return _isCarryingGolem; }
        set{ ToggleCarryGolem(value);  }
    }

    public bool IsBeingCarried { get; protected set; }

    public GolemState State
    { 
        get { return _state; }
        set { ChangeState(value); }
    }

    [SerializeField] public GameObject TopCollider;
    [SerializeField] protected Transform _feet;
    [SerializeField] protected Collider2D _collider;
    [SerializeField] protected Collider2D _extendedCollider;

    private GolemState _state;
    private bool _isCarryingGolem;
    protected LayerMask _groundGolemLayer;
    protected Rigidbody2D _rb;

    protected Animator _animator;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        _groundGolemLayer = LayerMask.GetMask(new string[] { "GroundGolem" });

        IsCarryingGolem = false;

        _animator = GetComponent<Animator>();
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
                if(_extendedCollider) _extendedCollider.enabled = false;
                if (TopCollider) TopCollider.SetActive(false);
                if (_animator) _animator.SetBool("Enabled", false);
                break;

            case GolemState.Enabled:
                _rb.isKinematic = false;
                _collider.enabled = true;
                if (_extendedCollider) _extendedCollider.enabled = IsCarryingGolem;
                if (TopCollider && !IsCarryingGolem) TopCollider.SetActive(true);
                else if(TopCollider) TopCollider.SetActive(false);
                if (/*transform.parent != null*/ IsBeingCarried) EndStickToGolem();
                if (_animator) _animator.SetBool("Enabled", true);
                break;

            case GolemState.Available:
                //_rb.isKinematic = true;  voy a dejar que cada golem active su isKinematic a su tiempo
                _collider.enabled = true;
                if (TopCollider) TopCollider.SetActive(true);
                if (_extendedCollider)
                {
                    _extendedCollider.enabled = IsCarryingGolem;
                    if (TopCollider) TopCollider.SetActive(!IsCarryingGolem);
                }
                TryToStickToGolem();
                if (_animator) _animator.SetBool("Enabled", false);

                break;

            case GolemState.BeingLaunched:
                _rb.isKinematic = false;
                _collider.enabled = true;
                if (_animator) _animator.SetBool("Enabled", false);
                break;
        }

        _state = newState;
        NewState();
    }

    protected virtual void ToggleCarryGolem(bool newState)
    {
        if (State != GolemState.Available || !_extendedCollider) return;
        if (newState && !_isCarryingGolem)
        {
            TopCollider.SetActive(false);
            _extendedCollider.enabled = true;
        }
        else if (!newState && _isCarryingGolem)
        {
            _extendedCollider.enabled = false;
            TopCollider.SetActive(true);
        }
        _isCarryingGolem = newState;
    }

    protected void TryToStickToGolem()
    {
        if (/*transform.parent != null*/ IsBeingCarried) return;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_feet.position, 0.3f, _groundGolemLayer);
        if (colliders.Length > 0)
        {
            foreach (var col in colliders)
            {
                if (col.transform.parent == transform) continue;
                if (col.transform.parent.TryGetComponent<Jumper>(out var c)) continue;
                if(/*col.transform.parent.parent != null*/IsBeingCarried || IsCarryingGolem) continue;

                StickToGolem(col.transform.parent.GetComponent<Golem>());
                break;
            }
        }
    }

    protected virtual void StickToGolem(Golem golemToStick)
    {
        _rb.isKinematic = true;
        _collider.enabled = false;

        

        transform.SetParent(golemToStick.transform);
        IsBeingCarried = true;
        golemToStick.IsCarryingGolem = true;
    }

    protected virtual void EndStickToGolem()
    {
        transform.parent.GetComponent<Golem>().IsCarryingGolem = false;
        IsBeingCarried = false;
        transform.parent = null;
    }

    public void EnterGolem()
    {
        State = GolemState.Enabled;
    }

    protected virtual void NewState()
    {

    }
}

public enum GolemState
{
    Disabled, Enabled, Available, BeingLaunched
}
