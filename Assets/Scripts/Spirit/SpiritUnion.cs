using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class SpiritUnion : MonoBehaviour
{
    public SpiritState State
    {
        get { return _state; }
        set { ChangeState(value); }
    }

    public bool CanSwap = true;
    public bool CanInteract = true;

    [SerializeField] private float _travelingSpeed, _vacuumSpeed;
    [SerializeField] private LayerMask _golemUnionLayer, _golemLayer;
    [SerializeField] private Collider2D _golemTrigger, _npcTrigger;

    [SerializeField] private Material _defaultMaterial, _outlineMaterial;
    [SerializeField] private ParticleSystem _fireTrail, _fireTrailPossess,_soulSucked;
    [SerializeField] private Color _travelingColor;

    [SerializeField] private AudioClip _possessSound;

    private SpiritState _state;
    private List<Golem> _golemsInArea, _avaliableGolems;
    private Golem _nearestGolem, _golemInPossession;
    private SpiritMovement _spiritMovement;
    private SpiritDim _spiritDim;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private Rigidbody2D _rb;
    private Transform _target;
    private int _golemIndex = 0;
    private Vector2 _directionToTarget;
    private int _npcLayer;
    private NPC _npcInArea;
    private bool _justEndedDialogue = false;
    private Light2D _light;
    private AudioSource _audioSource;
    private Color _defaultColor;
    


    private void Awake()
    {
        _golemsInArea = new List<Golem>();
        _avaliableGolems = new List<Golem>();
        _spiritMovement = GetComponentInParent<SpiritMovement>();
        _spiritDim = GetComponentInParent<SpiritDim>();
        _spriteRenderer = transform.parent.GetComponentInChildren<SpriteRenderer>();
        _collider = transform.parent.GetComponent<Collider2D>();
        _golemTrigger = GetComponent<Collider2D>();
        _rb = transform.parent.GetComponent<Rigidbody2D>();
        _light = transform.parent.GetComponentInChildren<Light2D>();
        _audioSource = GetComponent<AudioSource>();
        _defaultColor = _light.color;
        _npcLayer = LayerMask.NameToLayer("NPC");
        State = SpiritState.Roaming;
        
    }
    private IEnumerator Start()
    {
        foreach(var field in FindObjectsByType<ParticleSystemForceField>(FindObjectsSortMode.None))
        {
            _soulSucked.externalForces.AddInfluence(field);
        }
        
        foreach (Golem golem in GameObject.FindObjectsOfType<Golem>())
        {           
            if (golem.StartsScenePossed)
            {
                yield return new WaitForSeconds(0.1f);
                PossessGolem(golem);
                yield break;
            }
        }
    }

    private void FixedUpdate()
    {
        switch (State)
        {
            case SpiritState.Roaming:
                if (_golemsInArea.Count >= 2) RecalcNearestGolem();
                break;

            case SpiritState.Possessing:
                _rb.MovePosition(_golemInPossession.transform.position);
                break;

            case SpiritState.Traveling:
                _directionToTarget = (_target.position - transform.position).normalized;
                _rb.velocity = _directionToTarget * _travelingSpeed * Time.fixedDeltaTime;
                if (Vector2.Distance(transform.position, _target.position) < 0.2f) StartPossession();
                break;

            case SpiritState.Vacuum:
                _directionToTarget = (_target.position - transform.position).normalized;
                _rb.velocity = _directionToTarget * _vacuumSpeed * Time.fixedDeltaTime;
                if (Vector2.Distance(transform.position, _target.position) < 0.2f)
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact") && !PauseGame.Instance.Paused && CanInteract)
        {
            if (!_npcInArea)
            {
                if (State == SpiritState.Roaming) PossessNearestGolem();

                else if (State == SpiritState.Possessing) ExitGolem();
            }
            else TalktoNPC();
        }

        if (Input.GetButtonDown("Swap") &&
            State != SpiritState.Traveling && State != SpiritState.Vacuum && CanSwap && !PauseGame.Instance.Paused)
            SwapGolem();


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (State == SpiritState.Roaming)
        {
            if ((_golemUnionLayer.value & (1 << collision.gameObject.layer)) <= 0) return;

            Golem golem = collision.GetComponentInParent<Golem>();

            AddGolem(golem);
        }
        else if(State == SpiritState.Possessing)
        {
            if (collision.gameObject.layer != _npcLayer) return;
            _npcInArea = collision.GetComponent<NPC>();
            _npcInArea.GetComponent<SpriteRenderer>().material = _outlineMaterial;
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (State == SpiritState.Roaming)
        {
            if ((_golemUnionLayer.value & (1 << collision.gameObject.layer)) <= 0) return;

            Golem golem = collision.GetComponentInParent<Golem>();
            RemoveGolem(golem);
        }
        else if (State == SpiritState.Possessing)
        {
            if (!_npcInArea) return;
            if (collision.gameObject != _npcInArea.gameObject) return;

            _npcInArea.GetComponent<SpriteRenderer>().material = _defaultMaterial;
            _npcInArea = null;

        }
    }
        

    private void ChangeState(SpiritState newState)
    {
        switch(newState)
        {
            case SpiritState.Roaming:
                _spiritMovement.CanMove = true;
                _spiritDim.IsFading = true;
                _collider.enabled = true;
                _golemTrigger.enabled = true;
                _npcTrigger.enabled = false;
                _rb.velocity = Vector2.zero;
                _spriteRenderer.enabled = true;
                _fireTrail.Play();
                _fireTrailPossess.Stop();
                _light.enabled = true;
                _light.color = _defaultColor;
                break;

            case SpiritState.Traveling:
            case SpiritState.Vacuum:
                _spiritMovement.CanMove = false;
                _spiritDim.IsFading = false;
                _collider.enabled = false;
                _golemTrigger.enabled = false;
                _npcTrigger.enabled = false;
                _fireTrail.Play();
                _fireTrailPossess.Stop();
                _light.enabled = true;
                _light.color = _travelingColor;
                break;

            case SpiritState.Possessing:
                _spiritMovement.CanMove = false;
                _spiritDim.IsFading = false;
                _collider.enabled = false;
                _golemTrigger.enabled = false;
                _npcTrigger.enabled = true;
                _rb.velocity = Vector2.zero;
                _spriteRenderer.enabled = false;
                _fireTrail.Stop();
                _fireTrailPossess.Play();
                _light.color = _defaultColor;
                _light.enabled = false;
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
            _nearestGolem.GetComponent<SpriteRenderer>().material = _defaultMaterial;
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
            _nearestGolem.GetComponent<SpriteRenderer>().material = _outlineMaterial;
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
                //_nearestGolem.GetComponent<SpriteRenderer>().color = Color.green;
                _nearestGolem.GetComponent<SpriteRenderer>().material = _defaultMaterial;
                _nearestGolem = golem;
                currentDistance = distance;
            }
        }
        //_nearestGolem.GetComponent<SpriteRenderer>().color = Color.yellow;
        _nearestGolem.GetComponent<SpriteRenderer>().material = _outlineMaterial;
    }

    private void PossessNearestGolem() => PossessGolem(_nearestGolem);
    

    private void PossessGolem(Golem golem)
    {
        if (!golem || State != SpiritState.Roaming) return;
        if (CheckIfOverlapping(golem)) return;

       
        _golemInPossession = golem;

        if(!_avaliableGolems.Contains(_golemInPossession)) _avaliableGolems.Add(_golemInPossession);

        _golemIndex = _avaliableGolems.IndexOf(_golemInPossession);


        _target = golem.transform;
        _rb.velocity = (_target.position - transform.position).normalized * _travelingSpeed;
        State = SpiritState.Traveling;

        if (golem.State == GolemState.Disabled && golem.StartsScenePossed) return;
        
        _audioSource.PlayOneShot(_possessSound);
    }

    public void VacuumSpirit(Transform vacuum)
    {
        if (State == SpiritState.Possessing) ExitGolem();
        else if (State != SpiritState.Roaming) State = SpiritState.Roaming;

        _soulSucked.Play();

        _target = vacuum;
        _rb.velocity = (_target.position - transform.position).normalized * _travelingSpeed;
        State = SpiritState.Vacuum;
    }

    public void SuckSpirit(bool enter)
    {
        if (enter)
        {
            _soulSucked.Play();
        }
        else _soulSucked.Stop();
    }

    // Añadido por borja:
    public void RefreshAvailableGolems() 
    { 
        _avaliableGolems.Clear();
        _golemIndex = 0;
        foreach (Golem golem in GameObject.FindObjectsOfType<Golem>())
        {
            if (golem.State == GolemState.Enabled || golem.State == GolemState.Available)
            {
                _avaliableGolems.Add(golem);
            }
        }
    }

    private void SwapGolem()
    {
        if (_avaliableGolems.Count == 0) return;

        else if (_avaliableGolems.Count == 1)
        {
            if(State == SpiritState.Roaming) PossessGolem(_avaliableGolems[_golemIndex]);
            return;
        }

        if (State == SpiritState.Possessing) ExitGolem();
        _golemIndex ++;
        if (_golemIndex >= _avaliableGolems.Count) _golemIndex = 0;
        PossessGolem(_avaliableGolems[_golemIndex]);
    }

    private void StartPossession()
    {
        State = SpiritState.Possessing;
        _golemInPossession.State = GolemState.Enabled;
        _golemInPossession.GetComponent<SpriteRenderer>().material = _defaultMaterial;
    }

    public void ExitGolem()
    {
        if (State != SpiritState.Possessing) return;

        State = SpiritState.Roaming;
        _golemInPossession.State = GolemState.Available;
        
        _golemInPossession = null;
    }

    private void TalktoNPC()
    {
        if (_golemInPossession.IsTalking) return;
        if(_justEndedDialogue)
        {
            _justEndedDialogue = false;
            return;
        }

        _npcInArea.GetComponent<SpriteRenderer>().material = _defaultMaterial;

        CanSwap = false;
        _golemInPossession.IsTalking = true;
        DialogueUI.Instance.OnDialogueEnd.AddListener(EndTalkToNPC);
        DialogueUI.Instance.StartDialogue(_npcInArea.GetDialogue());

    }

    public void EndTalkToNPC()
    {
        DialogueUI.Instance.OnDialogueEnd.RemoveListener(EndTalkToNPC);
        CanSwap = true;
        _golemInPossession.IsTalking = false;
        _justEndedDialogue = true;

        _npcInArea.GetComponent<SpriteRenderer>().material = _outlineMaterial;
    }

    private bool CheckIfOverlapping(Golem golem)
    {
        if (golem.State != GolemState.Disabled) return false;
        CircleCollider2D golemUnionCol = golem.GetComponentInChildren<CircleCollider2D>();
        var c = Physics2D.OverlapCircle(golemUnionCol.bounds.center, golemUnionCol.radius, _golemLayer);
        if(c) Debug.Log(c.gameObject);
        return c;
    }
}

public enum SpiritState
{
    Roaming, Possessing, Traveling, Vacuum
}
