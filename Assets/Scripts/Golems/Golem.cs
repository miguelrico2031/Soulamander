using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Golem : MonoBehaviour
{
    public bool CanBeLaunched; //este bool es para idicar si este tipo de golem se pueda lanzar, por ejemplo el jumper no se podra lanazar
    public bool StartsScenePossed;

    public GolemState State
    { 
        get { return _state; }
        set { ChangeState(value); }
    }

    [SerializeField] protected GameObject _topCollider;

    private GolemState _state;
    protected LayerMask _groundGolemLayer;
    protected Rigidbody2D _rb;
    protected Collider2D _collider;
    

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

        _groundGolemLayer = LayerMask.GetMask(new string[] { "GroundGolem" });
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
                if (_topCollider) _topCollider.SetActive(false);
                break;

            case GolemState.Enabled:
                _rb.isKinematic = false;
                _collider.enabled = true;
                if (_topCollider) _topCollider.SetActive(true);
                if (transform.parent != null) transform.parent = null;
                break;

            case GolemState.Available:
                //_rb.isKinematic = true;  voy a dejar que cada golem active su isKinematic a su tiempo
                _collider.enabled = true;

                TryToStickToGolem();

                if (_topCollider) _topCollider.SetActive(true);
                break;

            case GolemState.BeingLaunched:
                _rb.isKinematic = false;
                _collider.enabled = true;
                break;
        }

        _state = newState;
    }

    protected void TryToStickToGolem()
    {
        return;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.65f, _groundGolemLayer);
        if (colliders.Length > 0)
        {
            foreach (var col in colliders)
            {
                if (col.transform.parent == transform) continue;
                if (col.transform.parent.TryGetComponent<Jumper>(out var c)) continue;

                transform.SetParent(col.transform);
                break;
            }
        }
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
