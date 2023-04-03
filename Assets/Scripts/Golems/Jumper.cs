using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : Golem
{
    [SerializeField] private LayerMask _groundLayers;
    [SerializeField] private float _minJumpForce, _maxJumpForce, _minJumpTime, _maxJumpTime, _gravityForce;


    private Vector2 _gravity;

    private float _horizontalInput, _nextHorizontalInput;
    private bool _isDelayingHorizontalInput = false;
    private float _horizontalDelayTimer = 0f;

    private bool _isHoldingJumpButton = false;
    private float _holdingJumpButtonTime = 0f;

    private bool _isGrounded = false;

    private Transform _northCollider, _westCollider, _eastCollider;

    private Collider2D _grapplingCollider = null, _lastGrapplingCollider = null;

    private bool _lerpingToGolem = false;
    private float _lerpTime = 0f;
    Vector2 _lerpTarget;

    private float _collisionAngle;


    protected override void Awake()
    {
        base.Awake();

        _gravity = _gravityForce * 9.81f * Vector2.down;

        _northCollider = transform.GetChild(1);
        _westCollider = transform.GetChild(2);
        _eastCollider = transform.GetChild(3);

        _northCollider.gameObject.SetActive(false);
        _westCollider.gameObject.SetActive(false);
        _eastCollider.gameObject.SetActive(false);

    }

    private void Update()
    {
        if (State == GolemState.Available)
        {
            if (_isGrounded)
            {
                _rb.isKinematic = true;
                _rb.velocity = Vector2.zero;
            }
            else if(/*transform.parent == null*/ !IsBeingCarried) _rb.isKinematic = false;
        }
        if (State != GolemState.Enabled) return;

        if (Input.GetButtonDown("Jump") && _isGrounded) _isHoldingJumpButton = true;

        if (Input.GetButtonUp("Jump") && _isGrounded) Jump();
        else
        {
            if (!_isDelayingHorizontalInput)
            {
                _horizontalInput = _nextHorizontalInput;
                _nextHorizontalInput = Input.GetAxisRaw("Horizontal");
            }

            if (_nextHorizontalInput == 0 && _horizontalInput != 0) _isDelayingHorizontalInput = true;

            if (_isDelayingHorizontalInput)
            {
                _horizontalDelayTimer += Time.deltaTime;
                if(_horizontalDelayTimer >= 0.1f)
                {
                    _isDelayingHorizontalInput = false;
                    _horizontalDelayTimer = 0f;
                }   
            }   
        }

        if (_isHoldingJumpButton && _holdingJumpButtonTime < _maxJumpTime) _holdingJumpButtonTime += Time.deltaTime;

    }

    private void FixedUpdate()
    {
        if (State != GolemState.Enabled && State != GolemState.Available) return;

        if(_lerpingToGolem)
        {
            transform.position = Vector2.Lerp(transform.position, _lerpTarget, _lerpTime);
            _lerpTime += Time.fixedDeltaTime * 10f;

            if (Vector2.Distance(transform.position, _lerpTarget) > 0.1f) return;

            _lerpTime = 0f;
            _lerpingToGolem = false;
            _northCollider.gameObject.SetActive(true);
            _westCollider.gameObject.SetActive(false);
            _eastCollider.gameObject.SetActive(false);
        }

        if (!_isGrounded && /*transform.parent == null*/ !IsBeingCarried) _rb.velocity += _gravity * Time.fixedDeltaTime;

    }

    private void Jump()
    {
        _isHoldingJumpButton = false;

        if ((_horizontalInput == -1 && _westCollider.gameObject.activeSelf) 
            || (_horizontalInput == 1 && _eastCollider.gameObject.activeSelf) || IsTalking)
        {
            _holdingJumpButtonTime = 0f;
            return;
        }

        _isGrounded = false;
        _rb.SetRotation(0f);
        _northCollider.gameObject.SetActive(true);
        _westCollider.gameObject.SetActive(false);
        _eastCollider.gameObject.SetActive(false);

        //(valor de x - a) / (b - a)
        float jumpForceFactor = _holdingJumpButtonTime <= _minJumpTime ? 0f : (_holdingJumpButtonTime - _minJumpTime) / (_maxJumpTime - _minJumpTime);

        float jumpForce = Mathf.Lerp(_minJumpForce, _maxJumpForce, jumpForceFactor);

        Vector2 jumpDirection = new Vector2(_horizontalInput / 2f, 1f).normalized;
        

        _rb.velocity = jumpDirection * jumpForce;


        _holdingJumpButtonTime = 0f;
    }
        
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (State != GolemState.Enabled && State != GolemState.Available) return;

        if ((_groundLayers.value & (1 << collision.gameObject.layer)) <= 0) return;
        if (collision.transform.parent == transform) return;

        if(collision.contacts[0].normal.y >= 0f)
        {
            _isGrounded = true;
            
            float newAngle = Vector2.SignedAngle(transform.up, collision.contacts[0].normal);

            _rb.velocity = Vector2.zero;
            _collisionAngle = newAngle;

            _grapplingCollider = collision.collider;
            _lastGrapplingCollider = _grapplingCollider;

            if (((_groundGolemLayer.value & (1 << _grapplingCollider.gameObject.layer)) > 0)
                && !_grapplingCollider.transform.parent.TryGetComponent<Jumper>(out var c))
            {
                _lerpTime = 0f;
                _lerpingToGolem = true;
                _lerpTarget = (Vector2)_grapplingCollider.transform.position + Vector2.up * 0.2f;
                return;
            }

            if (Mathf.Abs(_collisionAngle) < 10f)
            {
                _northCollider.gameObject.SetActive(true);
                _westCollider.gameObject.SetActive(false);
                _eastCollider.gameObject.SetActive(false);
            }
            else if(_collisionAngle < 0f)
            {
                _northCollider.gameObject.SetActive(false);
                _westCollider.gameObject.SetActive(true);
                _eastCollider.gameObject.SetActive(false);
            }
            else
            {
                _northCollider.gameObject.SetActive(false);
                _westCollider.gameObject.SetActive(false);
                _eastCollider.gameObject.SetActive(true);
            }

            _rb.SetRotation(_collisionAngle);

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (State != GolemState.Enabled && State != GolemState.Available) return;
        if (_grapplingCollider != collision.collider) return;
        if (/*transform.parent != null*/ IsBeingCarried) return;


        _grapplingCollider = null;
        _isGrounded = false;
        _rb.SetRotation(0f);
        _northCollider.gameObject.SetActive(true);
        _westCollider.gameObject.SetActive(false);
        _eastCollider.gameObject.SetActive(false);
    }

    protected override void NewState()
    {
        base.NewState();

        if(State == GolemState.Enabled)
        {
            Collider2D col = Physics2D.OverlapCircle(_feet.position, 0.1f, _groundLayers);
            if(col)
            {
                _isGrounded = true;

            }
        }
    }

    protected override void ToggleCarryGolem(bool newState)
    {
        base.ToggleCarryGolem(newState);

    }
}
