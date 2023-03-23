using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritUnion : MonoBehaviour
{
    public SpiritState State
    {
        get { return _state; }
        private set { ChangeState(value); }
    }


    [SerializeField] private float _unionRadius;
    [SerializeField] private LayerMask _golemLayers;

    private SpiritState _state;
    private List<Golem> _golemsInArea;
    private Golem _nearestGolem, _golemInPossession;
    private SpiritMovement _spiritMovement;
    private SpiritDim _spiritDim;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _golemsInArea = new List<Golem>();
        _spiritMovement = GetComponentInParent<SpiritMovement>();
        _spiritDim = GetComponentInParent<SpiritDim>();
        _spriteRenderer = transform.parent.GetComponentInChildren<SpriteRenderer>();
        _collider = transform.parent.GetComponent<Collider2D>();
        _rb = transform.parent.GetComponent<Rigidbody2D>();

        State = SpiritState.Roaming;
    }

    private void FixedUpdate()
    {
        if (State == SpiritState.Roaming && _golemsInArea.Count >= 2) RecalcNearestGolem();

        else if (State == SpiritState.Possessing) _rb.MovePosition(_golemInPossession.transform.position);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            if(State == SpiritState.Roaming) PossessNearestGolem();
            

            else if(State == SpiritState.Possessing) ExitGolem();
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (State != SpiritState.Roaming) return;
        if ((_golemLayers.value & (1 << collision.gameObject.layer)) <= 0) return;
        

        Golem golem = collision.GetComponentInParent<Golem>();

        AddGolem(golem);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (State != SpiritState.Roaming) return;
        if ((_golemLayers.value & (1 << collision.gameObject.layer)) <= 0) return;

        Golem golem = collision.GetComponentInParent<Golem>();
        RemoveGolem(golem);
    }

    private void ChangeState(SpiritState newState)
    {
        switch(newState)
        {
            case SpiritState.Roaming:
                _spiritMovement.CanMove = true;
                _spiritDim.IsFading = true;
                _spriteRenderer.enabled = true;
                _collider.enabled = true;
                break;
            case SpiritState.Possessing:
                _spiritMovement.CanMove = false;
                _spiritDim.IsFading = false;
                _spriteRenderer.enabled = false;
                _collider.enabled = false;
                break;
        }

        _state = newState;
    }

    private void AddGolem(Golem golem)
    {
        if (_golemsInArea.Contains(golem)) return;
           
        _golemsInArea.Add(golem);
        RecalcNearestGolem();
    }

    private void RemoveGolem(Golem golem)
    {
        if (!_golemsInArea.Contains(golem)) return;

        _golemsInArea.Remove(golem);

        if(golem == _nearestGolem)
        {
            _nearestGolem.GetComponent<SpriteRenderer>().color = Color.green;
            _nearestGolem = null;
        }

        RecalcNearestGolem();

    }

    private void RecalcNearestGolem()
    {
        if (_golemsInArea.Count == 0) return;

        if (_golemsInArea.Count == 1)
        {
            _nearestGolem = _golemsInArea[0];
            _nearestGolem.GetComponent<SpriteRenderer>().color = Color.yellow;
            return;
        }

        float currentDistance = float.MaxValue;
        float distance = 0f;
        foreach(Golem golem in _golemsInArea)
        {
            if(!_nearestGolem)
            {
                _nearestGolem = golem;
                currentDistance = Vector2.Distance(golem.transform.position, transform.position);
                continue;
            }

            distance = Vector2.Distance(golem.transform.position, transform.position);
            if (distance < currentDistance)
            {
                _nearestGolem.GetComponent<SpriteRenderer>().color = Color.green;
                _nearestGolem = golem;
                currentDistance = distance;
            }
        }
        _nearestGolem.GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    private void PossessNearestGolem()
    {
        if (!_nearestGolem || State != SpiritState.Roaming) return;

        _golemInPossession = _nearestGolem;

        State = SpiritState.Possessing;
        _golemInPossession.State = GolemState.Enabled;
        _golemInPossession.GetComponent<SpriteRenderer>().color = Color.blue;
        

    }

    private void ExitGolem()
    {
        if (State != SpiritState.Possessing) return;

        State = SpiritState.Roaming;
        _golemInPossession.State = GolemState.Available;
        _golemInPossession.GetComponent<SpriteRenderer>().color = Color.yellow;
        _golemInPossession = null;
    }
}

public enum SpiritState
{
    Roaming, Possessing
}
