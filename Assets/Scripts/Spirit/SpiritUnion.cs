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

    [SerializeField] private float _travelingSpeed;
    [SerializeField] private LayerMask _golemLayers;
    

    private SpiritState _state;
    private List<Golem> _golemsInArea, _avaliableGolems;
    private Golem _nearestGolem, _golemInPossession;
    private SpiritMovement _spiritMovement;
    private SpiritDim _spiritDim;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider, _trigger;
    private Rigidbody2D _rb;
    private Transform _target;
    private int _golemIndex = 0;
    private Vector2 _driectionToTarget;

    private void Awake()
    {
        _golemsInArea = new List<Golem>();
        _avaliableGolems = new List<Golem>();
        _spiritMovement = GetComponentInParent<SpiritMovement>();
        _spiritDim = GetComponentInParent<SpiritDim>();
        _spriteRenderer = transform.parent.GetComponentInChildren<SpriteRenderer>();
        _collider = transform.parent.GetComponent<Collider2D>();
        _trigger = GetComponent<Collider2D>();
        _rb = transform.parent.GetComponent<Rigidbody2D>();

        State = SpiritState.Roaming;
    }
    private void Start()
    {
        foreach (Golem golem in GameObject.FindObjectsOfType<Golem>())
        {
            if (golem.StartsScenePossed)
            {
                PossessGolem(golem);
            }
        }
    }

    private void FixedUpdate()
    {
        if (State == SpiritState.Roaming && _golemsInArea.Count >= 2) RecalcNearestGolem();

        else if (State == SpiritState.Possessing) _rb.MovePosition(_golemInPossession.transform.position);

        else if (State == SpiritState.Traveling)
        {
            _driectionToTarget = (_target.position - transform.position).normalized;
            _rb.velocity = _driectionToTarget * _travelingSpeed;
            if (Vector2.Distance(transform.position, _target.position) < 0.2f) StartPossession();
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            if(State == SpiritState.Roaming) PossessNearestGolem();
            

            else if(State == SpiritState.Possessing) ExitGolem();
            
        }

        if (Input.GetButtonDown("Swap") && State != SpiritState.Traveling) SwapGolem();


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
                _collider.enabled = true;
                _trigger.enabled = true;
                _rb.velocity = Vector2.zero;
                _spriteRenderer.enabled = true;
                break;

            case SpiritState.Traveling:
                _spiritMovement.CanMove = false;
                _spiritDim.IsFading = false;
                _collider.enabled = false;
                _trigger.enabled = false;
                break;

            case SpiritState.Possessing:
                _spiritMovement.CanMove = false;
                _spiritDim.IsFading = false;
                _collider.enabled = false;
                _trigger.enabled = false;
                _rb.velocity = Vector2.zero;
                _spriteRenderer.enabled = false;
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

    private void PossessNearestGolem() => PossessGolem(_nearestGolem);
    

    private void PossessGolem(Golem golem)
    {
        if (!golem || State != SpiritState.Roaming) return;

        _golemInPossession = golem;

        if(!_avaliableGolems.Contains(_golemInPossession)) _avaliableGolems.Add(_golemInPossession);

        _golemIndex = _avaliableGolems.IndexOf(_golemInPossession);


        _target = golem.transform;
        _rb.velocity = (_target.position - transform.position).normalized * _travelingSpeed;
        State = SpiritState.Traveling;
    }

    // Añadido por borja:
    public void RefreshAvailableGolems() 
    {
        _avaliableGolems.Clear();
        _golemIndex = 0;
        foreach (Golem golem in GameObject.FindObjectsOfType<Golem>())
        {
            if (golem.State == GolemState.Enabled)
            {
                _avaliableGolems.Add(golem);
            }
        }
    }

    private void SwapGolem()
    {
        if (_avaliableGolems.Count <= 1) return;

        if (State == SpiritState.Possessing) ExitGolem();

        _golemIndex ++;
        if (_golemIndex >= _avaliableGolems.Count) _golemIndex = 0;
        PossessGolem(_avaliableGolems[_golemIndex]);
    }

    private void StartPossession()
    {
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
    Roaming, Possessing, Traveling
}
