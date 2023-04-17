using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rammer : Golem
{
    public bool IsFacingRight = true;

    private float _direction;
    private bool _isRunning;
    private bool _isAtMaxSpeed;
    private float _speed;
    private float _horizontalInput;
    private bool _isPushing = false;

    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _initialSpeed;
    [SerializeField] private float _pushSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _wallCheckOffsetY;
    [SerializeField] private float _wallCheckOffsetX;
    [SerializeField] private float _groundCheckOffset;

    [SerializeField] private LayerMask _interactableLayers;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _pushableLayer;
    [SerializeField] private LayerMask _destructibleLayer;

    [SerializeField] private Transform[] _wallCheckPoints;

    [SerializeField] private ParticleSystem _dustEffect;
    [SerializeField] private AudioClip[] _footsteps;
    [SerializeField] private AudioClip _hitSound;
    [SerializeField] private float _footstepsSpeed;

    private Vector3[] _groundCheckPoints;

    private Collider2D[] _wallCheckColliders;
    private ContactFilter2D _wallCheckCF;
    private float _footstepsTimer = 0f, _footstepsMaxSpeed;

    protected override void Awake()
    {
        base.Awake();

        _direction = transform.localScale.x > 0 ? 1 : -1;

        _groundCheckPoints = new Vector3[]
        {
            Vector3.zero,
            Vector3.right * _collider.bounds.extents.x,
            Vector3.left * _collider.bounds.extents.x
        };

        _wallCheckCF = new ContactFilter2D() { layerMask = _interactableLayers };

        _footstepsMaxSpeed = _footstepsSpeed * 1.5f;
    }

    private void OnEnable()
    {
        _isPushing = false;
        StopRunning();
    }

    private void Update()
    {
        if (State == GolemState.Available && !_isRunning && !_rb.isKinematic)
        {
            if(RayCastHitGround())
            {
                _rb.isKinematic = true;
                _rb.velocity = Vector2.zero;
                if(_dustEffect.isPlaying) _dustEffect.Stop();
            }
        }

        if(State == GolemState.BeingLaunched)
        {
            if(_isRunning) StopRunning();
            if(RayCastHitGround())
            {
                State = GolemState.Enabled;
                _rb.velocity = Vector3.zero;
                if (_dustEffect.isPlaying) _dustEffect.Stop();
            }
        }

        //if (State == GolemState.BeingLaunched && _isRunning) StopRunning();
        //if (State == GolemState.BeingLaunched && RayCastHitGround())
        //{
        //    State = GolemState.Enabled;
        //    _rb.velocity = Vector3.zero;
        //}

        if (State != GolemState.Enabled) return;

        if (!_isRunning)
        {
            _horizontalInput = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
            if (_horizontalInput != 0 && !PauseGame.Instance.Paused) _direction = _horizontalInput;
        }

        if (IsTalking) _isRunning = false;

        if (Input.GetButtonDown("Jump") && !_isRunning && !IsTalking && !PauseGame.Instance.Paused)
        {
            _isRunning = true;
            _speed = _initialSpeed;
            _animator.SetBool("Running", true);
            _dustEffect.Play();
        }

        //if (_isAtMaxSpeed) Debug.Log("vel max");

        Flip();
    }


    private void FixedUpdate()
    {

        if (State != GolemState.Enabled && State != GolemState.Available) return;
        if (!_isRunning) return;

        if (_speed < _maxSpeed && !_isPushing)
        {
            _isAtMaxSpeed = false;
            _speed += _acceleration * Time.fixedDeltaTime;
            _animator.SetBool("MaxSpeed", false);
        }
        else if (!_isPushing)
        {
            _isAtMaxSpeed = true;
            _speed = _maxSpeed;
            _animator.SetBool("MaxSpeed", true);
        }
        else if (_isPushing)
        {
            _speed = _pushSpeed;
        }
        WallCheck();


        _rb.velocity = new Vector2((_isPushing ? _pushSpeed : _speed) * _direction, _rb.velocity.y);

        _animator.SetFloat("Speed", Mathf.Abs((_isPushing ? _pushSpeed : _speed) * _direction));

        _footstepsTimer += Time.fixedDeltaTime;
        if (_footstepsTimer >= 1f / (_isAtMaxSpeed ? _footstepsMaxSpeed : _footstepsSpeed))
        {
            _footstepsTimer = 0f;
            _audioSource.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
        }
    } 
        
    public void ResetSpeed()
    {
        _speed = _initialSpeed;
        _isAtMaxSpeed = false;
    }

    public void StopRunning()
    {
        _isPushing = false;
        _speed = 0;
        _rb.velocity = new Vector2(0f, _rb.velocity.y);
        _isAtMaxSpeed = false;
        _animator.SetBool("MaxSpeed", false);
        _isRunning = false;
        _animator.SetBool("Running", false);

        if (_dustEffect.isPlaying) _dustEffect.Stop();

        if (State == GolemState.Available && RayCastHitGround())
        { 
            _rb.isKinematic = true;
            _rb.velocity = Vector2.zero;
            if (_dustEffect.isPlaying) _dustEffect.Stop();

        }
    }

    private void Flip()
    {
        if (IsFacingRight && _direction < 0f || !IsFacingRight && _direction > 0f)
        {
            IsFacingRight = !IsFacingRight;

            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private bool RayCastHitGround()
    {
        foreach (var v in _groundCheckPoints)
        {
            if (Physics2D.Raycast(_collider.bounds.center + v, Vector2.down, _collider.bounds.extents.y + _groundCheckOffset, _groundLayer).collider)
                return true;
        }

        return false;
    }
    private void WallCheck()
    {
        foreach (var point in _wallCheckPoints)
        {
            _wallCheckColliders = new Collider2D[5];
            if (Physics2D.OverlapCircle(point.position, 0.1f, _wallCheckCF, _wallCheckColliders) == 0) continue;

            foreach (var col in _wallCheckColliders)
            {
                if (!col) continue;
                if (col.gameObject == gameObject) continue;

                if ((_groundLayer.value & (1 << col.gameObject.layer)) > 0 || col.gameObject.layer == gameObject.layer)
                {
                    StopRunning();
                    _audioSource.PlayOneShot(_hitSound);
                }

                else if ((_pushableLayer.value & (1 << col.gameObject.layer)) > 0)
                {
                    if (!col.GetComponent<PushableObject>().HasHitWall)
                    {
                        _isPushing = true;
                    }
                    else
                    {
                        _isPushing = false;
                        StopRunning();
                    }
                }

                else if ((_destructibleLayer.value & (1 << col.gameObject.layer)) > 0)
                {
                    if (_isAtMaxSpeed)
                    {
                        _audioSource.PlayOneShot(_hitSound);
                        col.GetComponent<DestructibleObject>().DestroyObstacle(this);
                    }
                    else StopRunning();
                }

                return;
            }
        }
    }


    protected override void NewState()
    {
        base.NewState();

        if (State == GolemState.Enabled)
        {
            IsFacingRight = transform.localScale.x > 0f;
            _direction = transform.localScale.x > 0f ? 1 : -1;
        }
    }

}